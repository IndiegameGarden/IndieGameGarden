// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetServ.Net.Json;
using Microsoft.Xna.Framework;

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
            string fn = g.GameFolder + "\\" + GardenGame.Instance.Config.GameLibraryFilename;
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
            ParseJson(gl,Vector2.Zero);
        }

        /// <summary>
        /// parse a JsonArray (array of items) or JsonObject (single game/item)
        /// </summary>
        /// <param name="j"></param>
        private void ParseJson(IJsonType j, Vector2 posOffset)
        {
            if (j is JsonArray)
            {
                JsonArray ja = j as JsonArray;
                bool offsetKnown = false;
                Vector2 offset = posOffset;
                foreach (IJsonType g2 in ja)
                {
                    // first item contains the offset for all items
                    if (!offsetKnown && (g2 is JsonObject) )
                    {
                        JsonObject jo = (JsonObject)g2;
                        if (jo.ContainsKey("SectionID"))
                        {
                            offset += new Vector2((float)(jo["X"] as JsonNumber).Value, (float)(jo["Y"] as JsonNumber).Value);
                            offsetKnown = true;
                        }
                        else
                        {
                            ParseJson(g2, offset);
                        }
                    }
                    else
                    {
                        ParseJson(g2, offset);
                    }
                }
            }
            else
            {
                IndieGame ig = new IndieGame((JsonObject) j);
                ig.Position += posOffset;
                if (ig.IsVisible)
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
