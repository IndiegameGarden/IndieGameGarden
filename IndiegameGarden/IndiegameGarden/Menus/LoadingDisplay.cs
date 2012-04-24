using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

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
        const float LEFT_POSITION = 0.10f;

        GameTextBox tbox;
        GameTextBox iggNameBox;
        GameTextBox helpTextBox;
        GardenItem game;
        Spritelet gameIcon;
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
                loadingDisplay.iggNameBox.ColorB.Intensity = 0f;
                loadingDisplay.iggNameBox.ColorB.FadeTarget = 0f;
                g.SimTime = 0f;
                if (loadingDisplay.gameIcon != null)
                    loadingDisplay.gameIcon.Visible = true;
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
                isFirstDraw = true;
                g.SimTime = 0f;
                if (loadingDisplay.gameIcon != null)
                    loadingDisplay.gameIcon.Visible = true;
            }

            public override void OnUpdate(Gamelet g)
            {
                // fade in 
                loadingDisplay.iggNameBox.ColorB.FadeTarget = 1.0f;

                if (g.SimTime <= 1f || isFirstDraw)
                {
                    loadingDisplay.tbox.Text = "Playing " + loadingDisplay.game.Name;
                }
                else 
                {
                    // suppress drawing during play of another game - save resources and avoid gfx conflicts.
                    GardenGame.Instance.SuppressDraw();
                }
                if (g.SimTime > TIME_SHOW_PLAYING_MESSAGE)
                {
                    g.SetNextState(new StateLoadingDisplay_Empty(loadingDisplay));
                }
            }

            public override void OnDraw(Gamelet g)
            {
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
                g.SimTime = 0f;
                loadingDisplay.tbox.Text = "";
            }

            public override void OnUpdate(Gamelet g)
            {
                if (g.SimTime > 1f && !isFirstDraw)
                {
                    // suppress drawing during play of another game - save resources and avoid gfx conflicts.
                    GardenGame.Instance.SuppressDraw();
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
            tbox.Text = "Loading ...";
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(LEFT_POSITION, 0.05f); // TODO consts
            Add(tbox);

            iggNameBox = new GameTextBox("GameDescriptionFont");
            iggNameBox.Text = "Indiegame Garden        Exit this game to return to the garden";
            iggNameBox.Motion.Position = new Microsoft.Xna.Framework.Vector2(LEFT_POSITION, 0.94f);
            iggNameBox.Motion.Scale = 0.7f;
            iggNameBox.DrawInfo.DrawColor = Color.Transparent;
            iggNameBox.ColorB.Intensity = 0f;
            iggNameBox.ColorB.FadeTarget = 0.0f;
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

    }
}
