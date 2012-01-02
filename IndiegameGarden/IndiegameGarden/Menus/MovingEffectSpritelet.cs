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
     * an EffectSpritelet with added functions for moving towards a target, gradual scaling, fading, intensity
     */
    public class MovingEffectSpritelet: EffectSpritelet
    {

        public MovingEffectSpritelet(): base((Texture2D)null,null)
        {
        }

        public MovingEffectSpritelet(string textureFile, string effectFile)
            : base(textureFile, effectFile)
        {
        }

        public MovingEffectSpritelet(Texture2D texture, string effectFile)
            : base(texture, effectFile)
        {
        }

        /// <summary>
        /// sets a target position for cursor to move towards
        /// </summary>
        public Vector2 Target = Vector2.Zero;
        public float TargetSpeed = 0f;
        public float FadeTarget = 1.0f;
        public float FadeSpeed = 0f;
        public float ScaleTarget = 1.0f;
        public float ScaleSpeed = 0f;


        public float Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
                DrawColor = new Color(intensity, intensity, intensity, DrawColor.A);
            }

        }

        protected float intensity = 1.0f;

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // motion towards target
            Velocity = (Target - Position) * TargetSpeed;

            // handle fading over time
            if (FadeTarget > Intensity)
            {
                Intensity += FadeSpeed * p.dt;
                Alpha = Intensity;
                if (FadeTarget < Intensity)
                    Intensity = FadeTarget;
            }
            else if (FadeTarget < Intensity)
            {
                Intensity -= FadeSpeed * p.dt;
                Alpha = Intensity;
                if (FadeTarget > Intensity)
                    Intensity = FadeTarget;
            }

            // handle scaling over time
            ScaleToTarget(ScaleTarget, ScaleSpeed, 0.01f);


        }

        private void ScaleToTarget(float targetScale, float spd, float spdMin)
        {
            if (this.Scale < targetScale)
            {
                this.Scale += spdMin + spd * (targetScale - this.Scale); //*= 1.01f;
                if (this.Scale > targetScale)
                {
                    this.Scale = targetScale;
                }
            }
            else if (this.Scale > targetScale)
            {
                this.Scale += -spdMin + spd * (targetScale - this.Scale); //*= 1.01f;
                if (this.Scale < targetScale)
                {
                    this.Scale = targetScale;
                }
            }
            this.LayerDepth = 0.8f - this.Scale / 1000.0f;
        }

        /// <summary>
        /// configure FadeTarget and FadeSpeed to fade to given value in given time duration
        /// </summary>
        /// <param name="fadeValue"></param>
        /// <param name="timeDuration"></param>
        public void FadeToTarget(float fadeValue, float timeDuration)
        {
            FadeTarget = fadeValue;
            FadeSpeed = Math.Abs((fadeValue - Intensity) / timeDuration);
        }


    }
}
