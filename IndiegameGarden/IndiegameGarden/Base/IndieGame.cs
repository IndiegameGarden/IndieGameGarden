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
        public string Description = "";

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
        public string[] PackedFileMirrors = new string[]{};

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
        /// scaling factor of game icon when displayed
        /// </summary>
        public float ScaleIcon = 1f;

        /// <summary>
        /// where in 2D coordinates this game is positioned
        /// </summary>
        public Vector2 Position = Vector2.Zero;

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

        public IndieGame()
        {
            throw new NotImplementedException("Constructor");
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
                    isInstalled =   IsPlayable &&
                                    Directory.Exists(gameDirPath) &&
                                    File.Exists(exePath) &&
                                    (DlAndInstallTask == null || DlAndInstallTask.IsFinished()) ;
                    refreshInstallationStatusNeeded = false;
                }
                return isInstalled;
            }
        }

        /// <summary>
        /// check whether this game can be played at all (i.e. grown in the garden).
        /// Some items may not be played e.g. display-icon-only games or coming-soon items.
        /// </summary>
        public bool IsPlayable
        {
            get
            {
                return ExeFile.Length > 0;
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
                PackedFileMirrors = JSONStore.ToStringArray(am);
            }
            catch (Exception) { ;}
        }


    }
}
