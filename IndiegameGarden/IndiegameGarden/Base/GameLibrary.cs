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
    public class GameLibrary
    {
        JSONStore json;
        GameCollection gamesList;

        public GameLibrary()
        {
            Load();
        }

        /// <summary>
        /// (re)load information from config file(s)
        /// </summary>
        public void Load()
        {
            string fn = GardenGame.Instance.Config.GameLibraryFilename;
            json = new JSONStore(fn);
            gamesList = new GameCollection();
            ParseJson();
        }

        // parse all games in the 'json' data
        private void ParseJson()
        {
            JsonArray gl = (JsonArray)json.GetArray("gameslist");
            foreach( IJsonType g in gl )
            {
                IndieGame ig = new IndieGame( (JsonObject)g );
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
