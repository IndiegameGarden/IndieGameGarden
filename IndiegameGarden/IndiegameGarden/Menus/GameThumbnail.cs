// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
using IndiegameGarden.Util;

using MyDownloader.Core;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A thumbnail showing a single game, with scaling, auto-loading and downloading of image data.
    /// </summary>
    public class GameThumbnail: Spritelet
    {
        // static Vector2 PROGRESS_BAR_POSITION_RELATIVE = new Vector2(-0.08f, -0.08f);

        /// <summary>
        /// my color change behavior used eg for fading in/out
        /// </summary>
        public ColorChangeBehavior ColorB;

        /// <summary>
        /// ref to game for which this thumbnail is
        /// </summary>
        public GardenItem Game;

        public ProgressBar progBar;

        public bool IsFadingOut = false;

        public bool IsPlaying = false;

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

        public static int MaxConcurrentDownloads = 2;

        public static bool IsNewDownloadAllowed
        {
            get
            {
                return (numLoadingTasksActive < MaxConcurrentDownloads);
            }
        }

        /// <summary>
        /// number of thumbnail loading tasks ongoing. Used to limit downloads to MaxConcurrentDownloads
        /// </summary>
        static volatile int numLoadingTasksActive = 0;

        /// <summary>
        /// thumbnail loading task
        /// </summary>
        ITask loaderTask;

        /// <summary>
        /// when a new texture is available, it's passed via this var and using corresponding lock object
        /// </summary>
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();

        // a default texture to use if no thumbnail has been loaded yet
        static Texture2D DefaultTexture;

        /**
         * internal Task to load a thumbnail from disk
         * To be called in a separate thread e.g. by using a ThreadedTask.
         */
        class GameThumbnailLoadTask : Task
        {

            // my parent - where to load for/to
            GameThumbnail parent;

            public GameThumbnailLoadTask(GameThumbnail th)
            {                
                parent = th;
            }

            /// <summary>
            /// loads image from file if it exists, or else by downloading and then loading from file
            /// </summary>
            protected override void StartInternal()
            {
                numLoadingTasksActive++;
                try{
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
                        if (File.Exists(parent.ThumbnailFilepath) && IsSuccess())
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
                finally
                {
                    numLoadingTasksActive--;
                }
            }

            protected override void AbortInternal()
            {
                //
            }

        } // class

        public GameThumbnail(GardenItem game)
            : base( (Texture2D) null)
        {
            ColorB = new ColorChangeBehavior();         
            Add(ColorB);
            Motion.Scale = GardenGamesPanel.THUMBNAIL_SCALE_UNSELECTED;
            Game = game;
            // first-time texture init            
            if (DefaultTexture == null)
            {
                DefaultTexture = BentoGame.Instance.Content.Load<Texture2D>("ball-supernova2");
            }
            
            // use default texture as long as thumbnail not loaded yet
            Texture = DefaultTexture;

            // progress bar
            progBar = new ProgressBar();
            progBar.Visible = false;
            progBar.Motion.Scale = 1f ; 
            progBar.ProgressValue = 0f;
            progBar.ProgressTarget = 0f;
            progBar.DrawInfo.LayerDepth = 0.04f;
            Add(progBar);


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
        /// Does nothing if too many downloads already ongoing.
        /// </summary>
        /// <returns>true if loading was started</returns>
        public bool LoadInBackground()
        {
            if (loaderTask == null)
            {
                if (IsNewDownloadAllowed)
                {                    
                    loaderTask = new ThreadedTask(new GameThumbnailLoadTask(this));
                    loaderTask.Start();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// makes thumbnail visible, setting EffectEnabled properly, and loading thumbnail image if not yet loaded/loading.
        /// </summary>
        public void Enable()
        {
            Visible = true;
            //EffectEnabled = (Texture != DefaultTexture);
            
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

            // adapt scale according to GameItem preset
            Motion.ScaleModifier *= ThumbnailScale;

            // debug progress bar code
            //progBar.ProgressTarget = (p.SimTime ) % 10;
            //if ((progBar.ProgressValue - progBar.ProgressTarget) > 0.12)
            //    progBar.ProgressValue = 0;
            //progBar.ProgressTarget += RandomMath.RandomBetween(-0.13f, 0.13f);
            
            // check if a new texture has been loaded in background
            if (updatedTexture != null)
            {
                // yes: replace texture by new one
                lock (updateTextureLock)
                {
                    Texture = updatedTexture;
                    updatedTexture = null;
                    //isLoaded = true;
                }
            }

            if (Game.DlAndInstallTask != null &&
                Game.ThreadedDlAndInstallTask != null &&
                !Game.ThreadedDlAndInstallTask.IsFinished())
            {
                progBar.IsDone = false;
                if (Game.DlAndInstallTask.IsDownloading())
                {
                    Game.Status = "[downloading " + String.Format("{0:0%}" , Game.DlAndInstallTask.ProgressDownload()) + "]"; 
                }
                else if (Game.DlAndInstallTask.IsInstalling())
                {
                    Game.Status = "[installing " + String.Format("{0:0%}", Game.DlAndInstallTask.ProgressInstall()) + "]"; 
                }
                progBar.ProgressTarget = (float)Game.DlAndInstallTask.Progress();
                progBar.ProgressSpeed = (float)Game.DlAndInstallTask.DownloadSpeed();
                // make bar visible if not already. Or if value needs to go down from a previous selected game's value.
                if (progBar.Visible == false || progBar.ProgressValue > progBar.ProgressTarget)
                {
                    progBar.Visible = true;
                    // instantly reset value to new one, if we just made the bar visible
                    progBar.ProgressValue = progBar.ProgressTarget;
                }
            }
            else
            {
                if (progBar.Visible && !progBar.IsDone)
                {
                    progBar.IsDone = true;
                    progBar.Add(new TimedInvisibility(3f));
                }
                //progBar.Visible = false;
                if (Game.ThreadedDlAndInstallTask != null &&
                    !Game.ThreadedDlAndInstallTask.IsSuccess() &&
                    Game.ThreadedDlAndInstallTask.IsFinished())
                {
                    Game.Status = "[download error - retry]";
                }
                else
                {
                    progBar.ProgressTarget = 1.0f;
                    //progBar.ProgressValue = 1.0f;
                    if (this.IsPlaying)
                    {
                        Game.Status = "[playing]";
                    } else 
                        Game.Status = null;
                }
            }

        }

        protected override void OnDraw(ref DrawParams p)
        {
            // use Alpha also for my progbar
            progBar.DrawInfo.Alpha = DrawInfo.Alpha;

            if (Texture != null)
            {
                Color col = DrawInfo.DrawColor;
                MySpriteBatch.Draw(Texture, DrawInfo.DrawPosition, null, col,
                       Motion.RotateAbs, DrawInfo.DrawCenter, DrawInfo.DrawScale, SpriteEffects.None, DrawInfo.LayerDepth);
            }
        }
    }
}
