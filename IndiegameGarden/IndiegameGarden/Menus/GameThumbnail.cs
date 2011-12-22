// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
    public class GameThumbnail: EffectSpritelet
    {
        string gameID;
        string thumbnailFilename ;
        string thumbnailUrl ;
        ThumbnailDownloader downl;
        Texture2D updatedTexture;
        Object updateTextureLock = new Object();
        bool isLoaded = false;
        float intensity = 1.0f;

        public static Texture2D DefaultTexture;

        public GameThumbnail(string gameID)
            : base(DefaultTexture,"GameThumbnail")
        {
            this.gameID = gameID;
            // TODO methods to construct paths!? incl .png
            this.thumbnailFilename = GardenMain.Instance.storageConfig.CreateThumbnailFilepath(gameID,false); 
            this.thumbnailUrl = GardenMain.Instance.storageConfig.CreateThumbnailURL(gameID,false);
            Thread t = new Thread(new ThreadStart(StartLoadingProcess));
            t.Start();
        }

        public float Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
                DrawColor = new Color(intensity, intensity, intensity, DrawColor.A);
            }

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
                downl = new ThumbnailDownloader(gameID);
                DownloadManager.Instance.DownloadEnded += new EventHandler<DownloaderEventArgs>(HandleDownloadEndedEvent);
                downl.Start();
            }
        }

        protected void LoadTextureFromFile()
        {
            FileStream fs = new FileStream(thumbnailFilename, FileMode.Open);
            Texture2D tex = Texture2D.FromStream(GardenMain.Instance.GraphicsDevice, fs);
            lock (updateTextureLock)
            {
                updatedTexture = tex;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
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
                lock (updateTextureLock)
                {
                    Texture = updatedTexture;
                    updatedTexture = null;
                    isLoaded = true;
                }
            }
        }

        public virtual void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e)
        {
            LoadTextureFromFile();
        }

    }
}
