// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
        /// value is constant for a build! update this manually for new version builds. Assembly version number
        /// will be "1.value.x"
        /// 1 = ALPHA-1
        /// 2 = ALPHA-2
        /// 3 = ALPHA-3
        /// 4 = BETA-4
        /// 5 = BETA-5
        /// 6 = BETA-6
        /// 7 = Indiegame Garden 7
        /// 8 = GWG
        /// 9 = Bento
        /// etc.
        /// </summary>
        public const int IGG_CLIENT_VERSION = 9;

        /// <summary>
        /// specifies relative folder from which to copy files to the DataPath folder during init
        /// </summary>
        public const string COPY_FILES_SRC_PATH = "."; 
        
        protected static GardenConfig instance = null;

        public GardenConfig()
        {
            InitDefaults();                        
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

        /// <summary>
        /// First init with only default values put into field. May be overwritten later by InitFromJSON()
        /// </summary>
        protected void InitDefaults()
        {
            // NOTE DataPath should be set FIRST of all.
            //DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IndiegameGarden");
            DataPath = "../../../../..";
            DataPath = Path.GetFullPath(DataPath);
            // For bundle .exe files, save in same folder as Indiegame Garden exe (nice for collecting them)
            BundleDataPath = Path.GetFullPath(".");

            ConfigFilesFolder = GetFolder("config");
            PackedFilesFolder = GetFolder("zips");
            UnpackedFilesFolder = GetFolder("games");
            ThumbnailsFolder = GetFolder("thumbs");

            ConfigFilename = "gwg-config.json";
            GameLibraryFilename = "gamelib.json";
            GameLibraryFilenameBin = "gamelib.bin";

            ThumbnailsServerURL = "http://indie.indiegamegarden.com/thumbs/";
            ConfigFilesServerURL = "http://indie.indiegamegarden.com/gwg_gamelib_fmt4/"; 
            PackedFilesServerURL = "http://indie.indiegamegarden.com/zips/";
            BundleFilesServerURL = "http://www.indiegamegarden.com/";

            jsonFilePath = Path.Combine(ConfigFilesFolder, ConfigFilename);

        }

        /// <summary>
        /// verify that the IGG data path is valid. If needed, create directories to get a valid
        /// data path.
        /// </summary>
        /// <returns>true if proper datapath existence was verified, false if not (and could not be created)</returns>
        public bool VerifyDataPath()
        {
            // verify and/or create all individual folders
            if (!VerifyFolder(".")) return false;
            if (!VerifyFolder("config")) return false;
            if (!VerifyFolder("thumbs")) return false;
            if (!VerifyFolder("zips")) return false;
            if (!VerifyFolder("games")) return false;

            // copy over some initial files if needed (only if dir is empty)           
            //if (!CopyFiles("config")) return false;
            if (!CopyFiles("thumbs")) return false;
            //if (!CopyFiles("zips")) return false;

            return true;
        }

        /// <summary>
        /// helper method that copies all files from given folderName from the local program install folder location
        /// to the external DataPath folder location.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns>true if copy succeeded or not needed anymore or source folder not present,
        /// false when copy failed</returns>
        protected bool CopyFiles(string folderName)
        {
            try
            {
                string src = Path.GetFullPath(Path.Combine(COPY_FILES_SRC_PATH, folderName));
                if (!Directory.Exists(src))
                    return true;
                string dest = Path.GetFullPath(Path.Combine(DataPath, folderName));
                // check if source, dest not identical
                if (!src.Equals(dest))
                {
                    string[] files = Directory.GetFiles(src);
                    foreach (string filepath in files)
                    {
                        string filename = Path.GetFileName(filepath);
                        string destFile = Path.Combine(dest, filename);
                        if (!File.Exists(destFile))
                            File.Copy(filepath, destFile, false);  // set to NOT overwrite any existing file of same name
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// helper method to verify a single folder used by VerifyDataPath()
        /// </summary>
        /// <param name="folderName">a simple folder name no path e.g. "config" or "." within the DataPath</param>
        /// <returns>true if folder exists or could be created, false if not exists and could not be created</returns>
        protected bool VerifyFolder(string folderName)
        {
            string p = GetFolder(folderName);
            if (!Directory.Exists(p))
            {
                try
                {
                    Directory.CreateDirectory(p);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if (Directory.Exists(p))
                return true;
            else
                return false;
        }

        /// <summary>
        /// check whether this config is valid e.g. by checking for certain mandatory properties.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// a folder path, abs or rel, pointing to the location where all below defined data folders are present
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// a folder path, abs or rel, pointing to the bundle location where all .exe file downloads go to
        /// </summary>
        public string BundleDataPath { get; set; }

        /// <summary>
        /// abs folder path where config files are stored
        /// </summary>
        public string ConfigFilesFolder { get; set; }

        /// <summary>
        /// abs folder path name where packed files (zip, rar, etc) of games are stored
        /// </summary>
        public string PackedFilesFolder { get; set; }

        /// <summary>
        /// abs folder path where unpacked folders of games reside
        /// </summary>
        public string UnpackedFilesFolder { get; set; }

        /// <summary>
        /// abs folder path where thumbnails are stored
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

        /// <summary>
        /// name of the binary version of game library file
        /// </summary>
        public string GameLibraryFilenameBin { get; set; }

        /// <summary>
        /// returns the version of current running client
        /// </summary>
        public int ClientVersion { 
            get {
                return IGG_CLIENT_VERSION;
            }
        }

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
        public string BundleFilesServerURL { get; set; }

        /// <summary>
        /// prepend the DataPath location to a simple folder name, in order to locate the folder properly
        /// </summary>
        /// <param name="folderName">any folder name e.g. ConfigFilesFolder or PackedFilesFolder</param>
        /// <returns>folderName prepended with the DataPath</returns>
        protected string GetFolder(string folderName)
        {
            return Path.Combine(DataPath , folderName);
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

    }
}
