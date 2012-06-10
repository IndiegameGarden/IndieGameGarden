using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndiegameGarden.Base;
using ProtoBuf;

namespace GameLibCompiler
{
    class GameLibCompiler
    {
        public GameLibrary GameLib;

        const string CONFIG_DIR = "..\\..\\..\\..\\config";

        /// <summary>
        /// target dir for checking in new files into version control server ("master" version)
        /// </summary>
        const string GAMELIB_TARGET_DIR = "gamelib_fmt3";

        /// <summary>
        /// target path to put a copy of latest gamelib binary (in the IGG XNA Content project)
        /// </summary>
        const string CONTENT_TARGET_DIR = "..\\..\\..\\IndiegameGardenContent";

        /// <summary>
        /// target dir where IGG client unpacks downloaded gamelib zips into ("local copy" version).
        /// For local testing, the new gamelib files need to be in here. This is not checked into
        /// the version control server.
        /// </summary>
        const string GAMELIB_UNPACKED_TARGET_DIR = "igg_gamelib_fmt3";

        const string GAMELIB_JSON_FILE = "gamelib.json";

        const string GAMELIB_BIN_FILE = "gamelib.bin";
        
        string GAMELIB_JSON_PATH = Path.Combine(CONFIG_DIR, GAMELIB_TARGET_DIR, GAMELIB_JSON_FILE);
        string GAMELIB_BIN_PATH = Path.Combine(CONFIG_DIR, GAMELIB_TARGET_DIR, GAMELIB_BIN_FILE);
        string GAMELIB_UNPACKED_PATH = Path.Combine(CONFIG_DIR, GAMELIB_UNPACKED_TARGET_DIR, GAMELIB_BIN_FILE);
        string GAMELIB_UNPACKED_DIR_PATH = Path.Combine(CONFIG_DIR, GAMELIB_UNPACKED_TARGET_DIR);
        string GAMELIB_CONTENTFOLDER_PATH = Path.Combine(CONTENT_TARGET_DIR, GAMELIB_BIN_FILE);

        public void Run()
        {
            int t0, t1;
            Log("GameLibCompiler started.");

            GameLib = new GameLibrary();
            t0 = Environment.TickCount;
            GameLib.LoadJson(GAMELIB_JSON_PATH);
            t1 = Environment.TickCount;
            Log("Json load: " + (t1 - t0) + " ms.");

            // save
            using (var file = File.Create(GAMELIB_BIN_PATH))
            {
                t0 = Environment.TickCount;
                Serializer.Serialize(file, GameLib.GetList().AsList());
                t1 = Environment.TickCount;
                Log("Bin  save: " + (t1 - t0) + " ms.");
            }

            // test load
            List<GardenItem> l;
            using (var file = File.OpenRead(GAMELIB_BIN_PATH))
            {
                t0 = Environment.TickCount;
                l = Serializer.Deserialize<List<GardenItem>>(file);
                t1 = Environment.TickCount;
                Log("Bin  load test 1: " + (t1 - t0) + " ms.");
            }

            // test load 2
            t0 = Environment.TickCount;
            GameLibrary gl = new GameLibrary();
            gl.LoadBin(GAMELIB_BIN_PATH);
            t1 = Environment.TickCount;
            Log("Bin load test 2: " + (t1 - t0) + " ms.");
            int c = gl.GetList().Count;

            // copy to gamelib unpacking location
            Log("Copying master " + GAMELIB_BIN_FILE + " to unpacking location " + GAMELIB_UNPACKED_TARGET_DIR);
            File.Copy(GAMELIB_BIN_PATH, GAMELIB_CONTENTFOLDER_PATH,true);
            try
            {
                Directory.CreateDirectory(GAMELIB_UNPACKED_DIR_PATH);
            }
            catch (Exception)
            {
                ;
            }
            File.Copy(GAMELIB_BIN_PATH, GAMELIB_UNPACKED_PATH, true);

            Log("\nDone - press any key.");
            Console.ReadKey();
        }

        void Log(string s)
        {
            System.Console.WriteLine(s);
        }
    }
}
