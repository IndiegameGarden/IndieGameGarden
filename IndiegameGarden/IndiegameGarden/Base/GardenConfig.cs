// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetServ.Net.Json;

namespace IndiegameGarden.Base
{
    /**
     * configuration data object for the Garden application. Information loaded from JSON file.
     */
    public class GardenConfig
    {
        JSONStore cfg;

        public GardenConfig(): base()
        {
            Init();
            LoadJson();
        }

        // default values for all fields
        private void Init()
        {
            // NOTE DataPath should be set FIRST of all.
            DataPath = "..\\..\\..\\..\\.."; // for testing in Visual Studio
            //DataPath = "..\\.."; // for deployment version when embedded in games folder
            //DataPath = "."; // for deployment version

            GardenID = "34729384298374238";
            ServerMsg = "";
            ConfigFilesFolder = GetFolder("config");
            PackedFilesFolder = GetFolder("zips");
            UnpackedFilesFolder = GetFolder("games");
            ThumbnailsFolder = GetFolder("thumbs");

            StorageConfigFilename = "config.json";
            GameLibraryFilename = "gamelib.json";

            ThumbnailsServerURL = "http://indie.trancetrance.com/thumbs/";
            ConfigFilesServerURL = "http://indie.trancetrance.com/config/";
            PackedFilesServerURL = "http://indie.trancetrance.com/zips/";

        }

        private void LoadJson()
        {
            cfg = new JSONStore( ConfigFilesFolder + "\\" + StorageConfigFilename );
                
            // get values from json config
            try { GameLibraryFilename = cfg.GetString("GameLibraryFilename"); }
            catch (Exception) { ; };
            try { ThumbnailsServerURL = cfg.GetString("ThumbnailsServerURL"); }
            catch (Exception) { ; };
            try { ConfigFilesServerURL = cfg.GetString("ConfigFilesServerURL"); }
            catch (Exception) { ; };
            try { PackedFilesServerURL = cfg.GetString("PackedFilesServerURL"); }
            catch (Exception) { ; };
            try { GardenID = cfg.GetString("Garden"); }
            catch (Exception) { ; };
            try { ServerMsg = cfg.GetString("ServerMsg"); }
            catch (Exception) { ; };
        }

        // TODO document the fields below

        public string GardenID { get; set; }

        public string ServerMsg { get; set; }

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
        protected string GetFolder(string folderName)
        {
            return DataPath + "\\" + folderName;
        }

        public string GetThumbnailFilename(IndieGame g)
        {
            return g.GameID + "_v" + g.Version + ".jpg";
        }

        /// <summary>
        /// get file path to locally stored thumbnail file for game
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetThumbnailFilepath(IndieGame g)
        {
            return ThumbnailsFolder + "\\" + GetThumbnailFilename(g);
        }

        /// <summary>
        /// get url to remotely stored thumbnail file on default server ThumbnailsServerURL
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetThumbnailURL(IndieGame g)
        {
            return ThumbnailsServerURL + g.GameID + "_v" + g.Version + ".jpg"; 
        }

        /// <summary>
        /// get full file path from base directory to a game's .exe (to check it's there).
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetExeFilepath(IndieGame g)
        {
            return GetGameFolder(g) + "\\" + g.CdPath + "\\" + g.ExeFile;
        }

        /// <summary>
        /// get the folder where a game is stored (unpacked)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetGameFolder(IndieGame g)
        {
            return UnpackedFilesFolder + "\\" + g.GameID + "_v" + g.Version;
        }

        /// <summary>
        /// get path to a game's packed file (.zip, .rar)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetPackedFilepath(IndieGame g)
        {
            return PackedFilesFolder + "\\" + GetPackedFileName(g);
        }

        /// <summary>
        /// the name of the packed file (eg .zip or .rar) once it is downloaded. May differ
        /// from the name of the archive as stored on the web which is included in PackedFileURL.
        /// </summary>
        public string GetPackedFileName(IndieGame g)
        {
            return g.GameID + "_v" + g.Version + "." + ExtractFileExtension(g.PackedFileURL);
        }


        // extract an extension e.g. "zip" from a partial or full URL e.g. http://server/test/name.zip 
        // <returns>extension after last dot, or default "zip" if no dot found in 'urlDl'.</returns>
        private string ExtractFileExtension(string url)
        {
            int i = url.LastIndexOf('.');
            if (i == -1)
                return "zip";
            return url.Substring(i + 1);
        }


    }
}
