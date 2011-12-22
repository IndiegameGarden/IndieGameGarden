// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetServ.Net.Json;

namespace IndiegameGarden.Store
{
    public class GameLibrary
    {

        JSONStore json;
        List<IndieGame> indieGamesList;

        public GameLibrary()
        {
            Load();
        }

        public void Load()
        {
            string fn = GardenMain.Instance.storageConfig.GameLibraryFilename;
            json = new JSONStore(fn);
            indieGamesList = new List<IndieGame>();
            ParseJson();
        }

        private void ParseJson()
        {
            //JsonString name = (JsonString)obj["Name"];
            JsonArray gl = (JsonArray)json.GetArray("gameslist");
            foreach( IJsonType g in gl )
            {
                //JsonString n = (JsonString) ((JsonObject)g)["Name"];
                IndieGame ig = new IndieGame( (JsonObject)g );
                indieGamesList.Add(ig);
            }
        }

        public List<IndieGame> GetList()
        {
            return indieGamesList;
        }
    }
}
