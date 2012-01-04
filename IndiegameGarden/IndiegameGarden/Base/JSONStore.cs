// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetServ.Net.Json;

namespace IndiegameGarden.Base
{
    /**
     * JSON storage object, can read JSON from a file
     */
    public class JSONStore
    {
        private string fn = null;
        JsonObject json = null;

        /// <summary>
        /// create new instance from a .json file and load it
        /// </summary>
        /// <param name="filename"></param>
        public JSONStore(string filename)
        {
            fn = filename;
            Load();
        }

        /// <summary>
        /// utility method to translate a JsonArray to a string[]
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
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

        /// <summary>
        /// retrieve a JsonArray from the store with given name
        /// </summary>
        /// <param name="key">name of the JSON array to return</param>
        /// <returns>JsonArray with given name 'key'</returns>
        public JsonArray GetArray(string key)
        {
            return (JsonArray) json[key];
        }
    }
}
