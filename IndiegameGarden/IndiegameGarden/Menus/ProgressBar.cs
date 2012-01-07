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
     * a graphical progress bar between 0 and 100% with textual indication on the side
     */
    public class ProgressBar: Spritelet
    {
        float progressValue;
        float progressValueTarget;
        float progressCatchupSpeed = 50.0f; // TODO make public?
        SpriteFont spriteFont;
        Color textColor = Color.White;

        public ProgressBar()
            : base("birch-test")
        {
            progressValue = 0f;
            progressValueTarget = 0f;
            spriteFont = TTengineMaster.ActiveGame.Content.Load<SpriteFont>("m41_lovebit");
        }

        public float Progress
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

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // move nrg level towards the target
            if (progressValue < progressValueTarget)
            {
                progressValue += progressCatchupSpeed * p.dt;
                if (progressValue > progressValueTarget)
                    progressValue = progressValueTarget;
            }
            if (progressValue > progressValueTarget)
            {
                progressValue -= progressCatchupSpeed * p.dt;
                if (progressValue < progressValueTarget)
                    progressValue = progressValueTarget;
            }

        }

        protected override void OnDraw(ref DrawParams p)
        {
            Vector2 pos = DrawPosition;
            int width = 1 + (int) Math.Round(WidthAbs * progressValue );
            if (width > Texture.Width) width = Texture.Width;

            Rectangle srcRect = new Rectangle(0, 0, width, Texture.Height);
            Screen.UseSharedSpritebatch().Draw(Texture, pos, srcRect, DrawColor,
                    this.RotateAbs, Vector2.Zero, 1.0f, SpriteEffects.None, LayerDepth);

            // plot text percentage
            Vector2 tpos = pos + new Vector2(width, Texture.Height / 2.0f - 10.0f) ;
            Screen.UseSharedSpritebatch().DrawString(spriteFont, String.Format("{0,3}%", Math.Round(progressValue)), tpos, textColor);
        }

    }

}
