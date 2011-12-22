// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetServ.Net.Json;

namespace IndiegameGarden.Store
{
    public class JSONStore
    {
        private string fn = null;
        JsonObject json = null;

        public JSONStore(string filename)
        {
            fn = GardenMain.Instance.storageConfig.ConfigFilesFolder + "\\" + filename;
            Load();
        }

        public static string[] ToStringArray(JsonArray j)
        {
            string[] s = new string[j.Count];
            for (int i = j.Count - 1; i >= 0; i--)
            {
                s[i] = ((JsonString)j[i]).ToString();
            }
            return s;
        }


        protected void Load()
        {
            TextReader tr = File.OpenText(@fn);
            JsonParser parser = new JsonParser(tr, true);
            json = (JsonObject)parser.ParseObject();
            //JsonString name = (JsonString)obj["Name"];
            //JsonNumber age = (JsonNumber)obj["Age"];
            //JsonBoolean hungry = (JsonBoolean)obj["Hungry?"];

        }

        public JsonArray GetArray(string key)
        {
            return (JsonArray) json[key];
        }
    }
}
