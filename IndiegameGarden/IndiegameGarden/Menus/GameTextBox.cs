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
    public class GameTextBox: Gamelet
    {
        IndieGame game;
        SpriteFont font;

        public GameTextBox()
        {
            font = GardenMain.Instance.Content.Load<SpriteFont>(@"TTDebugFont");
        }

        public void Update(IndieGame g)
        {
            game = g;
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
            if (game != null)
            {
                string txt = game.GameID;
                Vector2 origin = new Vector2(2f * txt.Length, 0f);
                Screen.UseSharedSpritebatch().DrawString(font, txt, DrawPosition, DrawColor, RotateAbs, origin, 3f, SpriteEffects.None, LayerDepth);
            }
        }
    }
}
