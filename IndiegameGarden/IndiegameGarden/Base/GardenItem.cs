// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using TTengine.Core;
using TTengine.Util;
using IndiegameGarden.Menus;
using IndiegameGarden.Install;
using NetServ.Net.Json;
using ProtoBuf;

namespace IndiegameGarden.Base
{
    /**
     * <summary>represents all data and status of a game that a user can select, download and start/play</summary>
     */
    [ProtoContract]
    public class GardenItem: IDisposable
    {
        /// <summary>
        /// internally used string ID for game, no whitespace allowed, only alphanumeric and 
        /// _ - special characters allowed.
        /// </summary>
        [ProtoMember(1)]
        public string GameID = "";

        /// <summary>
        /// default visibility of item for the user; 1 (yes) or 0 (no)
        /// </summary>
        [ProtoMember(2)]
        public byte VisibilityLabel = 1;

        /// <summary>
        /// Name of game
        /// </summary>
        [ProtoMember(3)]
        public string Name = "";

        /// <summary>
        /// short game description to show on screen
        /// </summary>
        [ProtoMember(4)]
        public string description = "";

        /// <summary>
        /// some hints for the player e.g. what the control keys are.
        /// </summary>
        [ProtoMember(5)]
        public string HelpText = "";

        [ProtoMember(6)]
        protected string packedFileURL = "";

        [ProtoMember(7)]
        protected List<string> packedFileMirrors = new List<string>();

        /// <summary>
        /// URL (optionally without the http:// or www. in front) to game developer's website
        /// </summary>
        [ProtoMember(8)]
        public string DeveloperWebsiteURL = "";

        /// <summary>
        /// name of .exe file or .bat to launch to start game
        /// </summary>
        [ProtoMember(9)]
        public string ExeFile = "";

        /// <summary>
        /// directory gameDirPath that OS has to 'change directory' to, before launching the game
        /// </summary>
        [ProtoMember(10)]
        public string CdPath = "";

        /// <summary>
        /// Latest version of the game packed file which is available
        /// </summary>
        [ProtoMember(11)]        
        public int Version = 1;

        /// <summary>
        /// scaling factor of game icon when displayed
        /// </summary>
        [ProtoMember(12)]
        public float ScaleIcon = 1f;

        /// <summary>
        /// where in 2D coordinates this game is positioned. Zero means non-specified.
        /// </summary>
        [ProtoMember(13)]
        public int PositionX = 0;
        [ProtoMember(14)]        
        public int PositionY = 0;

        /// <summary>
        /// in case a 2D Position is not given, this specifies a wished position delta of game w.r.t. previous game in the library.
        /// </summary>
        [ProtoMember(15)]        
        public int PositionDeltaX = 0;
        [ProtoMember(16)]        
        public int PositionDeltaY = 0;

        [ProtoMember(17)]
        protected string thumbnailURL = "";

        /// <summary>
        /// Vector2 version (copy) of PositionX/PositionY
        /// </summary>
        public Vector2 PositionXY
        {
            get
            {
                return new Vector2((float)PositionX, (float)PositionY);
            }
        }

        /// <summary>
        /// a set of mirrors for PackedFileURL
        /// </summary>
        public string[] PackedFileMirrors
        {
            get
            {
                return packedFileMirrors.ToArray();
            }
        }

        /// <summary>
        /// where can the packed file (.zip, .rar etc.) be downloaded from.
        /// Optionally without http:// in front. Optionally may set it without
        /// any server URL, then it is prepended with default file server URL
        /// automagically.
        /// </summary>
        public string PackedFileURL
        {
            get
            {
                if (IsBundleItem)
                    return GardenConfig.Instance.BundleFilesServerURL + ExeFile;
                if (!packedFileURL.Contains("/"))
                {
                    if (packedFileURL.Length == 0)
                        return GardenConfig.Instance.PackedFilesServerURL + GameIDwithVersion + "." + PackedFileExtension;
                    else
                        return GardenConfig.Instance.PackedFilesServerURL + packedFileURL;
                }
                else
                    return packedFileURL;
            }
            set
            {
                packedFileURL = value;
            }
        }

        /// <summary>
        /// the name of the packed file (eg .zip or .rar or .exe) once it is downloaded. May differ
        /// from the name of the archive as stored on the web which is included in PackedFileURL.
        /// </summary>
        public string PackedFileName{
            get
            {
                // check for .exe which are stored locally in DataPath
                if (IsBundleItem)
                    return ExeFile;
                else
                    return GameIDwithVersion + "." + PackedFileExtension;
            }
        }

        /// <summary>
        /// get path to a game's packed file (.zip, .rar)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public string PackedFilePath
        {
            get
            {
                return Path.Combine(PackedFileFolder, PackedFileName );
            }
        }

        public string PackedFileFolder
        {
            get
            {
                if (IsBundleItem)
                    return GardenConfig.Instance.BundleDataPath;
                else
                    return GardenConfig.Instance.PackedFilesFolder;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                lineCount = TTutil.LineCount(description);
            }
        }


        /// <summary>
        /// returns a human-readable string name for this item
        /// </summary>
        public string ItemName
        {
            get
            {
                if (IsMusic) return "music";
                if (IsSystemPackage) return "item";
                return "game";
            }
        }

        protected int lineCount = -1;

        public int DescriptionLineCount
        {
            get
            {
                if (lineCount < 0)
                    lineCount = TTutil.LineCount(description);
                return lineCount;
            }
        }

        public string GameIDwithVersion
        {
            get
            {
                if (Version == 1)
                    return GameID;
                else
                    return GameID + "_v" + Version;
            }
        }

        /// <summary>
        /// check whether a 2D coordinate position for game is explicitly given, or not
        /// </summary>
        public bool IsPositionGiven
        {
            get
            {
                return (PositionX != 0) || (PositionY != 0);
            }
        }

        /// <summary>
        /// check whether a 2D coordinate position delta for game is explicitly given, or not
        /// </summary>
        public bool IsPositionDeltaGiven
        {
            get
            {
                return (PositionDeltaX != 0) || (PositionDeltaY != 0);
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(PositionX,PositionY);
            }
            set
            {
                PositionX = (int) value.X;
                PositionY = (int) value.Y;
            }
        }

        public Vector2 PositionDelta
        {
            get
            {
                return new Vector2(PositionDeltaX, PositionDeltaY);
            }
            set
            {
                PositionDeltaX = (int)value.X;
                PositionDeltaY = (int)value.Y;
            }
        }

        /// <summary>
        /// Optionally a download/install task ongoing for this game
        /// </summary>
        public GameDownloadAndInstallTask DlAndInstallTask = null;

        /// <summary>
        /// A ThreadedTask that wraps DlAndInstallTask, so that download/install happens in own thread.
        /// </summary>
        public ThreadedTask ThreadedDlAndInstallTask = null;

        //-- private vars
        private bool isInstalled = false;
        private bool refreshInstallationStatusNeeded = true;

        protected GardenItem()
        {            
        }

        public static GardenItem ConstructGameLibItem(int version)
        {
            GardenItem g = new GardenItem();
            g.Version = version;
            g.GameID = "gwg_gamelib_fmt4"; // TODO to config constants?
            g.ExeFile = "gamelib.bin";
            g.PackedFileURL = GardenConfig.Instance.ConfigFilesServerURL + "gamelib.zip";
            return g;
        }

        public void Dispose()
        {
            if (ThreadedDlAndInstallTask != null)
            {
                ThreadedDlAndInstallTask.Abort();
            }
        }

        /// <summary>
        /// get full file path from base directory to a game's .exe (to check it's there).
        /// </summary>
        /// <returns></returns>
        public string ExeFilepath
        {
            get {
                if (IsBundleItem)
                    return Path.Combine(GameFolder, ExeFile);
                else
                    return GameFolder + "\\" + CdPath + "\\" + ExeFile;
            }
        }


        /// <summary>
        /// check whether this game is locally installed, if true it is. Use Refresh() to
        /// enforce the installation check again.
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                if (refreshInstallationStatusNeeded) // avoid continuous calling of Directory.Exists via this mechanism
                {
                    if (!IsBundleItem)
                    {
                        if (!IsGrowable)            // non-growable items are assumed installed by default
                            isInstalled = true;
                        else
                            isInstalled = Directory.Exists(GameFolder) && (DlAndInstallTask == null || DlAndInstallTask.IsFinished());
                    }
                    else
                    {
                        isInstalled = File.Exists(ExeFilepath) && (DlAndInstallTask == null || DlAndInstallTask.IsFinished());
                    }
                    refreshInstallationStatusNeeded = false;
                }
                return isInstalled;
            }
        }

        public float InstallProgress
        {
            get
            {
                if (ThreadedDlAndInstallTask != null)
                {
                    return (float) ThreadedDlAndInstallTask.Progress();
                }
                else
                {
                    if (IsInstalled)
                        return 1f;
                    else
                        return 0f;
                }
            }
        }

        /// <summary>
        /// check whether this game is a web-playable game in the browser
        /// </summary>
        public bool IsWebGame
        {
            get
            {
                return ExeFile.StartsWith("http://") || ExeFile.StartsWith("https://");
            }
        }

        public bool IsInstalling
        {
            get
            {
                return (ThreadedDlAndInstallTask != null) && (!ThreadedDlAndInstallTask.IsFinished());
            }
        }

        /// <summary>
        /// check whether it is a downloadable update for the Indiegame Garden client
        /// </summary>
        public bool IsIggClient
        {
            get
            {
                return GameID.Equals("igg");
            }
        }

        /// <summary>
        /// check whether this is a gamelib for igg
        /// </summary>
        public bool IsGameLib
        {
            get
            {
                return GameID.StartsWith("gwg_gamelib");
            }
        }

        /// <summary>
        /// check whether this game can be grown at all (i.e. downloaded and/or unpacked).
        /// Some items may not be growable e.g. display-icon-only games, system items, or coming-soon items.
        /// </summary>
        public bool IsGrowable
        {
            get
            {
                if (IsSystemPackage)
                {
                    if (IsIggClient)
                        return true;
                    if (IsGameLib)
                        return true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// checks whether this game can be played (i.e. an .exe file can be launched)
        /// </summary>
        public bool IsPlayable
        {
            get
            {
                if (!IsGrowable)
                    return false;
                if(ExeFile.ToLower().EndsWith(".exe"))
                    return true;
                if (IsSectionId || IsMusic )
                    return false;
                return true; // for games that don't specify their .exe file.                
            }
        }

        /// <summary>
        /// checks whether this game item is a music track (.ogg)
        /// </summary>
        public bool IsMusic
        {
            get
            {
                return (ExeFile.ToLower().EndsWith(".ogg"));
            }
        }

        /// <summary>
        /// checks whether this item is a system item, which are handled slightly 
        /// differently. (E.g. unpacked in another place)
        /// </summary>
        protected bool IsSystemPackage
        {
            get
            {
                return GameID.StartsWith("igg") || GameID.StartsWith("gwg");
            }
        }


        /// <summary>
        /// checks for item of type SectionID.
        /// </summary>
        public bool IsSectionId
        {
            get
            {
                return GameID.StartsWith("section_");
            }
        }

        /// <summary>
        /// checks whether this item is visible to the user, depending on a.o. user's 
        /// client version and other properties of the item
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return  (!IsSectionId) && 
                        (VisibilityLabel > 0);
            }
        }

        /// <summary>
        /// checks whether this item is a bundle-item, which means it is a single .exe
        /// and can be stored in another location i.e. the GardenConfig.BundleDataPath
        /// </summary>
        public bool IsBundleItem
        {
            get
            {
                // if (!PackedFileURL.Contains("/") && PackedFileURL.EndsWith(".exe"))
                if (ExeFile.Equals(packedFileURL) && ExeFile.Length > 0 )
                    return true;
                return false;
            }
        }

        /// <summary>
        /// checks whether this item is a valid 'trainwreck' game that can be included in the library.
        /// Invalid ones are e.g. without downloadable file, or that require Klik 'n Play runtime.
        /// or swf?
        /// </summary>
        public bool IsValidWreck
        {
            get
            {
                if (IsSectionId || IsSystemPackage || IsMusic || IsPositionGiven)
                    return true;

                // if no downloadable file at all, for a non-web-game
                if (!IsWebGame && packedFileURL.Length == 0)
                    return false;

                // if 7zip archive (not supported yet)
                if (packedFileURL.ToLower().EndsWith(".7z"))
                    return false;

                return true;
            }
        }

        public string PackedFileExtension
        {
            get
            {
                string s = ExtractFileExtension(packedFileURL);
                return s;
            }
        }

        /// <summary>
        /// get the folder where a game/item is stored (unpacked)
        /// </summary>
        /// <returns></returns>
        public string GameFolder
        {
            get
            {
                // if bundle, then special path
                if (IsBundleItem)
                    return GardenConfig.Instance.BundleDataPath;
                string folder;
                // if system package then it's located in config files folder
                if (IsSystemPackage)
                    folder = GardenConfig.Instance.ConfigFilesFolder;
                else
                    folder = GardenConfig.Instance.UnpackedFilesFolder;
                return Path.Combine(folder , GameIDwithVersion);
            }
        }

        public string ThumbnailFile
        {
            get
            {
                if (thumbnailURL.Length > 0)
                {
                    // if thumbnail url given, use the file format extension for generating local thumbnail file name.
                    string ext = ExtractFileExtension(thumbnailURL);
                    return GameIDwithVersion + "." + ext;
                }
                else
                {
                    return GameIDwithVersion + ".png";
                }
            }
        }

        public string ThumbnailURL
        {
            get
            {
                if (thumbnailURL.Length > 0)
                {
                    // if only a filename given, assume default url location for thumbs
                    if (!thumbnailURL.Contains("/"))
                        return GardenConfig.Instance.GetThumbnailURL(thumbnailURL);
                    else
                        return thumbnailURL; // in case a full url given with or without http:// in front
                }
                else
                {
                    // default if nothing given at all.
                    return GardenConfig.Instance.GetThumbnailURL(GameIDwithVersion + ".png");
                }
            }
        }

        // extract an extension e.g. "zip" from a partial or full URL e.g. http://server/test/name.zip 
        // <returns>extension after last dot, or default "zip" if no dot found in 'urlDl'.</returns>
        private string ExtractFileExtension(string url)
        {
            int i = url.LastIndexOf('.');
            if (i == -1)
                return "zip";
            string s = url.Substring(i + 1);
            if (s.Length > 3)
            {
                return "zip"; // HACK - if no ext found, assume zip.
            }
            return s;
        }


        /// <summary>
        /// refresh information by reading from local disk (e.g. installation status etc.)
        /// </summary>
        public void Refresh()
        {
            refreshInstallationStatusNeeded = true;
        }

        /// <summary>
        /// create a new instance from a JSON representation
        /// </summary>
        /// <param name="j">the JSON data for one game</param>
        public GardenItem(JsonObject j)
        {
            try { GameID = j["ID"].ToString(); }
            catch (Exception) { ; }
            try { Version = (int)((JsonNumber)j["Version"]).Value; }
            catch (Exception) { ;}
            try { VisibilityLabel = (byte)((JsonNumber)j["Visible"]).Value; }
            catch (Exception) { ;}
            try { PositionX = (int)((JsonNumber)j["X"]).Value; }
            catch (Exception) { ;}
            try { PositionY = (int)((JsonNumber)j["Y"]).Value; }
            catch (Exception) { ;}
            try { PositionDeltaX = (int)((JsonNumber)j["DX"]).Value; }
            catch (Exception) { ;}
            try { PositionDeltaY = (int)((JsonNumber)j["DY"]).Value; }
            catch (Exception) { ;}
            try { ScaleIcon = (float)((JsonNumber)j["Scale"]).Value; }
            catch (Exception) { ;}
            try { Name = j["Name"].ToString(); }
            catch (Exception) { ; }
            try { Description = j["Descr"].ToString(); }
            catch (Exception) { ; }
            try { HelpText = j["Help"].ToString(); }
            catch (Exception) { ; }
            try { ExeFile = j["Exe"].ToString(); }
            catch (Exception) { ; }
            try { CdPath = j["Cd"].ToString(); }
            catch (Exception) { ; }
            try { PackedFileURL = j["Zip"].ToString(); }
            catch (Exception) { ; }
            try { thumbnailURL = j["ThumbURL"].ToString(); }
            catch (Exception) { ; }
            try { DeveloperWebsiteURL = j["Site"].ToString(); }
            catch (Exception) { ; }
            try { 
                JsonArray am = (JsonArray)j["ZipMirrors"];
                packedFileMirrors = JSONStore.ToStringList(am);
            }
            catch (Exception) { ;}
            
            // update with default mirror location, only if a main location is defined
            // if no main location is given, use default location as main DL location 
            string defaultDownloadLoc;
            if (!IsBundleItem)
                defaultDownloadLoc = GardenConfig.Instance.PackedFilesServerURL + PackedFileName;
            else
                defaultDownloadLoc = GardenConfig.Instance.BundleFilesServerURL + PackedFileName;
            if (PackedFileURL.Length > 0)
                packedFileMirrors.Add( defaultDownloadLoc );
            else
                PackedFileURL = defaultDownloadLoc;

        }

        /// <summary>
        /// specific processing for 'wrecks' to fix issues in the json lib during game compilation.
        /// </summary>
        public void WreckProcessing()
        {
            // items without thumbnail get the default one
            if (packedFileURL.Length > 0 && thumbnailURL.Length == 0 && !IsSystemPackage && !IsSectionId )
            {
                thumbnailURL = GardenConfig.Instance.ThumbnailsServerURL + "default.jpg";
            }

            // swf file items become web items
            if (packedFileURL.ToLower().EndsWith(".swf"))
            {
                ExeFile = packedFileURL;
                packedFileURL = "";
            }

        }
    }
}
