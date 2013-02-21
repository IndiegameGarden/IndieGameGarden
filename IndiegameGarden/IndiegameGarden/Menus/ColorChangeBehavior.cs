using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// provides color/intensity fading and alpha fading features to the parent Gamelet
    /// </summary>
    public class ColorChangeBehavior: Gamelet
    {
        protected float intensity = 1.0f;
        protected float saturation = 1.0f;

        /// <summary>
        /// target for ALpha value
        /// </summary>
        public float AlphaTarget = 1.0f;

        /// <summary>
        /// target for Saturation value 
        /// </summary>
        public float SaturationTarget = 1.0f;

        /// <summary>
        /// speed of fading towards FadeTarget
        /// </summary>
        public float FadeSpeed = 0f;

        /// <summary>
        /// the intensity of displaying thumbnail (amount color/brightness) between 0f...1f (max)
        /// </summary>
        public float Alpha
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
                if (DrawInfo != null)
                    DrawInfo.Alpha = intensity;
            }

        }

        /// <summary>
        /// the intensity of displaying thumbnail (amount color/brightness) between 0f...1f (max)
        /// </summary>
        public float Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                saturation = value;
                DrawInfo.R = saturation;
            }

        }

        /// <summary>
        /// configure FadeTarget and FadeSpeed to fade to given value in given time duration
        /// </summary>
        /// <param name="fadeValue"></param>
        /// <param name="timeDuration"></param>
        public void FadeAlphaToTarget(float fadeValue, float timeDuration)
        {
            AlphaTarget = fadeValue;
            FadeSpeed = Math.Abs((fadeValue - Alpha) / timeDuration);
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            DrawInfo = Parent.DrawInfo;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // handle fading over time
            if (AlphaTarget > Alpha)
            {
                Alpha += FadeSpeed * p.Dt;
                if (AlphaTarget < Alpha)
                    Alpha = AlphaTarget;
                DrawInfo.Alpha = Alpha;
            }
            else if (AlphaTarget < Alpha)
            {
                Alpha -= FadeSpeed * p.Dt;
                if (AlphaTarget > Alpha)
                    Alpha = AlphaTarget;
                DrawInfo.Alpha = Alpha;
            }
        
            // handle fading over time
            if (SaturationTarget > saturation)
            {
                saturation += FadeSpeed * p.Dt;
                if (SaturationTarget < saturation)
                    saturation = SaturationTarget;
                DrawInfo.R = saturation;
            }
            else if (SaturationTarget < Saturation)
            {
                saturation -= FadeSpeed * p.Dt;
                if (SaturationTarget > saturation)
                    Saturation = SaturationTarget;
                DrawInfo.R = saturation;
            }

        }

    }
}
