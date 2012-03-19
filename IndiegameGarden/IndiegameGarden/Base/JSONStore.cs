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
        protected string jsonFilePath = null;
        protected JsonObject json = null;

        /// <summary>
        /// create new instance from a .json file and load it
        /// </summary>
        /// <param name="filename"></param>
        public JSONStore(string filename)
        {
            jsonFilePath = filename;
            LoadJson();
        }

        /// <summary>
        /// for use by subclasses - create but don't load .json file yet
        /// </summary>
        protected JSONStore()
        {
            json = new JsonObject();
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

        public static List<string> ToStringList(JsonArray j)
        {
            List<string> s = new List<string>();
            for (int i = 0; i < j.Count ; i++)
            {
                s.Add( ((JsonString)j[i]).ToString() );
            }
            return s;
        }

        protected void LoadJson()
        {
            TextReader tr = null;
            try
            {
                tr = File.OpenText(jsonFilePath);
                JsonParser parser = new JsonParser(tr, true);
                json = (JsonObject)parser.ParseObject();
            }
            finally
            {
                if (tr != null)
                    tr.Close();
            }
        }

        /// <summary>
        /// reload configuration from .json file on disk, e.g. after file was changed or downloaded again
        /// </summary>
        public virtual void Reload()
        {
            LoadJson();
        }

        /// <summary>
        /// save configuration to .json file, overwriting the old file
        /// </summary>
        public virtual void Save()
        {
            FileStream ios = File.OpenWrite(jsonFilePath);
            StreamWriter tw = new StreamWriter(ios);
            JsonWriter jsonWriter = new JsonWriter(tw,true);
            json.Write(jsonWriter);
            tw.Close();
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

        /// <summary>
        /// retrieve a Json string from the store with given name
        /// </summary>
        /// <param name="key">name of the JSON string to return</param>
        /// <returns>the string value stored under the key 'key'</returns>
        public string GetString(string key)
        {
            return ((JsonString)json[key]).ToString();
        }

        public double GetValue(string key)
        {
            try
            {
                return ((JsonNumber)json[key]).Value;
            }
            catch (InvalidCastException ex)
            {
                // try parsing from string.
                string s = GetString(key);
                double d = Double.Parse(s);
                return d;
            }
        }

        public void PutValue(string key, long value)
        {
            PutValue(key, new JsonNumber(value));
        }

        public void PutValue(string key, double value)
        {
            PutValue(key, new JsonNumber(value));
        }

        public void PutValue(string key, int value)
        {
            PutValue(key, new JsonNumber(value));
        }

        protected void PutValue(string key, JsonNumber value)
        {
            json[key] = value;
        }

        public bool HasKey(string key)
        {
            return json.ContainsKey(key);
        }
    }
}
