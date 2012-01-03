// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;
using IndiegameGarden.Store;

namespace IndiegameGarden.Menus
{
    /**
     * a box showing a text (which can be changed/updated)
     */
    public class GameTextBox: Gamelet
    {
        string txt ;
        SpriteFont font;

        /// <summary>
        /// construct a box with initial text. Can be changed later with the Text property.
        /// </summary>
        /// <param name="initialText">initial text to display</param>
        public GameTextBox(string initialText)
        {
            txt = initialText;
            Init();
        }

        /// <summary>
        /// construct a box without text yet (empty)
        /// </summary>
        public GameTextBox()
        {
            txt = "";
            Init();
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

        private void Init()
        {
            font = GardenGame.Instance.Content.Load<SpriteFont>("m41_lovebit");
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            Vector2 origin = Vector2.Zero; // new Vector2(2f * txt.Length, 0f);
            Screen.UseSharedSpritebatch().DrawString(font, txt, DrawPosition, DrawColor, RotateAbs, origin, ScaleAbs, SpriteEffects.None, LayerDepth);
        }
    }
}
