// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Base
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
            DataPath = "..\\..\\..\\..\\..";
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
                cfg = new JSONStore( GetFolder(ConfigFilesFolder) + "\\" + StorageConfigFilename );
            }
            catch(Exception)
            {
                ;
                // TODO
            }
        }

        // TODO document the fields below

        /// <summary>
        /// a folder path, abs or rel, pointing to the location where all below defined data folders are present
        /// </summary>
        public string DataPath { get; set; }

        public string ConfigFilesFolder { get; set; }

        public string PackedFilesFolder { get; set; }

        public string UnpackedFilesFolder { get; set; }

        public string ThumbnailsFolder { get; set; } 

        public string StorageConfigFilename { get; set; }

        public string GameLibraryFilename { get; set; }

        public string ThumbnailsServerURL { get; set; }

        public string ConfigFilesServerURL { get; set; }

        public string PackedFilesServerURL { get; set; }

        /// <summary>
        /// prepend the DataPath location to a simple folder name, in order to locate the folder properly
        /// </summary>
        /// <param name="folderName">any folder name e.g. ConfigFilesFolder or PackedFilesFolder</param>
        /// <returns>folderName prepended with the DataPath</returns>
        public string GetFolder(string folderName)
        {
            return DataPath + "\\" + folderName;
        }

        public string GetThumbnailFilepath(string gameID, bool alternativeFile)
        {
            return GetFolder(ThumbnailsFolder) + "\\" + gameID + (alternativeFile ? ".png" : ".jpg");
        }

        public string GetThumbnailURL(string gameID, bool alternativeFile)
        {
            return ThumbnailsServerURL + "/" + gameID + (alternativeFile ? ".png" : ".jpg");
        }

        public string GetExeFilepath(string gameID, int version, string cdPath, string exeFilename)
        {
            return GetGameFolder(gameID,version) + "\\" + cdPath + "\\" + exeFilename;
        }

        public string GetGameFolder(string gameID, int version)
        {
            return GetFolder(UnpackedFilesFolder) + "\\" + gameID + "_v" + version;
        }

        public string GetPackedFilepath(string packedGameFile)
        {
            return GetFolder(PackedFilesFolder) + "\\" + packedGameFile;
        }
    }
}
