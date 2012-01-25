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
        float progressValue;
        float progressValueTarget;
        SpriteFont spriteFont;
        Color textColor = Color.White;

        public ProgressBar()
            : base("birch-test")
        {
            progressValue = 0f;
            progressValueTarget = 0f;
            ProgressCatchupSpeed = 0.6f;
            spriteFont = TTengineMaster.ActiveGame.Content.Load<SpriteFont>("m41_lovebit");
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

        }

        protected override void OnDraw(ref DrawParams p)
        {
            Vector2 pos = DrawInfo.DrawPosition;
            double progressValuePercent = 100 * progressValue;
            int width = 1 + (int)Math.Round(ToPixels(DrawInfo.WidthAbs) * progressValue);
            if (width > Texture.Width) width = Texture.Width;

            Rectangle srcRect = new Rectangle(0, 0, width, Texture.Height);
            MySpriteBatch.Draw(Texture, pos, srcRect, DrawInfo.DrawColor,
                            Motion.RotateAbs, Vector2.Zero, 1.0f, SpriteEffects.None, DrawInfo.LayerDepth);

            // plot text percentage
            Vector2 tpos = pos + new Vector2(width, Texture.Height / 2.0f - 10.0f) ;
            MySpriteBatch.DrawString(spriteFont, String.Format("{0,3}%", Math.Round(progressValuePercent)), tpos, textColor);
        }

    }

}
