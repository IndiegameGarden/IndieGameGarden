// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetServ.Net.Json;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace IndiegameGarden.Base
{
    /**
     * stores a library of game information, parsed from JSON format files
     */
    public class GameLibrary: IDisposable
    {
        JSONStore json;
        GameCollection gamesCollection;
        int version = 0;

        /// <summary>
        /// create new game library 
        /// </summary>
        public GameLibrary()
        {
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
        /// width of garden (number of items horizontally)
        /// </summary>
        public int GardenSizeX
        {
            get
            {
                return 100; // TODO configurable
            }
        }

        /// <summary>
        /// max height of garden (number of items vertically)
        /// </summary>
        public int GardenSizeY
        {
            get
            {
                return 100; // TODO configurable
            }
        }

        /// <summary>
        /// (re)load information from default JSON config file(s) of a specific version number
        /// </summary>
        /// <param name="version">version number is required since there may be multiple gamelib versions on disk to choose from</param>
        /// <exception cref="">various IO exceptions may occur when library file could not be found/loaded</exception>
        public void LoadJson(int version)
        {
            GardenItem g = GardenItem.ConstructGameLibItem(version);
            string fn = g.GameFolder + "\\" + GardenConfig.Instance.GameLibraryFilename;
            LoadJson(fn);
            this.version = version;
        }

        /// <summary>
        /// (re)load information from default binary (protocol buffers) config file(s) of a specific version number
        /// </summary>
        /// <param name="version">version number is required since there may be multiple gamelib versions on disk to choose from</param>
        /// <exception cref="">various IO exceptions may occur when library file could not be found/loaded</exception>
        public void LoadBin(int version)
        {
            GardenItem g = GardenItem.ConstructGameLibItem(version);
            string fn = g.GameFolder + "\\" + GardenConfig.Instance.GameLibraryFilenameBin;
            LoadBin(fn);
            this.version = version;            
        }

        /// <summary>
        /// (re)load information from given Json file
        /// </summary>
        /// <exception cref="">various IO exceptions may occur when library file could not be found/loaded</exception>
        public void LoadJson(string libraryFile)
        {
            json = new JSONStore(libraryFile); // TODO use all json files in there?
            gamesCollection = new GameCollection(GardenSizeX, GardenSizeY, new List<GardenItem>()); 
            ParseJson(json);
        }

        /// <summary>
        /// (re)load information from given binary (protobuf) file
        /// </summary>
        /// <param name="libraryFile"></param>
        public void LoadBin(string libraryFile)
        {            
            using (var file = File.OpenRead(libraryFile))
            {
                gamesCollection = new GameCollection(GardenSizeX, GardenSizeY, Serializer.Deserialize<List<GardenItem>>(file));             
            }
        }

        public void Dispose()
        {
            gamesCollection.Dispose();
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
        protected GardenItem ParseJson(IJsonType j, Vector2 posOffset)
        {
            if (j is JsonArray)
            {
                JsonArray ja = j as JsonArray;
                Vector2 childPosOffset = posOffset;
                Vector2 posPrevious = Vector2.Zero;
                //Vector2 childPosPrevious = Vector2.Zero;
                Vector2 sectionWidthHeight = new Vector2(999f, 999f);
                GardenItem gi = null;
                
                foreach (IJsonType jChild in ja)
                {
                    // parse each subitem and add to gamelist
                    gi = ParseJson(jChild, childPosOffset);
                    if (gi == null)
                        continue;

                    // optional first SectionID item of a JsonArray may contain position offset info for all items
                    if (gi.IsSectionId)
                    {
                        childPosOffset += gi.Position;
                        // WARNING mis-use the posdelta field for section width/height!!
                        sectionWidthHeight = gi.PositionDelta;
                        //childPosPrevious = Vector2.Zero;
                        posPrevious = Vector2.Zero - Vector2.UnitX; // for first item of a section, apply a shift to align item to (0,0)
                        continue;
                    }                    

                    // calculate correct item position
                    if (!gi.IsPositionGiven)
                    {
                        gi.Position = posPrevious;
                        do
                        {
                            if (gi.IsPositionDeltaGiven)
                                gi.Position += gi.PositionDelta;
                            else
                                gi.Position += Vector2.UnitX; // advance in standard way to the right

                            // checking the automatic calculated game position with section width
                            if (gi.PositionX >= sectionWidthHeight.X)
                            {
                                gi.PositionY += 1;
                                gi.PositionX = 0;
                            }
                        } while (gamesCollection.FindGameAt(gi.Position + childPosOffset) != null);
                    }                    

                    // update prev item position 
                    posPrevious = gi.Position;

                    // apply the section position offset
                    gi.PositionX += (int) childPosOffset.X;
                    gi.PositionY += (int) childPosOffset.Y;

                    // add to collection at specified position
                    gamesCollection.Add(gi);
                }
                return null; // indicate array was last item.
            }
            else if (j is JsonObject)
            {
                // process single leaf item
                GardenItem ig = new GardenItem((JsonObject)j);
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
            return gamesCollection;
        }
    }
}
