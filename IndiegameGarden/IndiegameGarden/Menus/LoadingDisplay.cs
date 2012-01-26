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
        IndieGame game;

        class StateLoadingDisplay_Loading : State {
            public override void OnEntry(Gamelet g)
            {
                g.SimTime = 0f;
            }
            public override void OnUpdate(Gamelet g)
            {
                int phase = (int)Math.Round(g.SimTime % 3.0f);
                string t = "Loading " + ((LoadingDisplay)g).game.Name;
                switch (phase)
                {
                    case 0: t += " .";
                        break;
                    case 1: t += " ..";
                        break;
                    case 2: t += " ...";
                        break;
                }
                ((LoadingDisplay)g).tbox.Text = t;

                if (g.SimTime > 4f)
                {
                    g.SetNextState(new StateLoadingDisplay_Playing());
                }

            }
        }

        class StateLoadingDisplay_Playing : State {
            bool isFirstDraw = true;

            public override void OnEntry(Gamelet g)
            {
                g.SimTime = 0f;
            }
            public override void OnUpdate(Gamelet g)
            {
                if (isFirstDraw)
                {
                    isFirstDraw = false;
                    ((LoadingDisplay)g).tbox.Text = "Playing " + ((LoadingDisplay)g).game.Name;
                }
                else if (!isFirstDraw)
                {
                    GardenGame.Instance.SuppressDraw();
                }
                if (g.SimTime > 4f)
                {
                    g.SetNextState(new StateLoadingDisplay_Empty());
                }

            }
        }

        class StateLoadingDisplay_Empty   : State {
            public override void OnEntry(Gamelet g)
            {
                ((LoadingDisplay)g).tbox.Text = "";
            }
        }

        public LoadingDisplay()
        {
            SetNextState(new StateLoadingDisplay_Loading());
            tbox = new GameTextBox("Loading ...");
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(0.05f, 0.05f);
            Add(tbox);
        }

        public void SetLoadingGame(IndieGame g)
        {
            SetNextState(new StateLoadingDisplay_Loading());
            game = g;
        }

        public void SetPlayingGame()
        {
            SetNextState(new StateLoadingDisplay_Playing());
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }



    }
}
