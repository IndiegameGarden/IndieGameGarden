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
    public class MotionBehaviorXX: Gamelet
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

        // zoom, scale etc. related vars for panel
        public float ZoomTarget = 1.0f;

        public float ZoomSpeed = 0f;

        public Motion ZoomCenterTarget = null;

        public float RotateTarget = 0f;

        public float RotateSpeed = 0f;

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
            ScaleToTarget(ScaleTarget, ScaleSpeed, ScaleSpeed * 0.01f);

            // handle dynamic zooming
            ZoomToTarget(ref p);

            // rotation
            RotateToTarget();
        }

        private void ZoomToTarget(ref UpdateParams p)
        {
            // handle dynamic zooming
            if (ZoomSpeed > 0f)
            {
                // handle zoom value
                if (Motion.Zoom < ZoomTarget)
                {
                    Motion.Zoom *= (1.0f + ZoomSpeed);
                    if (Motion.Zoom > ZoomTarget)
                        Motion.Zoom = ZoomTarget;
                }
                else if (Motion.Zoom > ZoomTarget)
                {
                    Motion.Zoom /= (1.0f + ZoomSpeed);
                    if (Motion.Zoom < ZoomTarget)
                        Motion.Zoom = ZoomTarget;
                }

                // handle zoom center moving
                if (!Motion.ZoomCenter.Equals(ZoomCenterTarget))
                {
                    Vector2 vdif = ZoomCenterTarget.PositionAbs - Motion.ZoomCenter;
                    float vel = 1000.0f * ZoomSpeed * vdif.Length();
                    if (vel < ZoomSpeed * 100.0f)
                        vel = ZoomSpeed * 100.0f;
                    Vector2 vmove = vdif;
                    vmove.Normalize();
                    vmove *= vel * p.Dt;
                    if (vmove.LengthSquared() > vdif.LengthSquared())
                    {
                        // target reached
                        Motion.ZoomCenter = ZoomCenterTarget.PositionAbs; // FIXME abs?
                    }
                    else
                    {
                        Motion.ZoomCenter += vmove;
                    }
                }

            }
        }

        private void RotateToTarget()
        {
            // handle dynamic zooming
            if (RotateSpeed > 0f)
            {
                if (Motion.Rotate < RotateTarget)
                {
                    Motion.Rotate += (RotateSpeed);
                    if (Motion.Rotate > RotateTarget)
                        Motion.Rotate = RotateTarget;
                }
                else if (Motion.Rotate > RotateTarget)
                {
                    Motion.Rotate -= (RotateSpeed);
                    if (Motion.Rotate < RotateTarget)
                        Motion.Rotate = RotateTarget;
                }
            }
        }

        // scaling logic during OnUpdate()
        private void ScaleToTarget(float targetScale, float spd, float spdMin)
        {
            if (spd > 0)
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
                if (DrawInfo != null)
                    DrawInfo.LayerDepth = 0.8f - Motion.Scale / 1000.0f;
            }
        }

    }
}
