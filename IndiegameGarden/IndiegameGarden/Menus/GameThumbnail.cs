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
    public class GameThumbnail: MovingEffectSpritelet
    {
        /// <summary>
        /// ID of game for which this thumbnail is
        /// </summary>
        public string GameID;

        /// <summary>
        /// actual/intended filename of thumbnail file (the file may or may not exist)
        /// </summary>
        public string ThumbnailFilename;

        /// <summary>
        /// URL where thumbnail file may be retrieved
        /// </summary>
        public string ThumbnailUrl;

        //ThumbnailDownloader downl;
        ITask loaderTask;
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();
        bool isLoaded = false;

        // a default texture to use if no thumbnail has been loaded yet
        static Texture2D DefaultTexture;

        /**
         * internal Task to load a thumbnail from disk, or download it first if not available.
         * To be called in a separate thread e.g. ThreadedTask.
         */
        class GameThumbnailLoadTask : Task
        {
            // my parent - where to load for/to
            GameThumbnail thumbnail;

            public GameThumbnailLoadTask(GameThumbnail th)
            {
                thumbnail = th;
            }

            /// <summary>
            /// loads image from either file if it exists, or else by download
            /// </summary>
            public override void Start()
            {
                if (File.Exists(thumbnail.ThumbnailFilename))
                {
                    thumbnail.LoadTextureFromFile();
                }
                else
                {
                    // start a download
                    ThumbnailDownloader downl = new ThumbnailDownloader(thumbnail.GameID);
                    downl.Start();
                    if (File.Exists(thumbnail.ThumbnailFilename))
                    {
                        thumbnail.LoadTextureFromFile();
                    }
                }
            }
        } // class

        public GameThumbnail(string gameID)
            : base(DefaultTexture,"GameThumbnail")
        {
            Scale = GardenGamesPanel.THUMBNAIL_SCALE_UNSELECTED;
            this.GameID = gameID;
            // TODO methods to construct paths!? incl .png
            this.ThumbnailFilename = GardenGame.Instance.Config.GetThumbnailFilepath(gameID,false); 
            this.ThumbnailUrl = GardenGame.Instance.Config.GetThumbnailURL(gameID,false);
            loaderTask = new ThreadedTask(new GameThumbnailLoadTask(this));
            loaderTask.Start();
        }

        protected override void OnInit()
        {
            base.OnInit();

            // first-time texture init
            if (DefaultTexture == null)
            {
                DefaultTexture = GardenGame.Instance.Content.Load<Texture2D>("ball-supernova2");
            }
        }

        // (re) loads texture from a file and puts in updatedTexture var
        protected void LoadTextureFromFile()
        {
            FileStream fs = new FileStream(ThumbnailFilename, FileMode.Open);
            Texture2D tex = Texture2D.FromStream(GardenGame.Instance.GraphicsDevice, fs);
            lock (updateTextureLock)
            {
                updatedTexture = tex;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            ScaleModifier *= 2.0f;

            // animation of loading
            if (!isLoaded)
            {
                RotateModifier += p.simTime;
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
        }
    }
}
