using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IggLib.Util;
using ProtoBuf;

namespace IggLib.Base
{
    /**
     * a collection/list of games
     */
    public class GameCollection: IDisposable
    {
        int sizeX = 0, sizeY = 0;
        GardenItem[,] matrix;
        List<GardenItem> gamesList;

        public GameCollection(int sizeX, int sizeY, List<GardenItem> gamesList)
        {
            this.gamesList = gamesList;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            matrix = new GardenItem[sizeX,sizeY];
            FillMatrix();
        }

        private void FillMatrix()
        {
            foreach (GardenItem gi in gamesList)
            {
                //if (matrix[gi.PositionX, gi.PositionY] != null)
                //    throw new Exception("Multiple games at same position in GameCollection - Gamelib data error");
                matrix[gi.PositionX, gi.PositionY] = gi;
            }
        }

        /// <summary>
        /// add a new GardenItem to the collection at position (gi.PositionX,gi.PositionY)
        /// </summary>
        /// <param name="gi"></param>
        public void Add(GardenItem gi) {
            matrix[gi.PositionX,gi.PositionY] = gi;
            gamesList.Add(gi);
        }

        public int Count
        {
            get
            {
                return gamesList.Count;
            }
        }

        public List<GardenItem> GetItemsAround(Vector2 pos, float range)
        {
            return GetItemsAround((int)Math.Round(pos.X), (int)Math.Round(pos.Y), (int)Math.Round(range));
        }

        public List<GardenItem> GetItemsAround(int x, int y, int range)
        {
            int x1 = x - range;
            int x2 = x + range;
            int y1 = y - range;
            int y2 = y + range;
            if (x1 < 0) x1 = 0;
            if (x2 < 0) x2 = 0;
            if (y1 < 0) y1 = 0;
            if (y2 < 0) y2 = 0;
            if (x1 >= sizeX) x1 = sizeX - 1;
            if (x2 >= sizeX) x2 = sizeX - 1;
            if (y1 >= sizeY) y1 = sizeY - 1;
            if (y2 >= sizeY) y2 = sizeY - 1;
            List<GardenItem> l = new List<GardenItem>();
            for (int ix = x1; ix < x2; ix++)
            {
                for (int iy = y1; iy < y2; iy++)
                {
                    GardenItem g = matrix[ix, iy];
                    if (g!=null)
                        l.Add(g);
                }
            }
            return l;
        }

        public List<GardenItem> AsList()
        {
            return gamesList;
        }

        public void Dispose()
        {
            foreach (GardenItem g in gamesList)
            {
                g.Dispose();
            }
            gamesList.Clear();
            matrix = new GardenItem[0,0];
        }

        /// <summary>
        /// find the game closest to given position, if any
        /// </summary>
        /// <param name="pos">index position (x,y)</param>
        /// <returns>found GardenItem close to that position or null if none are found near</returns>
        public GardenItem FindGameAt(Vector2 pos)
        {
            int x = (int) Math.Round(pos.X);
            int y = (int)Math.Round(pos.Y);
            if (x >= 0 && x < sizeX && y >=0 && y < sizeY) {
                return matrix[x, y];
            }
            return null;
        }

        public GardenItem FindGameNamed(string gameID)
        {
            foreach (GardenItem gi in gamesList)
            {
                if (gi.GameID.Equals(gameID))
                    return gi;
            }
            return null;
        }

        /*
        public GardenItem GetRandomInstalledGame()
        {
            GardenItem g = null;
            do
            {
                int n = (int)RandomMath.RandomBetween(0, this.Count - 1.0f);
                g = this[n];
            } while (g.IsSystemPackage || !g.IsGrowable || !g.IsInstalled || !g.IsPlayable);
            return g;
        }
         */
    
    }
}
