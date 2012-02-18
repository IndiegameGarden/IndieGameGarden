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
    public class GardenConfig: JSONStore
    {
        public const string CONFIG_MAGIC_VALUE = "f20fj239jf0a9w";
        public const string DEFAULT_GARDEN_ID = "44729384298378";
        public const string IGG_CLIENT_AUTH_KEY = "sreqZRVmzJVqdsrKuCwJTnumI";

        public const string DATA_PATH = "..\\..\\..\\..\\.."; // for testing in Visual Studio
        //public const string DATA_PATH = "..\\.."; // for deployment version when embedded in games folder
        //public const string DATA_PATH =  "."; // for deployment version
        public const string DEFAULT_CONFIG_FILEPATH = DATA_PATH + "\\config\\config.json";

        bool hasLoadedFromFileOk = false;

        public GardenConfig()
        {
            jsonFilePath = DEFAULT_CONFIG_FILEPATH;
            hasLoadedFromFileOk = true;
            try
            {
                LoadJson();
            }
            catch (Exception ex)
            {
                //
                hasLoadedFromFileOk = false;
            }
            Init();
        }

        // default values for all fields
        protected void Init()
        {
            // NOTE DataPath should be set FIRST of all.
            DataPath = DATA_PATH;

            GardenID = DEFAULT_GARDEN_ID;
            ServerMsg = "";
            ConfigFilesFolder = GetFolder("config");
            PackedFilesFolder = GetFolder("zips");
            UnpackedFilesFolder = GetFolder("games");
            ThumbnailsFolder = GetFolder("thumbs");

            ConfigFilename = "config.json";
            GameLibraryFilename = "gamelib.json";

            ThumbnailsServerURL = "http://indie.indiegamegarden.com/thumbs/";
            ConfigFilesServerURL = "http://indieget.appspot.com/igg/";
            PackedFilesServerURL = "http://indie.indiegamegarden.com/zips/";

            jsonFilePath = ConfigFilesFolder + "\\" + ConfigFilename;

            // get values from json config
            try { GameLibraryFilename = GetString("GameLibraryFilename"); }
            catch (Exception) { ; };
            try { ThumbnailsServerURL = GetString("ThumbnailsServerURL"); }
            catch (Exception) { ; };
            try { ConfigFilesServerURL = GetString("ConfigFilesServerURL"); }
            catch (Exception) { ; };
            try { PackedFilesServerURL = GetString("PackedFilesServerURL"); }
            catch (Exception) { ; };
            try { GardenID = GetString("Garden"); }
            catch (Exception) { ; };
            try { ServerMsg = GetString("ServerMsg"); }
            catch (Exception) { ; };

        }

        /// <summary>
        /// check whether this config is valid e.g. by checking for certain mandatory properties.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (!HasKey("Magic") ||
                !GetString("Magic").Equals(GardenConfig.CONFIG_MAGIC_VALUE))
            {
                return false;
            }
            return true;
        }

        public override void Reload()
        {
            try
            {
                base.Reload();
                hasLoadedFromFileOk = true;
            }
            catch (Exception ex)
            {
                hasLoadedFromFileOk = false;
            }
            Init();
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

        public string ConfigFilename { get; set; }

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
            if (g.Version == 1)
                return g.GameID + ".jpg";
            else
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
            if (g.Version ==1 )
                return ThumbnailsServerURL + g.GameID + ".jpg"; 
            else
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
            if(g.Version == 1)
                return UnpackedFilesFolder + "\\" + g.GameID ;
            else
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
            if(g.Version == 1)
                return g.GameID + "." + ExtractFileExtension(g.PackedFileURL);
            else
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
