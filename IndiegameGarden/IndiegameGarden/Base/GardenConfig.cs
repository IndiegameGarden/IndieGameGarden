// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NetServ.Net.Json;

namespace IndiegameGarden.Base
{
    /**
     * configuration data object for the Garden application. Information loaded from JSON file.
     */
    public class GardenConfig: JSONStore
    {
        /// <summary>
        /// checking config file integrity (somewhat)
        /// </summary>
        public const string CONFIG_MAGIC_VALUE = "f20fj239jf0a9w";
        /// <summary>
        /// garden ID default when no ID assigned yet by server (this assignment is optional)
        /// </summary>
        public const string DEFAULT_GARDEN_ID = "42";
        /// <summary>
        /// auth for server communication
        /// </summary>
        public const string IGG_CLIENT_AUTH_KEY = "sreqZRVmzJVqdsrKuCwJTnumI";
        /// <summary>
        /// value is constant for a build! update this manually for new version builds.
        /// 1 = ALPHA-1
        /// 2 = ALPHA-2
        /// 3 = ALPHA-3
        /// </summary>
        public const int    IGG_CLIENT_VERSION = 5;
        /// <summary>
        /// update for a new build -> quick bootstrap to load a known version of gamelib. This version number
        /// starts again at 1 each time a new gamelib format ("fmt") version is released.
        /// For example "fmt3" in the source refers to gamelib format version 3.
        /// </summary>
        public const int    KNOWN_GAMELIB_VERSION = 2;

        public const string DATA_PATH_DEBUG = "..\\..\\..\\..\\.."; // for testing in Visual Studio
        public const string DATA_PATH = "..\\.."; // for deployment version when embedded in games folder
        
        public const string DEFAULT_CONFIG_FILEPATH = "config\\gamelib-config.json";

        bool hasLoadedFromFileOk = false;

        protected static GardenConfig instance = null;

        public GardenConfig()
        {
#if DEBUG
            jsonFilePath = Path.Combine(DATA_PATH_DEBUG, DEFAULT_CONFIG_FILEPATH);
#else
            jsonFilePath = Path.Combine(DATA_PATH, DEFAULT_CONFIG_FILEPATH);
#endif
            hasLoadedFromFileOk = true;
            try
            {
                LoadJson();
            }
            catch (Exception )
            {
                //
                hasLoadedFromFileOk = false;
            }
            Init();
        }

        public static GardenConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = new GardenConfig();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        // default values for all fields
        protected void Init()
        {
            // NOTE DataPath should be set FIRST of all.
            // check whether in Visual studio debugging mode
#if DEBUG
            DataPath = Path.GetFullPath(DATA_PATH_DEBUG);
#else
            DataPath = Path.GetFullPath(DATA_PATH);
#endif

            GardenID = DEFAULT_GARDEN_ID;
            ServerMsg = "Enjoy your garden!\nBut watch out for the weeds.";
            ConfigFilesFolder = GetFolder("config");
            PackedFilesFolder = GetFolder("zips");
            UnpackedFilesFolder = GetFolder("games");
#if DEBUG
            ThumbnailsFolder = GetFolder("thumbs-test");
#else
            ThumbnailsFolder = GetFolder("thumbs");
#endif

            ConfigFilename = "gamelib-config.json";
            GameLibraryFilename = "gamelib.json";
            GameLibraryFilenameBin = "gamelib.bin";
            NewestGameLibraryVersion = KNOWN_GAMELIB_VERSION;
            Magic = CONFIG_MAGIC_VALUE;

            ThumbnailsServerURL = "https://github.com/trancetrance/IndieGameGarden/raw/master/thumbs/";
            ConfigFilesServerURL = "https://github.com/trancetrance/IndieGameGarden/raw/master/config/gamelib_fmt3/";
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
            try { NewestGameLibraryVersion = (int)GetValue("GameLibVer"); }
            catch (Exception) { ; };
            try { NewestClientVersion = (int)GetValue("ClientVer"); }
            catch (Exception) { ; };
            try { Magic = GetString("Magic"); }
            catch (Exception) { ; };

        }

        /// <summary>
        /// check whether this config is valid e.g. by checking for certain mandatory properties.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (!Magic.Equals(GardenConfig.CONFIG_MAGIC_VALUE))
                return false;
            else
                return true;
        }

        public override void Reload()
        {
            try
            {
                base.Reload();
                hasLoadedFromFileOk = true;
            }
            catch (Exception)
            {
                hasLoadedFromFileOk = false;
            }
            Init();
        }

        /// <summary>
        /// check whether a config has been loaded successfully from a file
        /// </summary>
        public bool HasLoadedFromFileOk
        {
            get
            {
                return hasLoadedFromFileOk;
            }
        }

        /// <summary>
        /// the unique ID of this garden (may be changed by config server, may be unused as well)
        /// </summary>
        public string GardenID { get; set; }

        /// <summary>
        /// special message from config server e.g. showing issues or news
        /// </summary>
        public string ServerMsg { get; set; }

        /// <summary>
        /// a folder path, abs or rel, pointing to the location where all below defined data folders are present
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// folder name where config files are stored
        /// </summary>
        public string ConfigFilesFolder { get; set; }

        /// <summary>
        /// folder name where packed files (zip, rar, etc) of games are stored
        /// </summary>
        public string PackedFilesFolder { get; set; }

        /// <summary>
        /// folder name where unpacked folders of games reside
        /// </summary>
        public string UnpackedFilesFolder { get; set; }

        /// <summary>
        /// folder name where thumbnails are stored
        /// </summary>
        public string ThumbnailsFolder { get; set; } 

        /// <summary>
        /// name of the configuration file (may be updated by server for some reason)
        /// </summary>
        public string ConfigFilename { get; set; }

        /// <summary>
        /// name of the game library JSON file
        /// </summary>
        public string GameLibraryFilename { get; set; }

        public string GameLibraryFilenameBin { get; set; }

        /// <summary>
        /// version of the newest game library currently available (obtained from config server)
        /// </summary>
        public int NewestGameLibraryVersion { get; set; }

        /// <summary>
        /// magic value to check config integrity (loosely)
        /// </summary>
        public string Magic { get; set; }

        /// <summary>
        /// returns the version of current running client
        /// </summary>
        public int ClientVersion { 
            get {
                return IGG_CLIENT_VERSION;
            }
        }

        /// <summary>
        /// returns newest available known client version (obtained from config server)
        /// </summary>
        public int NewestClientVersion { get; set; }

        /// <summary>
        /// url of the thumbnails server (incl path to thumbnails folder if any)
        /// </summary>
        public string ThumbnailsServerURL { get; set; }

        /// <summary>
        /// url of the config server (incl path, excl config file name)
        /// </summary>
        public string ConfigFilesServerURL { get; set; }

        /// <summary>
        /// url of a mirror server storing packed files (incl path) for all or most games
        /// </summary>
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

        /// <summary>
        /// get file path to locally stored thumbnail file for game
        /// </summary>
        /// <param name="g"></param>
        /// <returns>by default a .png thumbnail for a game (whether file exists or not)
        /// but if a .jpg thumbnail exists, it is chosen.</returns>
        public string GetThumbnailFilepath(GardenItem g)
        {
            string p1 = Path.Combine(ThumbnailsFolder , g.GameIDwithVersion);
            string p2 = p1;
            p1 += ".jpg";
            p2 += ".png";
            if (File.Exists(p2))
                return p2;
            else if (File.Exists(p1))
                return p1;
            else
                return p2;
        }
               
        /// <summary>
        /// get url to remotely stored thumbnail file on default server ThumbnailsServerURL
        /// </summary>
        /// <returns></returns>
        public string GetThumbnailURL(string thumbnailFilename)
        {
            return ThumbnailsServerURL + thumbnailFilename; 
        }

        /// <summary>
        /// get path to a game's packed file (.zip, .rar)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string GetPackedFilepath(GardenItem g)
        {
            return PackedFilesFolder + "\\" + GetPackedFileName(g);
        }

        /// <summary>
        /// the name of the packed file (eg .zip or .rar) once it is downloaded. May differ
        /// from the name of the archive as stored on the web which is included in PackedFileURL.
        /// </summary>
        public string GetPackedFileName(GardenItem g)
        {
            return g.GameIDwithVersion + "." + g.PackedFileExtension;
        }



    }
}
