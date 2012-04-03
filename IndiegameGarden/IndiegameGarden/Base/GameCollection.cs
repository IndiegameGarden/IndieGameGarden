using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IndiegameGarden.Util;

namespace IndiegameGarden.Base
{
    /**
     * a selected collection/list of games
     */
    public class GameCollection: List<GardenItem>, IDisposable
    {

        public void Dispose()
        {
            foreach (GardenItem g in this)
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// find the game closest to given position
        /// </summary>
        /// <param name="pos">index position (x,y)</param>
        /// <returns>found IndieGame or null if none are found near</returns>
        public GardenItem FindGameAt(Vector2 pos)
        {
            GardenItem sel = null;
            float bestD = 999999;

            foreach (GardenItem g in this)
            {
                Vector2 v = pos - g.Position;
                float d = v.Length();
                if (d < bestD)
                {
                    sel = g;
                    bestD = d;
                }
            }
            // check distance limit
            if (bestD > 0.5)  // TODO set constant? relation to cursor selection range?
                return null;
            return sel;
        }

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
    
    }
}
