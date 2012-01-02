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

using IndiegameGarden.Store;
using IndiegameGarden.Install;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// main menu to choose a game; uses a GamePanel to delegate rendering to
    /// </summary>
    public class GameChooserMenu: Gamelet
    {
        const int POINTER_UNDEFINED = -1;
        GameCollection gamesList;
        IndieGame gameLastLaunched = null;
        float lastKeypressTime = 0;
        double timeEscapeIsPressed = 0;
        double timeEnterIsNotPressed = 9999;
        int timesEnterPressed = 0;
        // to launch/start a game and track its state
        GameLauncher launcher;
        // game thumbnails or items selection panel
        GamesPanel panel;        
        // box showing info of a game such as title
        GameInfoBox infoBox;

        // below are UI configuration values
        public const double MIN_MENU_CHANGE_DELAY = 0.2f;
        public static Vector2 INFOBOX_SHOWN_POSITION = new Vector2(0.05f,0.85f);
        public static Vector2 INFOBOX_HIDDEN_POSITION = new Vector2(0.05f, 0.95f);
        public const float INFOBOX_SPEED_MOVE = 2.8f;

        public GameChooserMenu()
        {
            panel = new GardenGamesPanel();
            panel.Position = new Vector2(0.0f, 0.0f);

            // get the items to display
            gamesList = GardenMain.Instance.gameLibrary.GetList();

            // set my panel and games list
            Add(panel);
            panel.UpdateList(gamesList);

            // info box
            infoBox = new GameInfoBox();
            infoBox.Position = INFOBOX_HIDDEN_POSITION;
            Add(infoBox);

            // background
            Spritelet bg = new Spritelet("flower");
            bg.Position = new Vector2(0.66667f, 0.5f);
            bg.DrawColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            bg.Add(new MyFuncyModifier( delegate(float v) { return v/20.0f; }, "Rotate"));
            Add(bg);

        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            if (timeEscapeIsPressed > 0)
                Screen.DebugText(new Vector2(0f, 0.1f), "ESC is pressed");
        }

        protected void KeyboardControls(ref UpdateParams p)
        {
            KeyboardState st = Keyboard.GetState();
            // time bookkeeping
            float timeSinceLastKeypress = p.simTime - lastKeypressTime;

            if (!st.IsKeyDown(Keys.Enter))
                timeEnterIsNotPressed += p.dt;
            else
                timeEnterIsNotPressed = 0f;
            
            if (st.IsKeyDown(Keys.Escape))
            {
                // if escape was pressed...
                panel.SendUserInput(GamesPanel.UserInput.QUITTING);
                timeEscapeIsPressed += p.dt;
                if (timeEscapeIsPressed > 0.5f)
                    GardenMain.Instance.Exit();
            }
            else if (timeEscapeIsPressed > 0f)
            {
                // if ESC was released just now... before quitting
                timeEscapeIsPressed = 0f;
                panel.SendUserInput(GamesPanel.UserInput.ABORT_QUITTING);
            }

            // check - only proceed if a key pressed and some minimal delay has passed...            
            if (timeSinceLastKeypress < MIN_MENU_CHANGE_DELAY)
                return ;
            if (st.GetPressedKeys().Length == 0)
                return;
            
            // -- a key is pressed - check all keys and take action(s)
            if (st.IsKeyDown(Keys.Left))
                panel.SendUserInput(GamesPanel.UserInput.LEFT);

            else if (st.IsKeyDown(Keys.Right))
                panel.SendUserInput(GamesPanel.UserInput.RIGHT);

            else if (st.IsKeyDown(Keys.Up))
                panel.SendUserInput(GamesPanel.UserInput.UP);

            else if (st.IsKeyDown(Keys.Down))
                panel.SendUserInput(GamesPanel.UserInput.DOWN);

            if (st.IsKeyDown(Keys.Enter))
            {
                timesEnterPressed++;
                if (timesEnterPressed == 1)
                {
                    panel.SendUserInput(GamesPanel.UserInput.SELECT1);
                    infoBox.Target = INFOBOX_SHOWN_POSITION;
                    infoBox.TargetSpeed = INFOBOX_SPEED_MOVE;
                    infoBox.SetGameInfo(panel.SelectedGame);
                }
                else if (timesEnterPressed >= 2)
                {
                    panel.SendUserInput(GamesPanel.UserInput.SELECT2);

                    // check if download needed
                    IndieGame g = panel.SelectedGame;
                    if (!g.IsInstalled)
                    {
                        g.dlAndInstallTask = new GameDownloadAndInstallTask(g);
                        g.dlAndInstallTask.Start();
                    }
                    else
                    {
                        // if installed, launch it
                        if (launcher == null || launcher.IsDone() == true)
                        {
                            if (timeEnterIsNotPressed > 1.5f) // only launch if enter was released for some time
                            {
                                if (g.ExeFile.Length > 0)
                                {
                                    launcher = new GameLauncher(g);
                                    gameLastLaunched = panel.SelectedGame;
                                    launcher.Start();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                // if some key is pressed but not Enter, reset the timesEnter count
                timesEnterPressed = 0;
                infoBox.Target = INFOBOX_HIDDEN_POSITION;
            }

            // bookkeeping for next keypress
            lastKeypressTime = p.simTime;
            gameLastLaunched = null; // reset the memory of last launched upon keypress
            

        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            KeyboardControls(ref p);

            // update text box
            infoBox.SetGameInfo(panel.SelectedGame);

        }

    }
}
