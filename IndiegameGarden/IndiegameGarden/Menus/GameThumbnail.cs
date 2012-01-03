// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Store;
using IndiegameGarden.Download;
using MyDownloader.Core;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A thumbnail showing a single game, with auto-loading and downloading of image data.
    /// </summary>
    public class GameThumbnail: MovingEffectSpritelet
    {
        string gameID;
        string thumbnailFilename ;
        string thumbnailUrl ;
        ThumbnailDownloader downl;
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();
        bool isLoaded = false;

        // a default texture to use if no thumbnail has been loaded yet
        static Texture2D DefaultTexture;

        public GameThumbnail(string gameID)
            : base(DefaultTexture,"GameThumbnail")
        {
            Scale = GardenGamesPanel.THUMBNAIL_SCALE_UNSELECTED;
            this.gameID = gameID;
            // TODO methods to construct paths!? incl .png
            this.thumbnailFilename = GardenGame.Instance.Config.CreateThumbnailFilepath(gameID,false); 
            this.thumbnailUrl = GardenGame.Instance.Config.CreateThumbnailURL(gameID,false);
            Thread t = new Thread(new ThreadStart(StartLoadingProcess));
            t.Start();
        }

        /// <summary>
        /// loads image from either file or by download
        /// </summary>
        void StartLoadingProcess()
        {
            if (File.Exists(thumbnailFilename))
            {
                LoadTextureFromFile();
            }
            else
            {
                // start a download
                downl = new ThumbnailDownloader(gameID, new OnDownloadOK(OnThumbnailDownloaded) );
                downl.Start();
            }
        }

        // (re) loads texture from a file and puts in updatedTexture var
        protected void LoadTextureFromFile()
        {
            FileStream fs = new FileStream(thumbnailFilename, FileMode.Open);
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

        /// <summary>
        /// called via a Delegate when thumbnail has been downloaded ok.
        /// </summary>
        public void OnThumbnailDownloaded(string gameID)
        {
            LoadTextureFromFile();
        }

    }
}
