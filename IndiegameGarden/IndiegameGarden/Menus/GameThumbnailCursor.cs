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
        /// <param name="g"></param>
        /// <returns></returns>
        public bool GameletInRange(Gamelet g)
        {
            float d = (g.Position - this.Position).Length();
            if (d < 0.3)
                return true;
            return false;

        }

        /// <summary>
        /// set cursor to select a given game
        /// </summary>
        /// <param name="g"></param>
        public void SetToGame(IndieGame g)
        {
            Target = g.Position;
            TargetSpeed = 4f;
        }

    }
}
