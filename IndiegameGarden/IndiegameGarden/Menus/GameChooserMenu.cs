// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TTengine.Core;
using TTengine.Modifiers;

using IndiegameGarden.Base;
using IndiegameGarden.Install;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// main menu to choose a game; uses a GamePanel to delegate thumbnail rendering and navigation to
    /// </summary>
    public class GameChooserMenu: Drawlet
    {
        public Spritelet background;

        /// <summary>
        /// UI constants
        /// </summary>
        const double MIN_MENU_CHANGE_DELAY = 0.2f; 
        
        GameCollection gamesList;

        float lastKeypressTime = 0;
        bool wasEscPressed = false;
        bool wasEnterPressed = false;
        // the game thumbnails or items selection panel
        GamesPanel panel;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        /// <summary>
        /// construct new menu
        /// </summary>
        public GameChooserMenu()
        {
            ActiveInState = new StateBrowsingMenu();
            panel = new GardenGamesPanel(this);
            panel.Motion.Position = new Vector2(0.0f, 0.0f);

            // get the items to display
            gamesList = GardenGame.Instance.GameLib.GetList();

            // background
            Spritelet bg = new Spritelet("parc-by-gadl.png");
            bg.Motion.Position = new Vector2(0.66667f, 0.22f);
            bg.DrawInfo.LayerDepth = 1f;
            //bg.DrawInfo.DrawColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            // bg color 169, 157, 241
            bg.Motion.Add(new MyFuncyModifier( delegate(float v) { return v/70.0f; }, "Rotate"));
            bg.Motion.Add(new MyFuncyModifier(delegate(float v) { return (0.45f + 0.75f * (float) Math.Sqrt(v/430.0)); }, "Scale"));
            bg.Motion.TargetPos = new Vector2(0.66667f, 0.9f);
            bg.Motion.TargetPosSpeed = 0.004f;
            Add(bg);
            background = bg;

            // logo
            Spritelet logo = new Spritelet("igglogo");
            Add(logo);
            logo.DrawInfo.Alpha = 0.7f;
            logo.Motion.Scale = 0.55f;
            logo.Motion.Position = new Vector2(Screen.AspectRatio - 0.24f, 0.04f);
            ColorChangeBehavior fadeIn = new ColorChangeBehavior();
            logo.Add(fadeIn);
            fadeIn.Intensity = 0f;
            fadeIn.FadeToTarget(0.9344f, 6f);
            logo.Motion.Add(new SineModifier("ScaleModifier", 0.03124f, 0.07344f, 1.0f));

            // set my panel and games list
            Add(panel);
            panel.OnUpdateList(gamesList);

        }

        /// <summary>
        /// handles all keyboard input into the menu, transforming that into events sent to GUI components
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
                panel.OnUserInput(GamesPanel.UserInput.STOP_EXIT);
            }

            if (!st.IsKeyDown(Keys.Enter) && wasEnterPressed)
            {
                wasEnterPressed = false;
                panel.OnUserInput(GamesPanel.UserInput.STOP_SELECT);
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
                    panel.OnUserInput(GamesPanel.UserInput.START_EXIT);
                }
                wasEscPressed = true;
            }

            // -- website launch key
            if (st.IsKeyDown(Keys.W) && !prevKeyboardState.IsKeyDown(Keys.W))
            {
                panel.OnUserInput(GamesPanel.UserInput.LAUNCH_WEBSITE);
            }

            // -- music togglekey
            if (st.IsKeyDown(Keys.M) && !prevKeyboardState.IsKeyDown(Keys.M))
            {
                panel.OnUserInput(GamesPanel.UserInput.TOGGLE_MUSIC);
            }

            // -- a navigation key is pressed - check keys and generate action(s)
            if (st.IsKeyDown(Keys.Left)) {
                panel.OnUserInput(GamesPanel.UserInput.LEFT);                
            }
            if (st.IsKeyDown(Keys.Right)) {
                panel.OnUserInput(GamesPanel.UserInput.RIGHT);
            }

            if (st.IsKeyDown(Keys.Up)) {
                panel.OnUserInput(GamesPanel.UserInput.UP);
            }

            if (st.IsKeyDown(Keys.Down)){
                panel.OnUserInput(GamesPanel.UserInput.DOWN);
            }

            if (st.IsKeyDown(Keys.Enter))
            {
                if (!wasEnterPressed)
                    panel.OnUserInput(GamesPanel.UserInput.START_SELECT);
                wasEnterPressed = true;
            }

            // (time) bookkeeping for next keypress
            lastKeypressTime = p.SimTime;
            prevKeyboardState = st;
        }
       
        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // check keyboard inputs from user
            KeyboardControls(ref p);
        }

    }
}
