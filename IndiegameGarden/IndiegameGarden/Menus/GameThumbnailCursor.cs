// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Store;

namespace IndiegameGarden.Menus
{
    /**
     * a cursor hovering over game thumbnails to select them
     */
    public class GameThumbnailCursor: MovingEffectSpritelet
    {
        public Vector2 GridPosition = Vector2.Zero;        

        public GameThumbnailCursor()
            : base("WhiteTexture","GameThumbnailCursor")
        {
            LayerDepth = 0f;
        }

        /// <summary>
        /// checks whether a Gamelet is in selection distance of this cursor
        /// </summary>
        /// <param name="g">gamelet to check this cursor against</param>
        /// <returns>true if in range</returns>
        public bool GameletInRange(Gamelet g)
        {
            float d = (g.Position - this.Position).Length();
            if (d < 0.3)
                return true;
            return false;

        }

        /// <summary>
        /// set cursor to select a given game. It will move there in next Update()s.
        /// </summary>
        /// <param name="g"></param>
        public void SetToGame(IndieGame g)
        {
            Target = g.Position;
            TargetSpeed = 4f;
        }

    }
}
