using System;
using System.Collections.Generic;
using System.Linq;
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
        string txt = null;
        SpriteFont font;

        public GameTextBox(string initialText)
        {
            txt = initialText;
            Init();
        }

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
            font = GardenMain.Instance.Content.Load<SpriteFont>("m41_lovebit");
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            if (txt == null)
            {
                txt = "";
            }
            Vector2 origin = Vector2.Zero; // new Vector2(2f * txt.Length, 0f);
            Screen.UseSharedSpritebatch().DrawString(font, txt, DrawPosition, DrawColor, RotateAbs, origin, ScaleAbs, SpriteEffects.None, LayerDepth);
        }
    }
}
