// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
        /// calculate the distance from cursor to given GameThumbnail in the grid
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public float DistanceTo(GameThumbnail g)
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
