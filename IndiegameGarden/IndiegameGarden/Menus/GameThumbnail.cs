// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Base;
using IndiegameGarden.Download;
using MyDownloader.Core;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A thumbnail showing a single game, with scaling, auto-loading and downloading of image data.
    /// </summary>
    public class GameThumbnail: EffectSpritelet
    {
        /// <summary>
        /// my color change behavior used eg for fading in/out
        /// </summary>
        public ColorChangeBehavior ColorB;

        /// <summary>
        /// ref to game for which this thumbnail is
        /// </summary>
        public GardenItem Game;

        /// <summary>
        /// actual/intended full path to local thumbnail file (the file may or may not exist)
        /// </summary>
        protected string ThumbnailFilepath
        {
            get
            {
                return GardenConfig.Instance.GetThumbnailFilepath(Game);
            }
        }

        /// <summary>
        /// thumbnail loading task
        /// </summary>
        ITask loaderTask;

        /// <summary>
        /// when a new texture is available, it's passed via this var and using corresponding lock object
        /// </summary>
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();

        /// <summary>
        /// indicate whether texture already loaded
        /// </summary>
        bool isLoaded = false;
        
        /// <summary>
        /// a time variable for rendering shader 'halo' around a thumbnail
        /// </summary>
        float haloTime = 0f;

        // a default texture to use if no thumbnail has been loaded yet
        static Texture2D DefaultTexture;

        /**
         * internal Task to load a thumbnail from disk, or download it first if not available.
         * To be called in a separate thread e.g. by using a ThreadedTask.
         */
        class GameThumbnailLoadTask : ThumbnailDownloader
        {
            // my parent - where to load for/to
            GameThumbnail parent;

            public GameThumbnailLoadTask(GameThumbnail th): 
                base(th.Game)
            {
                parent = th;
            }

            /// <summary>
            /// loads image from file if it exists, or else by downloading and then loading from file
            /// </summary>
            protected override void StartInternal()
            {
                if (File.Exists(parent.ThumbnailFilepath))
                {
                    bool ok = parent.LoadTextureFromFile();
                    if (ok)
                        status = ITaskStatus.SUCCESS;
                    else
                        status = ITaskStatus.FAIL;
                }
                else
                {
                    // first run the base downloading task now. If that is ok, then load from file downloaded.
                    base.StartInternal();

                    if (File.Exists(parent.ThumbnailFilepath) && IsSuccess() )
                    {
                        bool ok = parent.LoadTextureFromFile();
                        if (ok)
                            status = ITaskStatus.SUCCESS;
                        else
                            status = ITaskStatus.FAIL;
                    }
                    else
                    {
                        status = ITaskStatus.FAIL;
                    }
                }

                // after a successful load, enable the thumbnail.
                if (IsSuccess())
                {
                    parent.Enable();
                }
            }
        } // class

        public GameThumbnail(GardenItem game)
            : base( (Texture2D) null, "GameThumbnail")
        {
            ColorB = new ColorChangeBehavior();         
            Add(ColorB);
            Motion.Scale = GardenGamesPanel.THUMBNAIL_SCALE_UNSELECTED;
            Game = game;
            // effect is still off if no bitmap loaded yet
            EffectEnabled = false;
            // first-time texture init
            if (DefaultTexture == null)
            {
                DefaultTexture = GardenGame.Instance.Content.Load<Texture2D>("ball-supernova2");
            }
            // use default texture as long as thumbnail not loaded yet
            Texture = DefaultTexture;
        }

        public override void Dispose()
        {
            if (loaderTask != null)
                loaderTask.Abort();
            loaderTask = null;

            base.Dispose();
        }

        /// <summary>
        /// test whether the image for this thumbnail has already been (down)loaded or not.
        /// </summary>
        /// <returns>true if image has been loaded, false if not yet or not successfully</returns>
        public bool IsLoaded()
        {
            return ((loaderTask != null) && (loaderTask.IsSuccess() ));
        }

        /// <summary>
        /// trigger the background loading in a thread of the game's thumbnail image.
        /// Does nothing if loading already in progress or finished.
        /// </summary>
        public void LoadInBackground()
        {
            if (loaderTask == null)
            {
                loaderTask = new ThreadedTask(new GameThumbnailLoadTask(this));
                loaderTask.Start();
            }
        }

        /// <summary>
        /// makes thumbnail visible, setting EffectEnabled properly, and loading thumbnail image if not yet loaded/loading.
        /// </summary>
        public void Enable()
        {
            Visible = true;
            EffectEnabled = (Texture != DefaultTexture);
            if (loaderTask == null)
            {
                loaderTask = new ThreadedTask(new GameThumbnailLoadTask(this));
                loaderTask.Start();
            }
        }

        /// <summary>
        /// get the manual scaling if specified, or else the auto-scaling value for game thumbnail size 
        /// </summary>
        protected float ThumbnailScale
        {
            get
            {
                float sc = Game.ScaleIcon;
                if (Texture != null && sc == 1.0f)
                {
                    // scale back in width and height when needed
                    float scw = GardenGamesPanel.THUMBNAIL_MAX_WIDTH_PIXELS / ((float)Texture.Width);
                    float sch = GardenGamesPanel.THUMBNAIL_MAX_HEIGHT_PIXELS / ((float)Texture.Height);
                    sc *= (scw < sch ? scw : sch ); // scale using the smallest value found
                }
                return sc;
            }
        }

        /// <summary>
        /// (re) loads texture from a file and puts in internal updatedTexture var,
        /// which replaces the present texture in next Update() round.
        /// </summary>
        /// <returns>true if loaded ok, false if failed (e.g. no file or invalid file)</returns>
        protected bool LoadTextureFromFile()
        {
            Texture2D tex = null;
            try
            {
                tex = LoadBitmap(ThumbnailFilepath, "", true);
            }
            catch (InvalidOperationException)
            {
                return false; // TODO be able to log the error somehow
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            lock (updateTextureLock)
            {
                updatedTexture = tex;
            }
            return true;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // animation of loading
            if (!isLoaded)
            {
                Motion.RotateModifier += SimTime / 6.0f;
            }

            // adapt scale according to GameItem preset
            Motion.ScaleModifier *= ThumbnailScale;

            // check if a new texture has been loaded in background
            if (updatedTexture != null)
            {
                // yes: replace texture by new one
                lock (updateTextureLock)
                {
                    Texture = updatedTexture;
                    updatedTexture = null;
                    isLoaded = true;
                }
            }

            // effect on when FX mode says so, and only if thumbnail is loaded
            if (isLoaded)
                EffectEnabled = (Game.IsGrowable); // TODO && Game.IsInstalled ?;

            if (EffectEnabled)
            {
                Motion.ScaleModifier *= (1f / 0.7f); // this extends image for shader fx region, see .fx file
                haloTime += p.Dt; // move the 'halo' of the icon onwards as long as it's visible.
            }
        }

        protected override void OnDraw(ref DrawParams p)
        {
            if (timeParam != null)
                timeParam.SetValue(SimTime);
            if (positionParam != null)
                positionParam.SetValue(Motion.Position);

            Color col = DrawInfo.DrawColor;
            if (EffectEnabled)
            {
                // this is a conversion from 'halotime' to the time format that can be given to the pixel shader
                // via the 'draw color' parameter
                int t = (int) (haloTime * 256);
                int c3 = t % 256;
                int c2 = ((t - c3)/256) % 256;
                int c1 = ((t - c2 - c3)/65536) % 256;
                col = new Color(c1, c2, c3, col.A);
            }
            MySpriteBatch.Draw(Texture, DrawInfo.DrawPosition, null, col,
                   Motion.RotateAbs, DrawInfo.DrawCenter, DrawInfo.DrawScale, SpriteEffects.None, DrawInfo.LayerDepth);

        }
    }
}
