// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /**
     * a cursor hovering over game thumbnails to select them
     */
    public class GameThumbnailCursor: EffectSpritelet
    {
        public MotionBehavior MotionB;
        public Vector2 GridPosition = Vector2.Zero;        

        public GameThumbnailCursor()
            : base("cursor2","GameThumbnailCursor")
        {
            DrawInfo.LayerDepth = 0.95f;
            MotionB = new MotionBehavior();
            Add(MotionB);
        }

        /// <summary>
        /// checks whether a Gamelet is in selection distance of this cursor
        /// </summary>
        /// <param name="g">gamelet to check this cursor against</param>
        /// <returns>true if in range</returns>
        public bool GameletInRange(Gamelet g)
        {
            float d = (g.Motion.Position - this.Motion.Position).Length();
            if (d <= GardenGamesPanel.CURSOR_DISCOVERY_RANGE)
                return true;
            return false;

        }

        /// <summary>
        /// set cursor to select a given game. It will move there in next Update()s.
        /// </summary>
        /// <param name="g"></param>
        public void SetToGame(GardenItem g)
        {
            MotionB.Target = g.Position;
            MotionB.TargetSpeed = 4f;
            GridPosition = g.Position;
        }

    }
}
