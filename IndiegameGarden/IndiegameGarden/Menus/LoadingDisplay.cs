using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        const float LEFT_POSITION = 0.15f;
        const double MIN_MENU_CHANGE_DELAY = 0.2f;
        const double MAX_KEY_PRESSING_TIME = 3f;
        const float TIME_ESC_PRESS_TO_EXIT = 1.8f;
        const float SCALE_AT_LOADING_START = 0.95f;

        GameTextBox tbox;
        GameTextBox iggNameBox;
        GameTextBox helpTextBox;
        GardenItem game;
        Spritelet gameIcon;
        float nextStateTimer = -1f;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        float lastKeypressTime = 0;
        bool wasEscPressed = false;
        bool wasEnterPressed = false;
        bool isExiting = false;
        bool willExitSoon = false;
        float timeExiting = 0f;

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
                base.OnEntry(g);
                loadingDisplay.willExitSoon = false;
                loadingDisplay.isExiting = false;
                loadingDisplay.iggNameBox.ColorB.Alpha = 0f;
                loadingDisplay.iggNameBox.ColorB.AlphaTarget = 0f;
                if (loadingDisplay.gameIcon != null)
                    loadingDisplay.gameIcon.Visible = true;
            }
            public override void OnUpdate(Gamelet g, ref UpdateParams p)
            {
                base.OnUpdate(g, ref p);
                int phase = (int)Math.Round((SimTime*2f) % 3.0f);
                string t = ""; // "Loading \"" + loadingDisplay.game.Name + "\"";
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
                
                if (loadingDisplay.nextStateTimer >= 0f && SimTime > loadingDisplay.nextStateTimer)
                {
                    loadingDisplay.nextStateTimer = -1f;                    
                    g.SetNextState(new StateLoadingDisplay_Playing(loadingDisplay));
                }

                loadingDisplay.helpTextBox.Text = loadingDisplay.game.HelpText;

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
                base.OnEntry(g);
                isFirstDraw = true;
                if (loadingDisplay.gameIcon != null)
                    loadingDisplay.gameIcon.Visible = true;
            }

            public override void OnUpdate(Gamelet g, ref UpdateParams p)
            {
                base.OnUpdate(g, ref p);
                // fade in 
                loadingDisplay.iggNameBox.ColorB.AlphaTarget = 1.0f;

                if (SimTime <= 1f || isFirstDraw)
                {
                    loadingDisplay.tbox.Text = ""; //   Enjoy \"" + loadingDisplay.game.Name + "\"";
                }
                else 
                {
                    // suppress drawing during play of another game - save resources and avoid gfx conflicts.
                    BentoGame.Instance.SuppressDraw();
                }
                if (SimTime > TIME_SHOW_PLAYING_MESSAGE)
                {
                    g.SetNextState(new StateLoadingDisplay_Empty(loadingDisplay));
                }

                // check keyboard - if esc, get back to garden state                
                if (loadingDisplay.isExiting){
                    loadingDisplay.timeExiting += p.Dt;
                    if (loadingDisplay.timeExiting > TIME_ESC_PRESS_TO_EXIT)
                    {
                        loadingDisplay.willExitSoon = true;
                    }
                }

                // perform real exit operation (abort launcher task) when ESC already released
                if (!loadingDisplay.isExiting && loadingDisplay.willExitSoon)
                {
                    BentoGame.Instance.launcher.Abort();
                    loadingDisplay.willExitSoon = false;
                    //to do: // do not progress to next global state until user has released esc button? or do not consider that as a new ESC press (even better)
                }
            }

            public override void OnDraw(Gamelet g)
            {
                base.OnDraw(g);
                isFirstDraw = false;
            }

        }

        /// <summary>
        /// state where empty screen is shown
        /// </summary>
        class StateLoadingDisplay_Empty : LoadingDisplayState
        {
            bool isFirstDraw = true;

            public StateLoadingDisplay_Empty(LoadingDisplay parent)
                : base(parent)
            { }

            public override void OnEntry(Gamelet g)
            {
                base.OnEntry(g);
                loadingDisplay.tbox.Text = "";
            }

            public override void OnUpdate(Gamelet g, ref UpdateParams p)
            {
                base.OnUpdate(g, ref p);
                if (SimTime > 1f && !isFirstDraw)
                {
                    // suppress drawing during play of another game - save resources and avoid gfx conflicts.
                    BentoGame.Instance.SuppressDraw();
                }

                // check keyboard - if esc, get back to garden state                
                if (loadingDisplay.isExiting)
                {
                    loadingDisplay.timeExiting += p.Dt;
                    if (loadingDisplay.timeExiting > TIME_ESC_PRESS_TO_EXIT)
                    {
                        loadingDisplay.willExitSoon = true;
                    }
                }

                // perform real exit operation (abort launcher task) when ESC released
                if (!loadingDisplay.isExiting && loadingDisplay.willExitSoon)
                {
                    BentoGame.Instance.launcher.Abort();
                    loadingDisplay.willExitSoon = false;
                }

            }

            public override void OnDraw(Gamelet g)
            {
                isFirstDraw = false;
            }

        }

        public LoadingDisplay()
        {
            tbox = new GameTextBox("m41_lovebit");
            tbox.Text = "";
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(LEFT_POSITION, 0.05f); // TODO consts
            Add(tbox);

            iggNameBox = new GameTextBox("GameDescriptionFont");
            iggNameBox.Text = "Glorious Wreckz Garden        Exit current game or hold ESC to return to the garden";
            iggNameBox.Motion.Position = new Microsoft.Xna.Framework.Vector2(LEFT_POSITION, 0.94f);
            iggNameBox.Motion.Scale = 0.75f;
            iggNameBox.DrawInfo.DrawColor = new Color(245,245,245,0);
            iggNameBox.ColorB.Alpha = 0f;
            iggNameBox.ColorB.AlphaTarget = 0.0f;
            iggNameBox.ColorB.FadeSpeed = 1.0f;
            Add(iggNameBox);

            helpTextBox = new GameTextBox("GameDescriptionFont");
            helpTextBox.Text = "";
            helpTextBox.Motion.Position = new Microsoft.Xna.Framework.Vector2(LEFT_POSITION, 0.83f);
            helpTextBox.Motion.Scale = 1.0f;
            Add(helpTextBox);

            gameIcon = new Spritelet();
            //gameIcon.Motion.Position = new Vector2( Screen.Width*0.8f , 0.2f);
            gameIcon.Motion.Position = Screen.Center;
            gameIcon.DrawInfo.LayerDepth = 1f;
            gameIcon.DrawInfo.DrawColor = Color.Gray;
            Add(gameIcon);

            // setup initial state
            SetNextState(new StateLoadingDisplay_Loading(this));
        }

        /// <summary>
        /// set state to loading game and display given game name
        /// </summary>
        /// <param name="g">game whose name/info to display while loading</param>
        public void SetLoadingGame(GardenItem g, GameThumbnail thumb)
        {
            SetNextState(new StateLoadingDisplay_Loading(this));
            Motion.Scale = SCALE_AT_LOADING_START;
            Motion.ScaleTarget = SCALE_AT_LOADING_START;
            game = g;
            gameIcon.Texture = thumb.Texture;
            //gameIcon.Motion.Scale = thumb.Motion.Scale * 1.4f * g.ScaleIcon;
            gameIcon.Motion.Scale = Screen.Width / gameIcon.DrawInfo.Width;

        }

        /// <summary>
        /// use to set display from Loading state to Playing state after specified time in seconds. Works only
        /// once per state transition.
        /// </summary>
        /// <param name="afterTime">after which time from now to change msg from 'loading' to 'playing'</param>
        public void SetPlayingGame(float afterTime)
        {
            if (nextStateTimer < 0f)
                nextStateTimer = afterTime;
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

        public void OnUserInput(GamesPanel.UserInput inp)
        {
            if (inp == GamesPanel.UserInput.START_EXIT)
            {
                isExiting = true;
                timeExiting = 0f;
                willExitSoon = false;
            }
            if (inp == GamesPanel.UserInput.STOP_EXIT)
            {
                isExiting = false;
                timeExiting = 0f;
            }

        }

        /// <summary>
        /// handles all keyboard input into the loading screen
        /// </summary>
        /// <param name="p">UpdateParams from TTEngine OnUpdate()</param>
        protected void KeyboardControls(ref UpdateParams p)
        {
            KeyboardState st = Keyboard.GetState();

            // time bookkeeping
            float timeSinceLastKeypress = p.SimTime - lastKeypressTime;

            // -- check all relevant key releases
            if (!st.IsKeyDown(Keys.Escape) && wasEscPressed)
            {
                wasEscPressed = false;
                OnUserInput(GamesPanel.UserInput.STOP_EXIT);
            }

            if (st.IsKeyDown(Keys.Enter) && wasEnterPressed && timeSinceLastKeypress > MAX_KEY_PRESSING_TIME )
            {
                int a = 3;
                //OnUserInput(GamesPanel.UserInput.STOP_SELECT);
                //lastKeypressTime = p.SimTime; // assume next round gonna be a new keypress
            }

            // for new keypresses - only proceed if a key pressed and some minimal delay has passed...            
            if (timeSinceLastKeypress < MIN_MENU_CHANGE_DELAY)
                return;
            // if no keys pressed, skip further checks
            if (st.GetPressedKeys().Length == 0)
            {
                prevKeyboardState = st;
                return;
            }


            // -- esc key
            if (st.IsKeyDown(Keys.Escape))
            {
                if (!wasEscPressed)
                {
                    OnUserInput(GamesPanel.UserInput.START_EXIT);
                }
                wasEscPressed = true;
            }

            // -- website launch key
            if (st.IsKeyDown(Keys.W) && !prevKeyboardState.IsKeyDown(Keys.W))
            {
                OnUserInput(GamesPanel.UserInput.LAUNCH_WEBSITE);
            }

            // -- music togglekey
            if (st.IsKeyDown(Keys.M) && !prevKeyboardState.IsKeyDown(Keys.M))
            {
                OnUserInput(GamesPanel.UserInput.TOGGLE_MUSIC);
            }

            // -- a navigation key is pressed - check keys and generate action(s)
            if (st.IsKeyDown(Keys.Left))
            {
                OnUserInput(GamesPanel.UserInput.LEFT);
            }
            if (st.IsKeyDown(Keys.Right))
            {
                OnUserInput(GamesPanel.UserInput.RIGHT);
            }

            if (st.IsKeyDown(Keys.Up))
            {
                OnUserInput(GamesPanel.UserInput.UP);
            }

            if (st.IsKeyDown(Keys.Down))
            {
                OnUserInput(GamesPanel.UserInput.DOWN);
            }

            if (st.IsKeyDown(Keys.Enter))
            {
                if (!wasEnterPressed)
                    OnUserInput(GamesPanel.UserInput.START_SELECT);
                wasEnterPressed = true;
            }

            // (time) bookkeeping for next keypress
            lastKeypressTime = p.SimTime;
            prevKeyboardState = st;
        }

        public void ExitSoon()
        {
            willExitSoon = true;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (BentoGame.Instance.IsActive)
            {
                KeyboardControls(ref p);
            }
            if (isExiting)
            {
                Motion.ScaleTarget = 1f;
                if (timeExiting > 0.29f)
                    Motion.ScaleTarget = 1f - (timeExiting / TIME_ESC_PRESS_TO_EXIT) * 0.2f;
                Motion.ScaleSpeed = 0.005f;
            }
            else
            {
                Motion.ScaleTarget = 1f;
                Motion.ScaleSpeed = 0.006f;
            }
        }
    }
}
