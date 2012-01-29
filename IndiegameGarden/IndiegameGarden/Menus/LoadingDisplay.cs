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
        const float TIME_SHOW_PLAYING_MESSAGE = 4.0f;

        GameTextBox tbox;
        GameTextBox iggNameBox;
        IndieGame game;
        float nextStateTimer = -1f;

        /// <summary>
        /// base class for my state classes
        /// </summary>
        class LoadingDisplayState : State
        {
            public LoadingDisplay loadingDisplay;

            public LoadingDisplayState(LoadingDisplay parent)
            {
                this.loadingDisplay = parent;
            }
        }

        /// <summary>
        /// state where 'loading' message is displayed
        /// </summary>
        class StateLoadingDisplay_Loading : LoadingDisplayState {

            public StateLoadingDisplay_Loading(LoadingDisplay parent)
                : base(parent)
            { }

            public override void OnEntry(Gamelet g)
            {
                g.SimTime = 0f;
            }
            public override void OnUpdate(Gamelet g)
            {
                int phase = (int)Math.Round((g.SimTime*2f) % 3.0f);
                string t = "Loading " + loadingDisplay.game.Name;
                switch (phase)
                {
                    case 0: t += " .";
                        break;
                    case 1: t += " ..";
                        break;
                    case 2: t += " ...";
                        break;
                }
                loadingDisplay.tbox.Text = t;
                
                if (loadingDisplay.nextStateTimer >= 0f && g.SimTime > loadingDisplay.nextStateTimer)
                {
                    loadingDisplay.nextStateTimer = -1f;                    
                    g.SetNextState(new StateLoadingDisplay_Playing(loadingDisplay));
                }

            }
        }

        /// <summary>
        /// state where 'playing' is displayed
        /// </summary>
        class StateLoadingDisplay_Playing : LoadingDisplayState
        {
            bool isFirstDraw = true;

            public StateLoadingDisplay_Playing(LoadingDisplay parent)
                : base(parent)
            { }

            public override void OnEntry(Gamelet g)
            {
                g.SimTime = 0f;
            }
            public override void OnUpdate(Gamelet g)
            {
                if (isFirstDraw)
                {
                    isFirstDraw = false;
                    loadingDisplay.tbox.Text = "Playing " + loadingDisplay.game.Name;
                }
                else if (!isFirstDraw)
                {
                    GardenGame.Instance.SuppressDraw();
                }
                if (g.SimTime > TIME_SHOW_PLAYING_MESSAGE)
                {
                    g.SetNextState(new StateLoadingDisplay_Empty(loadingDisplay));
                }

            }
        }

        /// <summary>
        /// state where empty screen is shown
        /// </summary>
        class StateLoadingDisplay_Empty : LoadingDisplayState
        {
            public StateLoadingDisplay_Empty(LoadingDisplay parent)
                : base(parent)
            { }


            public override void OnEntry(Gamelet g)
            {
                loadingDisplay.tbox.Text = ""; 
            }
        }

        public LoadingDisplay()
        {
            SetNextState(new StateLoadingDisplay_Loading(this));
            tbox = new GameTextBox("Loading ...");
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(0.05f, 0.05f);
            Add(tbox);

            iggNameBox = new GameTextBox("Indiegame Garden               Exit game to return!");
            iggNameBox.Motion.Position = new Microsoft.Xna.Framework.Vector2(0.05f, 0.92f);
            iggNameBox.Motion.Scale = 0.6f;
            Add(iggNameBox);
        }

        /// <summary>
        /// set state to loading game and display given game name
        /// </summary>
        /// <param name="g">game whose name/info to display while loading</param>
        public void SetLoadingGame(IndieGame g)
        {
            SetNextState(new StateLoadingDisplay_Loading(this));
            game = g;
        }

        /// <summary>
        /// use to set display from Loading state to Playing state after specified time in seconds. Works only
        /// once per state transition.
        /// </summary>
        /// <param name="afterTime">after which time from now to change msg from 'loading' to 'playing'</param>
        public void SetPlayingGame(float afterTime)
        {
            if (nextStateTimer < 0f)
                nextStateTimer = SimTime + afterTime;
        }

        /// <summary>
        /// check if display is in loading state currently
        /// </summary>
        /// <returns>true if loading state</returns>
        public bool IsLoadingState()
        {
            return IsInState(new StateLoadingDisplay_Loading(this));
        }

        /// <summary>
        /// check if display is in playing state currently
        /// </summary>
        /// <returns>true if playing state</returns>
        public bool IsPlayingState()
        {
            return IsInState(new StateLoadingDisplay_Playing(this));
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
