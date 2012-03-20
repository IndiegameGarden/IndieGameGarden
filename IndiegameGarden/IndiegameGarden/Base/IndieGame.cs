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

namespace IndiegameGarden.Base
{
    /**
     * <summary>represents all data and status of a game that a user can select, download and start/play</summary>
     */
    public class IndieGame: IDisposable
    {
        /// <summary>
        /// internally used string ID for game, no whitespace allowed, only alphanumeric and 
        /// _ - special characters allowed.
        /// </summary>
        public string GameID = "";

        /// <summary>
        /// Name of game
        /// </summary>
        public string Name = "";

        /// <summary>
        /// short game description to show on screen
        /// </summary>
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
        public string HelpText = "";

        /// <summary>
        /// where can the packed file (.zip, .rar etc.) be downloaded from.
        /// Optionally without http:// in front.
        /// </summary>
        public string PackedFileURL = "";

        /// <summary>
        /// where a .EXE_NOT_PACKED file can be downloaded from, which needs no unpacking (run straight away)
        /// </summary>
        public string ExeFileURL = "";

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

        protected List<string> packedFileMirrors = new List<string>();

        /// <summary>
        /// URL (optionally without the http:// or www. in front) to game developer's website
        /// </summary>
        public string DeveloperWebsiteURL = "";

        /// <summary>
        /// name of .exe file or .bat to launch to start game
        /// </summary>
        public string ExeFile = "";

        /// <summary>
        /// directory gameDirPath that OS has to 'change directory' to, before launching the game
        /// </summary>
        public string CdPath = ".";

        /// <summary>
        /// Latest version of the game packed file which is available
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// only show this item if client version is below this version number
        /// </summary>
        public int ShowBelowClientVersion = 99999999;

        /// <summary>
        /// scaling factor of game icon when displayed
        /// </summary>
        public float ScaleIcon = 1f;

        /// <summary>
        /// selection of rendering/shading effect for icon rendering. 0 = off.
        /// </summary>
        public int FXmode = 1;

        /// <summary>
        /// PNG icon mode selection (instead of JPG)
        /// </summary>
        public bool isPNG = false;

        /// <summary>
        /// where in 2D coordinates this game is positioned
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        public double SoundVolume = 0.5;

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

        protected IndieGame()
        {            
        }

        public static IndieGame ConstructGameLib(int version)
        {
            IndieGame g = new IndieGame();
            g.Version = version;
            g.GameID = "gamelib";
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
        /// check whether this game is locally installed, if true it is. Use Refresh() to
        /// enforce the installation check again.
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                if (refreshInstallationStatusNeeded)
                {
                    String gameDirPath = GardenGame.Instance.Config.GetGameFolder(this);
                    String exePath = GardenGame.Instance.Config.GetExeFilepath(this);
                    isInstalled =   IsGrowable &&
                                    Directory.Exists(gameDirPath) &&
                                    File.Exists(exePath) &&
                                    (DlAndInstallTask == null || DlAndInstallTask.IsFinished()) ;
                    refreshInstallationStatusNeeded = false;
                }
                return isInstalled;
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
                return ExeFile.Length > 0;
            }
        }

        /// <summary>
        /// checks whether this game can be played (i.e. an .exe file can be launched)
        /// </summary>
        public bool IsPlayable
        {
            get
            {
                return ExeFile.ToLower().EndsWith(".exe");
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
        /// checks whether this item is visible to the user, depending on a.o. user's 
        /// client version and other properties of the item
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (GardenGame.Instance.Config.ClientVersion < ShowBelowClientVersion);
            }
        }

        /// <summary>
        /// get this game's thumbnail filename
        /// </summary>
        public string ThumbnailFilename
        {
            get
            {
                if (Version == 1)
                    return GameID + "." + ThumbnailFiletype;
                else
                    return GameID + "_v" + Version + "." + ThumbnailFiletype;
            }
        }

        /// <summary>
        /// thumbnail file type eg "jpg" or "png"
        /// </summary>
        public string ThumbnailFiletype
        {
            get
            {
                if (isPNG)
                    return "png";
                return "jpg";
            }
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
        public IndieGame(JsonObject j)
        {
            try { GameID = j["ID"].ToString(); }
            catch (Exception ex) { throw (ex); }
            try { Version = (int)((JsonNumber)j["Version"]).Value; }
            catch (Exception) { ;}
            try { Position.X = (float) ((JsonNumber)j["X"]).Value; }
            catch (Exception) { ;}
            try { Position.Y = (float) ((JsonNumber)j["Y"]).Value; }
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
            try { DeveloperWebsiteURL = j["Site"].ToString(); }
            catch (Exception) { ; }
            try { 
                JsonArray am = (JsonArray)j["ZipMirrors"];
                packedFileMirrors = JSONStore.ToStringList(am);
            }
            catch (Exception) { ;}
            try { isPNG = (((JsonNumber)j["PNG"]).Value > 0 ); }
            catch (Exception) { ;}
            try { FXmode = (int) ((JsonNumber)j["FX"]).Value; }
            catch (Exception) { ;}
            try { ShowBelowClientVersion = (int)((JsonNumber)j["ShowBelowVer"]).Value; }
            catch (Exception) { ;}
            try { SoundVolume = (double)((JsonNumber)j["Vol"]).Value; }
            catch (Exception) { ;}
            
            // update with default mirror location
            packedFileMirrors.Add(GardenGame.Instance.Config.PackedFilesServerURL + GardenGame.Instance.Config.GetPackedFileName(this) );

            // special case: igg, then enter version numbers info from the config
            if (GameID.Equals("igg"))
            {
                ShowBelowClientVersion = GardenGame.Instance.Config.NewestClientVersion;
                Version = GardenGame.Instance.Config.NewestClientVersion;
            }
        }


    }
}
