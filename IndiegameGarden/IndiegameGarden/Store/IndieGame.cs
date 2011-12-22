// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TTengine.Core;
using IndiegameGarden.Menus;
using NetServ.Net.Json;

namespace IndiegameGarden.Store
{
    /**
     * <summary>represents all data of a game that a user can select, download and start/play</summary>
     */
    public class IndieGame
    {
        public string GameID = "";
        public string Name = "";
        public string Description = "";
        public string PackedFileURL = "";
        public string[] PackedFileMirrors = new string[]{};
        public string DeveloperWebsiteURL = "";
        public string ExeFile = "";
        public string CdPath = ".";
        public double Version = 1;

        bool isInstalled;

        public IndieGame()
        {
            throw new NotImplementedException("Constructor");
        }

        public bool IsInstalled
        {
            get
            {
                return isInstalled;
            }
        }

        public IndieGame(JsonObject j)
        {
            try { GameID = j["GameID"].ToString(); }                catch (KeyNotFoundException ex) { throw (ex);  }
            try { Version = ((JsonNumber)j["Version"]).Value; }     catch (KeyNotFoundException) { ;}
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
            String path = GardenMain.Instance.storageConfig.CreateGameFolder(GameID);
            isInstalled = Directory.Exists(path);            
        }


    }
}
