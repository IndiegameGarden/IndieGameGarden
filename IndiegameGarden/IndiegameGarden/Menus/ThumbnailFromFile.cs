// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /*
    public class ThumbnailFromFile: Spritelet
    {
        string ThumbnailFilename = null;

        protected ThumbnailFromFile(string fn): base()
        {
            this.ThumbnailFilename = fn;            
        }

        public static ThumbnailFromFile CreateFromGameID(string GameID)
        {
            string gameDirPath = GardenConfig.Instance.ThumbnailsFolder + "\\" + GameID + ".png" ;
            return new ThumbnailFromFile(gameDirPath);
        }

        public static ThumbnailFromFile CreateFromFile(string filePath)
        {
            return new ThumbnailFromFile(filePath);
        }

        public void EnsureTextureLoaded()
        {
            if (Texture == null)
                LoadTextureFromFile();
        }

        public void LoadTextureFromFile()
        {
            FileStream fs = new FileStream(ThumbnailFilename, FileMode.Open);
            Texture = Texture2D.FromStream(Screen.graphicsDevice, fs);
        }

        protected override void OnInit()
        {
            base.OnInit();

            LoadTextureFromFile();
        }
    }
     */
}
