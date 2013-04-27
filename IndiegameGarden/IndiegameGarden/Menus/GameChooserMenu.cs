// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
        const double MIN_MENU_CHANGE_DELAY = 0.3f; 
        
        GameCollection gamesList;

        float lastKeypressTime = 0;
        bool wasEscPressed = false;
        bool wasEnterPressed = false;
        bool wasMouseButPressed = false;
        // the game thumbnails or items selection panel
        GamesPanel panel;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        Vector2 pointerPos, lastPointerPos;

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
            Spritelet bg = new Spritelet("wee2");
            bg.Motion.Position = new Vector2(0.68f, 0.425f);
            bg.DrawInfo.LayerDepth = 1f;
            //bg.DrawInfo.DrawColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            // bg color 169, 157, 241
            bg.Motion.Add(new MyFuncyModifier( delegate(float v) { return v/44.7f; }, "Rotate"));
            bg.Motion.Add(new MyFuncyModifier(delegate(float v) { return (10.5f + 0.95f * (float) Math.Sqrt(v/430.0)); }, "Scale"));
            bg.Motion.TargetPos = new Vector2(Screen.Width/2.0f + 0.1f, 0.75f);
            bg.Motion.TargetPosSpeed = 0.004f;
            Add(bg);
            background = bg;

            // logo that floats in top right
            Spritelet logo = new Spritelet("gtlogo");
            Add(logo);
            logo.DrawInfo.Alpha = 0.0f;
            logo.Motion.Scale = 0.7f;
            logo.Motion.Position = new Vector2(Screen.AspectRatio - 0.24f, 0.06f);
            //ColorChangeBehavior fadeIn = new ColorChangeBehavior();
            //logo.Add(fadeIn);
            //fadeIn.Alpha = 0f;
            //fadeIn.AlphaTarget = 0f;
            //fadeIn.FadeAlphaToTarget(0.9344f, 6f);
            logo.Motion.Add(new SineModifier("ScaleModifier", 0.06124f, 0.07144f, 1.0f));

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

            if (st.IsKeyDown(Keys.Enter) && wasEnterPressed && timeSinceLastKeypress > 2.7f)
            {
                //wasEnterPressed = false;
                //panel.OnUserInput(GamesPanel.UserInput.STOP_SELECT);
            }

            
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
                    lastKeypressTime = p.SimTime;
                }
                wasEscPressed = true;
            }

            // -- website launch key
            if (st.IsKeyDown(Keys.W) && !prevKeyboardState.IsKeyDown(Keys.W))
            {
                panel.OnUserInput(GamesPanel.UserInput.LAUNCH_WEBSITE);
                lastKeypressTime = p.SimTime;
            }

            // -- music togglekey
            if (st.IsKeyDown(Keys.M) && !prevKeyboardState.IsKeyDown(Keys.M))
            {
                panel.OnUserInput(GamesPanel.UserInput.TOGGLE_MUSIC);
                lastKeypressTime = p.SimTime;
            }

            // -- a navigation key is pressed - check keys and generate action(s)
            if (st.IsKeyDown(Keys.Left)) {
                panel.OnUserInput(GamesPanel.UserInput.LEFT);
                lastKeypressTime = p.SimTime;
            }
            if (st.IsKeyDown(Keys.Right)) {
                panel.OnUserInput(GamesPanel.UserInput.RIGHT);
                lastKeypressTime = p.SimTime;
            }

            if (st.IsKeyDown(Keys.Up)) {
                panel.OnUserInput(GamesPanel.UserInput.UP);
                lastKeypressTime = p.SimTime;
            }

            if (st.IsKeyDown(Keys.Down)){
                panel.OnUserInput(GamesPanel.UserInput.DOWN);
                lastKeypressTime = p.SimTime;
            }

            if (st.IsKeyDown(Keys.Enter))
            {
                if (!wasEnterPressed)
                {
                    lastKeypressTime = p.SimTime;
                    panel.OnUserInput(GamesPanel.UserInput.START_SELECT);
                }
                wasEnterPressed = true;
            }
            
            prevKeyboardState = st;
        }

        protected void MouseControls(ref UpdateParams p)
        {
            MouseState st = Mouse.GetState();

            if (st.LeftButton == ButtonState.Released && wasMouseButPressed)
            {
                wasMouseButPressed = false;
                panel.OnUserInput(GamesPanel.UserInput.STOP_SELECT);
            }

            if (st.LeftButton == ButtonState.Pressed)
            {
                if (!wasMouseButPressed)
                {
                    lastPointerPos = pointerPos;
                    pointerPos = new Vector2(st.X, st.Y); 
                    panel.OnUserInput(GamesPanel.UserInput.POSITION_SELECT, pointerPos);
                    //if ((pointerPos - lastPointerPos).Length() < 30f)
                    //{
                     //   panel.OnUserInput(GamesPanel.UserInput.START_SELECT);
                    //}                    
                }
                wasMouseButPressed = true;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // check keyboard/mouse inputs from user
            if (GardenGame.Instance.IsActive)
            {
                KeyboardControls(ref p);
                MouseControls(ref p);
            }
        }

    }
}
