using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IndiegameGarden.Store
{
    /**
     * a selected collection/list of games
     */
    public class GameCollection: List<IndieGame>
    {

        /// <summary>
        /// find the game closest to given position
        /// </summary>
        /// <param name="pos">index position (x,y)</param>
        /// <returns>found IndieGame or null if none are found near</returns>
        public IndieGame FindGameAt(Vector2 pos)
        {
            IndieGame sel = null;
            float bestD = 999999;

            foreach (IndieGame g in this)
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
    }
}
