// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
using System;
using System.Collections.Generic;
using System.Text;

using TTengine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace IndiegameGarden.Menus
{
    /**
     * a graphical progressContributionSingleFile bar between 0 and 100% with textual indication on the side.
     * The bar is configured to move progressContributionSingleFile up only towards a target. The current value
     * can be reset with ProgressValue if progressContributionSingleFile needs to move down or be reset to 0.
     */
    public class ProgressBar: Spritelet
    {
        bool isPulsing = false;
        float progressValue;
        float progressSpeed = 0f;
        float progressValueTarget;
        SpriteFont spriteFont;
        Color textColor = Color.White;
        float textScale = 1f;

        public ProgressBar()
            : base("birch-progress-bar")
        {
            progressValue = 0f;
            progressValueTarget = 0f;
            ProgressCatchupSpeed = 0.6f;
            spriteFont = TTengineMaster.ActiveGame.Content.Load<SpriteFont>("m41_lovebit");
            Motion.Scale = 0.6f;
        }

        /// <summary>
        /// Pulsing is an animation showing activity eg a download is in progress/active.
        /// </summary>
        public bool Pulsing
        {
            get
            {
                return isPulsing;
            }
            set
            {
                isPulsing = value;
            }
        }

        public float ProgressTarget
        {
            get
            {
                return progressValueTarget;
            }
            set
            {
                progressValueTarget = value;
            }
        }

        public float ProgressSpeed
        {
            get
            {
                return progressSpeed;
            }
            set
            {
                progressSpeed = value;
            }
        }

        public float ProgressCatchupSpeed { get; set; }

        public float ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // move level towards the target
            if (progressValue < progressValueTarget)
            {
                progressValue += ProgressCatchupSpeed * p.Dt;
                if (progressValue > progressValueTarget)
                    progressValue = progressValueTarget;
            }

            // pulsing
            textScale = 1f;
            if (isPulsing)
            {
                float ampl = 0.273f;
                float frequency = 0.2f;// +progressSpeed * 0.0000001f; //0.6243f;
                textScale = 1f + ampl * (float)Math.Sin(MathHelper.TwoPi * (double)frequency * SimTime);

            }
        }

        protected override void OnDraw(ref DrawParams p)
        {
            Vector2 pos = DrawInfo.DrawPosition;
            double progressValuePercent = 100 * progressValue;
            float drawSc = DrawInfo.DrawScale;
            int width = 1 + (int)Math.Round(ToPixels(DrawInfo.WidthAbs) * progressValue);
            int height = (int) Math.Round(ToPixels(DrawInfo.HeightAbs));
            if (width > Texture.Width) width = Texture.Width;

            Rectangle srcRect = new Rectangle(0, 0, width, Texture.Height-2);
            MySpriteBatch.Draw(Texture, pos, srcRect, DrawInfo.DrawColor,
                            Motion.RotateAbs, new Vector2(0f,height/4), drawSc, SpriteEffects.None, DrawInfo.LayerDepth);

            // plot text percentage
            Vector2 tpos = pos + new Vector2(width * drawSc, height/4); //Texture.Height / 2.0f - 10.0f) ;
            Vector2 origin = new Vector2(15f,6f);
            MySpriteBatch.DrawString(spriteFont, String.Format(" {0,3}%", Math.Round(progressValuePercent)), tpos, 
                                     textColor, Motion.RotateAbs, origin, textScale * drawSc * 1.2f, SpriteEffects.None, DrawInfo.LayerDepth);
        }

    }

}
