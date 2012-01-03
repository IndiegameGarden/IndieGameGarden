// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Store
{
    /**
     * configuration data object for the Garden application. Information loaded from JSON.
     */
    public class GardenConfig
    {
        JSONStore cfg;

        public GardenConfig(): base()
        {
            Init();
            LoadJson();
        }

        private void Init()
        {
            ConfigFilesFolder = "Config";
            PackedFilesFolder = "zips";
            UnpackedFilesFolder = "games";
            ThumbnailsFolder = "thumbs";

            StorageConfigFilename = "Config.json";
            GameLibraryFilename = "gamelib.json";

            ThumbnailsServerURL = "http://indie.trancetrance.com/thumbs/";
            ConfigFilesServerURL = "http://indie.trancetrance.com/Config/";
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

        // TODO document the fields below
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

        public string CreatePackedFilepath(string packedGameFile)
        {
            return PackedFilesFolder + "\\" + packedGameFile;
        }
    }
}
