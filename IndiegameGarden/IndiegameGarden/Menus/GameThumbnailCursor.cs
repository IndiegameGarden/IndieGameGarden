// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Store;

namespace IndiegameGarden.Menus
{
    public class GameThumbnailCursor: EffectSpritelet
    {
        /// <summary>
        /// sets a target position for cursor to move towards
        /// </summary>
        public Vector2 Target = Vector2.Zero;
        public Vector2 GridPosition = Vector2.Zero;
        public float TargetSpeed = 0f;

        public GameThumbnailCursor()
            : base("WhiteTexture","GameThumbnailCursor")
        {
            LayerDepth = 0f;
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            // motion towards target
            Velocity = (Target - Position) * TargetSpeed;
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
        /// configure cursor to travel towards given target position with given speed. Movement
        /// is not linear but is first-order controlled by distance to target
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="spd"></param>
        public void MoveToTarget(Vector2 pos, float spd)
        {
            Target = pos;
            TargetSpeed = spd;
        }

        /// <summary>
        /// set cursor to select a given game
        /// </summary>
        /// <param name="g"></param>
        public void SetToGame(IndieGame g)
        {
            MoveToTarget(g.Position, 4f);
        }

    }
}
