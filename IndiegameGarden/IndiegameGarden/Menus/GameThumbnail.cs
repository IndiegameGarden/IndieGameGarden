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
    /// A thumbnail showing a single game, with auto-loading and downloading of image data.
    /// </summary>
    public class GameThumbnail: EffectSpritelet
    {
        /// <summary>
        /// my color change behavior
        /// </summary>
        public ColorChangeBehavior ColorB;

        /// <summary>
        /// ref to game for which this thumbnail is
        /// </summary>
        public GardenItem Game;

        /// <summary>
        /// actual/intended filename of thumbnail file (the file may or may not exist)
        /// </summary>
        public string ThumbnailFilename
        {
            get
            {
                return GardenGame.Instance.Config.GetThumbnailFilepath(Game);
            }
        }

        /// <summary>
        /// thumbnail loading task
        /// </summary>
        ITask loaderTask;
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();
        bool isLoaded = false;
        
        float HaloTime = 0f;

        // a default texture to use if no thumbnail has been loaded yet
        static Texture2D DefaultTexture;

        /**
         * internal Task to load a thumbnail from disk, or download it first if not available.
         * To be called in a separate thread e.g. ThreadedTask.
         */
        class GameThumbnailLoadTask : ThumbnailDownloader
        {
            // my parent - where to load for/to
            GameThumbnail thumbnail;

            public GameThumbnailLoadTask(GameThumbnail th): 
                base(th.Game)
            {
                thumbnail = th;
            }

            /// <summary>
            /// loads image from either file if it exists, or else by download
            /// </summary>
            protected override void StartInternal()
            {
                if (File.Exists(thumbnail.ThumbnailFilename))
                {
                    thumbnail.LoadTextureFromFile();
                    status = ITaskStatus.SUCCESS;
                }
                else
                {
                    // first run the base downloading task now. If that is ok, then load from file downloaded.
                    base.StartInternal();

                    if (File.Exists(thumbnail.ThumbnailFilename) && IsSuccess() )
                    {
                        thumbnail.LoadTextureFromFile();
                        status = ITaskStatus.SUCCESS;
                    }
                    else
                    {
                        status = ITaskStatus.FAIL;
                    }
                }

                if (IsSuccess())
                {
                    thumbnail.Enable();
                }
            }
        } // class

        public GameThumbnail(GardenItem game)
            : base( (Texture2D) null,"GameThumbnail")
        {
            ColorB = new ColorChangeBehavior();         
            Add(ColorB);
            Motion.Scale = GardenGamesPanel.THUMBNAIL_SCALE_UNSELECTED_UNINSTALLED;
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
            base.Dispose();
            if (loaderTask != null)
                loaderTask.Abort();
            loaderTask = null;
        }

        public bool IsLoaded()
        {
            return ((loaderTask != null) && (loaderTask.IsSuccess() ));
        }

        public void LoadInBackground()
        {
            if (loaderTask == null)
            {
                loaderTask = new ThreadedTask(new GameThumbnailLoadTask(this));
                loaderTask.Start();
            }
        }

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
        /// (re) loads texture from a file and puts in updatedTexture var 
        /// </summary>
        /// <returns>true if loaded ok, false if failed (e.g. no file or invalid file)</returns>
        protected bool LoadTextureFromFile()
        {
            Texture2D tex = null;
            try
            {
                tex = LoadBitmap(ThumbnailFilename, "", true);
            }
            catch (InvalidOperationException)
            {
                return false;
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

            // rotate thumbnail if specified
            if (Game.RotateSpeed != 0f)
            {
                Motion.RotateModifier += SimTime * Game.RotateSpeed;
            }

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

            // effect on when FX mode says so, and thnail is loaded
            if (isLoaded)
                EffectEnabled = (Game.FXmode > 0); // DEBUG isLoaded && (Game.FXmode > 0) && Game.IsInstalled;

            if (EffectEnabled)
            {
                Motion.ScaleModifier *= (1f / 0.7f);
                HaloTime += p.Dt; // move the 'halo' of the icon onwards as long as it's visible.
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
                int t = (int) (HaloTime * 256);
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
