// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Store
{
    public class StorageConfig
    {
        JSONStore cfg;

        public StorageConfig(): base()
        {
            Init();
            LoadJson();
        }

        private void Init()
        {
            ConfigFilesFolder = "config";
            PackedFilesFolder = "zips";
            UnpackedFilesFolder = "games";
            ThumbnailsFolder = "thumbs";

            StorageConfigFilename = "config.json";
            GameLibraryFilename = "gamelib.json";

            ThumbnailsServerURL = "http://indie.trancetrance.com/thumbs/";
            ConfigFilesServerURL = "http://indie.trancetrance.com/config/";
            PackedFilesServerURL = "http://indie.trancetrance.com/zips/";

        }

        private void LoadJson()
        {
            try{
                cfg = new JSONStore( StorageConfigFilename );
                // TODO parse it
            }
            catch(Exception)
            {
                ;
            }
        }

        public string ConfigFilesFolder { get; set; }

        public string PackedFilesFolder { get; set; }

        public string UnpackedFilesFolder { get; set; }

        public string ThumbnailsFolder { get; set; } 


        public string StorageConfigFilename { get; set; }

        public string GameLibraryFilename { get; set; }


        public string ThumbnailsServerURL { get; set; }

        public string ConfigFilesServerURL { get; set; }

        public string PackedFilesServerURL { get; set; }

        public string CreateThumbnailFilepath(string gameID, bool alternativeFile)
        {
            return ThumbnailsFolder + "\\" + gameID + (alternativeFile ? ".png" : ".jpg");
        }

        public string CreateThumbnailURL(string gameID, bool alternativeFile)
        {
            return ThumbnailsServerURL + "/" + gameID + (alternativeFile ? ".png" : ".jpg");
        }

        public string CreateExeFilepath(string gameID, string exeFilename)
        {
            return UnpackedFilesFolder + "\\" + gameID + "\\" + exeFilename;
        }

        public string CreateGameFolder(string gameID)
        {
            return UnpackedFilesFolder + "\\" + gameID;
        }
    }
}
