// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

namespace IndiegameGarden.Menus
{
    /**
     * Behavior to let a Gamelet smoothly move towards a given target, and to change
     * Scale smoothly to a target.
     */
    public class MotionBehavior: Gamelet
    {
        /// <summary>
        /// sets a target position for cursor to move towards
        /// </summary>
        public Vector2 Target = Vector2.Zero;

        /// <summary>
        /// speed for moving towards Target
        /// </summary>
        public float TargetSpeed = 0f;

        /// <summary>
        /// set target for Scale
        /// </summary>
        public float ScaleTarget = 1.0f;

        /// <summary>
        /// speed for scaling towards ScaleTarget
        /// </summary>
        public float ScaleSpeed = 0f;

        protected override void OnNewParent()
        {
            base.OnNewParent();
            Motion = Parent.Motion;
            DrawInfo = Parent.DrawInfo;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // motion towards target
            Motion.Velocity = (Target - Motion.Position) * TargetSpeed;

            // handle scaling over time
            ScaleToTarget(ScaleTarget, ScaleSpeed, 0.01f);

        }

        // scaling logic during OnUpdate()
        private void ScaleToTarget(float targetScale, float spd, float spdMin)
        {
            if (Motion.Scale < targetScale)
            {
                Motion.Scale += spdMin + spd * (targetScale - Motion.Scale); //*= 1.01f;
                if (Motion.Scale > targetScale)
                {
                    Motion.Scale = targetScale;
                }
            }
            else if (Motion.Scale > targetScale)
            {
                Motion.Scale += -spdMin + spd * (targetScale - Motion.Scale); //*= 1.01f;
                if (Motion.Scale < targetScale)
                {
                    Motion.Scale = targetScale;
                }
            }
            DrawInfo.LayerDepth = 0.8f - Motion.Scale / 1000.0f;
        }

    }
}
