// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using TTengine.Core;
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
        /// default visibility for user status 1 (yes) or 0 (no)
        /// </summary>
        [ProtoMember(2)]
        public int VisibilityLabel = 1;

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

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                lineCount = PerformLineCount(description);
            }
        }

        /// <summary>
        /// returns a human-readable string name for this item
        /// </summary>
        public string ItemName
        {
            get
            {
                if (VisibilityLabel == 0) return "invisible-item";
                if (IsMusic)         return "music";
                if (IsSystemPackage) return "system-package";

                return "game";
            }
        }

        int PerformLineCount(string s)
        {
            int result = 1;
            foreach (char c in s)
            {
                if (c.Equals('\n'))
                {
                  result++;
                }
            }
            return result;
        }

        protected int lineCount;

        public int DescriptionLineCount
        {
            get
            {
                return lineCount;
            }
        }

        /// <summary>
        /// some hints for the player e.g. what the control keys are.
        /// </summary>
        [ProtoMember(5)]
        public string HelpText = "";

        [ProtoMember(6)]
        protected string packedFileURL = "";

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
                if (!packedFileURL.Contains("/"))
                    return GardenConfig.Instance.PackedFilesServerURL + packedFileURL;
                else
                    return packedFileURL ;
            }
            set
            {
                packedFileURL = value;
            }
        }

        /// <summary>
        /// where a .EXE_NOT_PACKED file can be downloaded from, which needs no unpacking (run straight away)
        /// </summary>
        [ProtoMember(7)]
        public string ExeFileURL = "";

        [ProtoMember(8)]
        protected List<string> packedFileMirrors = new List<string>();

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
        /// URL (optionally without the http:// or www. in front) to game developer's website
        /// </summary>
        [ProtoMember(9)]
        public string DeveloperWebsiteURL = "";

        /// <summary>
        /// name of .exe file or .bat to launch to start game
        /// </summary>
        [ProtoMember(10)]
        public string ExeFile = "";

        /// <summary>
        /// directory gameDirPath that OS has to 'change directory' to, before launching the game
        /// </summary>
        [ProtoMember(11)]
        public string CdPath = "";

        /// <summary>
        /// Latest version of the game packed file which is available
        /// </summary>
        [ProtoMember(12)]        
        public int Version = 1;

        /// <summary>
        /// only show this item if client version is below this version number. 0 is unitialized
        /// </summary>
        [ProtoMember(13)]        
        protected int showBelowClientVersion = 0;

        public int ShowBelowClientVersion
        {
            get
            {
                if (showBelowClientVersion == 0)
                    return 999999;
                else
                    return showBelowClientVersion;
            }
            set
            {
                showBelowClientVersion = value;
            }
        }

        /// <summary>
        /// scaling factor of game icon when displayed
        /// </summary>
        [ProtoMember(14)]
        public float ScaleIcon = 1f;

        /// <summary>
        /// where in 2D coordinates this game is positioned. Zero means non-specified.
        /// </summary>
        [ProtoMember(15)]
        public int PositionX = 0;
        [ProtoMember(16)]        
        public int PositionY = 0;

        /// <summary>
        /// in case a 2D Position is not given, this specifies a wished position delta of game w.r.t. previous game in the library.
        /// </summary>
        [ProtoMember(17)]        
        public int PositionDeltaX = 0;
        [ProtoMember(18)]        
        public int PositionDeltaY = 0;

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

        [ProtoMember(19)]
        protected string thumbnailURL = "";

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
            g.GameID = "igg_gamelib";
            g.ExeFile = "gamelib.json";
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
                if (refreshInstallationStatusNeeded)
                {
                    isInstalled =   IsGrowable &&
                                    Directory.Exists(GameFolder) &&
                                    (DlAndInstallTask == null || DlAndInstallTask.IsFinished()) ;
                    refreshInstallationStatusNeeded = false;
                }
                return isInstalled;
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
        /// check whether this game can be grown at all (i.e. downloaded).
        /// Some items may not be growable e.g. display-icon-only games or coming-soon items.
        /// </summary>
        public bool IsGrowable
        {
            get
            {
                if (IsSectionId || IsSystemPackage)
                    return false;
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
                if(ExeFile.ToLower().EndsWith(".exe"))
                    return true;
                if (IsSectionId || IsMusic || IsSystemPackage)
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

        public bool IsSystemPackage
        {
            get
            {
                return GameID.StartsWith("igg_");
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
                        (GardenConfig.Instance.ClientVersion < ShowBelowClientVersion) && 
                        (VisibilityLabel > 0);
            }
        }

        public string PackedFileExtension
        {
            get
            {
                string s = ExtractFileExtension(PackedFileURL);
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
                string folder = GardenConfig.Instance.UnpackedFilesFolder;
                // if system package then it's located in config files folder
                if (IsSystemPackage)
                {
                    folder = GardenConfig.Instance.ConfigFilesFolder;
                }
                return folder + "\\" + GameIDwithVersion;
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
            catch (Exception ex) { throw (ex); }
            try { Version = (int)((JsonNumber)j["Version"]).Value; }
            catch (Exception) { ;}
            try { VisibilityLabel = (int)((JsonNumber)j["Visible"]).Value; }
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
            try { ShowBelowClientVersion = (int)((JsonNumber)j["ShowBelowVer"]).Value; }
            catch (Exception) { ;}
            
            // update with default mirror location, only if a main location is defined
            // if no main location is given, use default location as main DL location which assumes a .zip file type too.
            string defaultDownloadLoc = GardenConfig.Instance.PackedFilesServerURL + GardenConfig.Instance.GetPackedFileName(this);
            if (PackedFileURL.Length > 0)
                packedFileMirrors.Add( defaultDownloadLoc );
            else
                PackedFileURL = defaultDownloadLoc;

            // special case: igg, the IndiegameGarden client itself, then enter version numbers info from the config
            if (GameID.Equals("igg"))
            {
                // make the item conditionally show - only if user has a lower version of the client!
                ShowBelowClientVersion = GardenConfig.Instance.NewestClientVersion;
                Version = GardenConfig.Instance.NewestClientVersion;
            }
        }

    }
}
