﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// provides color/intensity fading features to the parent Gamelet
    /// </summary>
    public class ColorChangeBehavior: Gamelet
    {
        protected float intensity = 1.0f;

        /// <summary>
        /// target for Fade value
        /// </summary>
        public float FadeTarget = 1.0f;

        /// <summary>
        /// speed of fading towards FadeTarget
        /// </summary>
        public float FadeSpeed = 0f;

        /// <summary>
        /// the intensity of displaying thumbnail (amount color/brightness) between 0f...1f (max)
        /// </summary>
        public float Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
                DrawInfo.DrawColor = new Color(intensity, intensity, intensity, intensity); //DrawInfo.DrawColor.A); // TODO allow drawcolor?
            }

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

        protected override void OnNewParent()
        {
            base.OnNewParent();
            DrawInfo = Parent.DrawInfo;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // handle fading over time
            if (FadeTarget > Intensity)
            {
                Intensity += FadeSpeed * p.Dt;
                if (FadeTarget < Intensity)
                    Intensity = FadeTarget;
                DrawInfo.Alpha = Intensity;
            }
            else if (FadeTarget < Intensity)
            {
                Intensity -= FadeSpeed * p.Dt;
                if (FadeTarget > Intensity)
                    Intensity = FadeTarget;
                DrawInfo.Alpha = Intensity;
            }
        }

    }
}
