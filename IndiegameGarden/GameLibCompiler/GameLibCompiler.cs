using System;
using System.IO;
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

        public void Run()
        {
            int t0, t1;

            GameLib = new GameLibrary();
            t0 = Environment.TickCount;
            GameLib.LoadJson("..\\..\\..\\..\\config\\igg_gamelib_v3\\gamelib.json");
            t1 = Environment.TickCount;
            System.Console.WriteLine("Json load: " + (t1 - t0) + " ms.");

            // save
            using (var file = File.Create("gamelib.bin"))
            {
                t0 = Environment.TickCount;
                Serializer.Serialize(file, (List<GardenItem>) GameLib.GetList());
                t1 = Environment.TickCount;
                System.Console.WriteLine("Bin  save: " + (t1 - t0) + " ms.");
            }

            // test load
            List<GardenItem> l;
            using (var file = File.OpenRead("gamelib.bin"))
            {
                t0 = Environment.TickCount;
                l = Serializer.Deserialize<List<GardenItem>>(file);
                t1 = Environment.TickCount;
                System.Console.WriteLine("Bin  load: " + (t1 - t0) + " ms.");
            }

            // test load 2
            GameLibrary gl = new GameLibrary();
            gl.LoadBin("gamelib.bin");
            int c = gl.GetList().Count;
        }
    }
}
