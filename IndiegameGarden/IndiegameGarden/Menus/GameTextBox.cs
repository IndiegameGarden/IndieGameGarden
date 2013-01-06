// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
        public ColorChangeBehavior ColorB;

        string txt ;
        SpriteFont font;

        /// <summary>
        /// construct a box without text yet and given font set
        /// </summary>
        public GameTextBox(string fontName)
        {
            ColorB = new ColorChangeBehavior();
            Add(ColorB);
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
            Vector2 pos = Motion.PositionAbsZoomedPixels;
            // draw shadow
            Color shadowDrawColor = Color.Black;
            shadowDrawColor.A = DrawInfo.DrawColor.A;

            // scaling with resolutions
            //             descriptionBox.Motion.Scale = ((float)Screen.WidthPixels) / 1440f; // 768f / ((float)Screen.HeightPixels); // +(((float)Screen.WidthPixels) - 1440f) / 1440f;
            float sc = Motion.ScaleAbs; // *((float)Screen.WidthPixels) / 1440f; // HACK
            float scRatio = (float) (Screen.AspectRatio / 1.6f);
            Vector2 vScale = new Vector2( sc * scRatio, sc );
            try
            {
                MySpriteBatch.DrawString(font, txt, pos + new Vector2(1f, 1f), shadowDrawColor,
                                        Motion.RotateAbs, origin, vScale, SpriteEffects.None, DrawInfo.LayerDepth + 0.00001f); // TODO the const
                MySpriteBatch.DrawString(font, txt, pos, DrawInfo.DrawColor,
                                        Motion.RotateAbs, origin, vScale, SpriteEffects.None, DrawInfo.LayerDepth);
            }
            catch (Exception)
            {
                ;  // e.g. if character to draw not available. // TODO
            }
        }
    }
}
