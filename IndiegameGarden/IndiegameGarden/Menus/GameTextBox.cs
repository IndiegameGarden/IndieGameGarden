// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /**
     * a box showing a text (which can be changed/updated) with a shadow border effect
     */
    public class GameTextBox: Drawlet
    {
        string txt ;
        SpriteFont font;

        /// <summary>
        /// construct a box without text yet and given font set
        /// </summary>
        public GameTextBox(string fontName)
        {
            txt = "";
            Init(fontName);
        }

        /// <summary>
        /// get/set the text of this box
        /// </summary>
        public string Text
        {
            get
            {
                return txt;
            }
            set
            {
                txt = value;
            }
        }

        private void Init(string fontName)
        {
            font = GardenGame.Instance.Content.Load<SpriteFont>(fontName);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            Vector2 origin = Vector2.Zero; // new Vector2(2f * txt.Length, 0f);
            Vector2 pos = Motion.DrawPosition;
            // draw shadow
            Color shadowDrawColor = Color.Black;
            shadowDrawColor.A = DrawInfo.DrawColor.A;
            MySpriteBatch.DrawString(font, txt, pos + new Vector2(1f,1f), shadowDrawColor,
                                    Motion.RotateAbs, origin, Motion.ScaleAbs, SpriteEffects.None, DrawInfo.LayerDepth + 0.00001f);
            MySpriteBatch.DrawString(font, txt, pos, DrawInfo.DrawColor,
                                    Motion.RotateAbs, origin, Motion.ScaleAbs, SpriteEffects.None, DrawInfo.LayerDepth);
        }
    }
}
