using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /**
     * Drawlet that contains all contents of the game loading screen, including
     * text and animated graphics.
     */
    public class LoadingDisplay: Drawlet 
    {
        GameTextBox tbox;
        bool isFirstDraw = true;
        IndieGame game;
        bool showLoadingMsg = true;

        public LoadingDisplay()
        {            
            tbox = new GameTextBox("Loading ...");
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(0.15f, 0.15f);
            Add(tbox);
        }

        public void SetGame(IndieGame g)
        {
            game = g;
            tbox.Text = "Loading " + g.Name + " ...";
            showLoadingMsg = true;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (showLoadingMsg)
            {
                int phase = (int) Math.Round(SimTime % 3.0f);
                string t = "Loading " + game.Name;
                switch (phase)
                {
                    case 0: t += " .";
                        break;
                    case 1: t += " ..";
                        break;
                    case 2: t += " ...";
                        break;
                }
                tbox.Text = t;
            }

            if (isFirstDraw && !showLoadingMsg)
            {
                isFirstDraw = false;
            }
            else if (!isFirstDraw)
            {
                // disable text drawing after first draw where nothing happens on screen.
                GardenGame.Instance.SuppressDraw();
            }

        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

        }



    }
}
