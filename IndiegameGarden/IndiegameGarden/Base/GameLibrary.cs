// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetServ.Net.Json;

namespace IndiegameGarden.Base
{
    /**
     * stores a library of game information, parsed from JSON format files
     */
    public class GameLibrary: IDisposable
    {
        JSONStore json;
        GameCollection gamesList;
        int version = 0;

        /// <summary>
        /// create new game library of specified version by loading it from disk using the Load() method
        /// </summary>
        /// <param name="version">version number is required since there may be multiple gamelib versions on disk to choose from</param>
        public GameLibrary(int version)
        {
            this.version = version;
            Load();
        }

        /// <summary>
        /// return version number of this game library
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// (re)load information from config file(s)
        /// </summary>
        /// <exception cref="">various IO exceptions may occur when library file could not be found/loaded</exception>
        public void Load()
        {
            IndieGame g = IndieGame.ConstructGameLib(version);
            string fn = GardenGame.Instance.Config.GetGameFolder(g) + 
                "\\" + GardenGame.Instance.Config.GameLibraryFilename;
            json = new JSONStore(fn); // FIXME use all json files in there?
            gamesList = new GameCollection();
            ParseJson(json);
            // for extra safety, get version nr from the gamelib file and use that from here on.
            version = (int) json.GetValue("version");
        }

        public void Dispose()
        {
            gamesList.Dispose();
        }

        // parse all games in the 'json' data
        private void ParseJson(JSONStore json)
        {
            JsonArray gl = (JsonArray)json.GetArray("gameslist");
            foreach( IJsonType g in gl )
            {
                IndieGame ig = new IndieGame( (JsonObject)g );
                if (!ig.IsVisible)
                    continue;
                gamesList.Add(ig);
            }
        }

        /// <summary>
        /// get a GameCollection containing all games in the library
        /// </summary>
        /// <returns>GameCollection containing all games in the library</returns>
        public GameCollection GetList()
        {
            return gamesList;
        }
    }
}
