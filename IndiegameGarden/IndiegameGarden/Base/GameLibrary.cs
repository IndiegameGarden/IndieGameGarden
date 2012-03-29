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
        protected IndieGame ParseJson(IJsonType j, Vector2 posOffset)
        {
            if (j is JsonArray)
            {
                JsonArray ja = j as JsonArray;
                Vector2 childPosOffset = posOffset;
                Vector2 posPrevious = Vector2.Zero;
                //Vector2 childPosPrevious = Vector2.Zero;
                Vector2 sectionWidthHeight = new Vector2(999f, 999f);
                IndieGame ig = null;
                
                foreach (IJsonType jChild in ja)
                {
                    // parse each subitem and add to gamelist
                    ig = ParseJson(jChild, childPosOffset);
                    if (ig == null)
                        continue;

                    // optional first SectionID item of a JsonArray may contain position offset info for all items
                    if (ig.GameID.StartsWith("section_"))
                    {
                        childPosOffset += ig.Position;
                        // WARNING mis-use the posdelta field for section width/height!!
                        sectionWidthHeight = ig.PositionDelta;
                        //childPosPrevious = Vector2.Zero;
                        posPrevious = Vector2.Zero;
                        continue;
                    }                    

                    // calculate correct item position
                    if (!ig.IsPositionGiven)
                    {
                        ig.Position = posPrevious + ig.PositionDelta;
                        // checking the automatic calculated game position with section width
                        if (ig.Position.X >= sectionWidthHeight.X)
                        {
                            ig.Position.Y += 1.0f;
                            ig.Position.X = 0f;
                        }
                    }                    

                    // update prev item position 
                    posPrevious = ig.Position;

                    // apply the section position offset
                    ig.Position += childPosOffset;

                }
                return null; // indicate array was last item.
            }
            else if (j is JsonObject)
            {
                // process single leaf item
                IndieGame ig = new IndieGame((JsonObject)j);
                if (ig.IsVisible && ig.GameID.Length > 0)
                    gamesList.Add(ig);
                return ig;
            }
            else
                throw new NotImplementedException("Unknown JSON type " + j + " found.");
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
