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
    public class IndieGame
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
        /// where can the packed file (.zip, .rar etc.) be downloaded from.
        /// Optionally without http:// in front.
        /// </summary>
        public string PackedFileURL = "";

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
        /// where in 2D coordinates this game is positioned
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Optionally a download/install task ongoing for this game
        /// </summary>
        public GameDownloadAndInstallTask DlAndInstallTask = null;

        //-- private vars
        private bool isInstalled = false;
        private bool installationChanged = true;

        public IndieGame()
        {
            throw new NotImplementedException("Constructor");
        }

        /// <summary>
        /// check whether this game is locally installed, if true it is.
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                if (installationChanged)
                {
                    String gameDirPath = GardenGame.Instance.Config.GetGameFolder(GameID, Version);
                    String exePath = GardenGame.Instance.Config.GetExeFilepath(GameID, Version, CdPath, ExeFile);
                    isInstalled = Directory.Exists(gameDirPath) && File.Exists(exePath);
                    installationChanged = false;
                }
                return isInstalled;
            }
        }

        /// <summary>
        /// refresh information by reading from local disk (e.g. installation status etc.)
        /// </summary>
        public void Refresh()
        {
            installationChanged = true;
        }

        /// <summary>
        /// the name of the packed file (eg .zip or .rar) once it is downloaded. May differ
        /// from the name of the archive as stored on the web which is included in PackedFileURL.
        /// </summary>
        public string PackedFileName
        {
            get
            {
                return GameID + "_v" + Version + "." + ExtractFileExtension(PackedFileURL);
            }
        }

        /// <summary>
        /// create a new instance from a JSON representation
        /// </summary>
        /// <param name="j">the JSON data for one game</param>
        public IndieGame(JsonObject j)
        {
            try { GameID = j["GameID"].ToString(); }                catch (KeyNotFoundException ex) { throw (ex);  }
            try { Version = (int) ((JsonNumber)j["Version"]).Value; }     catch (KeyNotFoundException) { ;}
            try { Position.X = (float) ((JsonNumber)j["X"]).Value; }
            catch (KeyNotFoundException) { ;}
            try { Position.Y = (float) ((JsonNumber)j["Y"]).Value; }
            catch (KeyNotFoundException) { ;}
            try { Name = j["Name"].ToString(); }                     catch (KeyNotFoundException) { ; }
            try { Description = j["Description"].ToString(); }       catch (KeyNotFoundException) { ; }
            try { ExeFile = j["ExeFile"].ToString(); }            catch (KeyNotFoundException) { ; }
            try { CdPath   = j["CdPath"].ToString(); }            catch (KeyNotFoundException) { ; }
            try { PackedFileURL = j["PackedFileURL"].ToString(); }
            catch (KeyNotFoundException) { ; }
            try { DeveloperWebsiteURL = j["DeveloperWebsiteURL"].ToString(); } catch (KeyNotFoundException) { ; }
            try { 
                JsonArray am = (JsonArray)j["PackedFileMirrors"];
                PackedFileMirrors = JSONStore.ToStringArray(am);
            }catch(KeyNotFoundException){;}
        }

        // extract an extension e.g. "zip" from a full URL http://server/test/name.zip 
        // <returns>extension after last dot, or empty string if no dot found in 'url'.
        private string ExtractFileExtension(string url)
        {
            int i = url.LastIndexOf('.');
            if (i == -1)
                return "";
            return url.Substring(i + 1);
        }


    }
}
