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
        public Vector2 GridPosition = Vector2.Zero;        

        public GameThumbnailCursor()
            : base("cursor2","GameThumbnailCursor")
        {
            DrawInfo.LayerDepth = 0.95f;
        }

        /// <summary>
        /// checks whether a Gamelet is in selection distance of this cursor
        /// </summary>
        /// <param name="g">gamelet to check this cursor against</param>
        /// <returns>true if in range</returns>
        public bool GameletInRange(GameThumbnail g)
        {
            if (GridDistanceTo(g) <= GardenGamesPanel.CURSOR_DISCOVERY_RANGE)
                return true;
            return false;

        }

        public bool GameletInFadeOutRange(GameThumbnail g)
        {
            if (GridDistanceTo(g) > GardenGamesPanel.CURSOR_DISCOVERY_RANGE)
                return true;
            return false;

        }

        public bool GameletOutOfRange(GameThumbnail g)
        {
            if (GridDistanceTo(g) > GardenGamesPanel.CURSOR_DESTRUCTION_RANGE) //TODO
                return true;
            return false;

        }

        public float GridDistanceTo(GameThumbnail g)
        {
            Vector2 v = g.Game.PositionXY - GridPosition;
            return v.Length();
        }

        /// <summary>
        /// set cursor to select a given game. It will move there in next Update()s.
        /// </summary>
        /// <param name="g"></param>
        public void SetToGame(GardenItem g)
        {
            Motion.TargetPos = g.Position;
            Motion.TargetPosSpeed = 4f; // TODO constant?
            GridPosition = g.Position;
        }

    }
}
