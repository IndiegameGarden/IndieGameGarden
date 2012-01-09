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
    public class GameChooserMenu: Gamelet
    {
        GameCollection gamesList;
        IndieGame gameLastLaunched = null;
        float lastKeypressTime = 0;
        double timeEscapeIsPressed = 0;
        int timesEnterPressed = 0;
        // used to launch/start a game and track its state
        GameLauncherTask launcher;
        // the game thumbnails or items selection panel
        GamesPanel panel;        
        // box showing info of a game such as title and download progress
        GameInfoBox infoBox;

        // below are UI configuration values
        const double MIN_MENU_CHANGE_DELAY = 0.2f;
        static Vector2 INFOBOX_SHOWN_POSITION = new Vector2(0.05f,0.85f);
        static Vector2 INFOBOX_HIDDEN_POSITION = new Vector2(0.05f, 0.95f);
        const float INFOBOX_SPEED_MOVE = 2.8f;

        /// <summary>
        /// construct new menu
        /// </summary>
        public GameChooserMenu(): base(new StateChooserMenu())
        {
            SetNextState(new StateChooserMenu());
            panel = new GardenGamesPanel();
            panel.Position = new Vector2(0.0f, 0.0f);

            // get the items to display
            gamesList = GardenGame.Instance.GameLib.GetList();

            // set my panel and games list
            Add(panel);
            panel.OnUpdateList(gamesList);

            // info box
            infoBox = new GameInfoBox();
            infoBox.Position = INFOBOX_HIDDEN_POSITION;
            Add(infoBox);

            // background
            Spritelet bg = new Spritelet("flower");
            bg.Position = new Vector2(0.66667f, 0.5f);
            bg.DrawColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            bg.Add(new MyFuncyModifier( delegate(float v) { return v/25.0f; }, "Rotate"));
            Add(bg);

        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            // DEBUG
            if (timeEscapeIsPressed > 0)
                Screen.DebugText(new Vector2(0f, 0.1f), "ESC is pressed");
        }

        /// <summary>
        /// handles all keyboard input into the menu
        /// </summary>
        /// <param name="p">UpdateParams from TTEngine OnUpdate()</param>
        protected void KeyboardControls(ref UpdateParams p)
        {
            KeyboardState st = Keyboard.GetState();
            // time bookkeeping
            float timeSinceLastKeypress = p.simTime - lastKeypressTime;

            // check esc key
            if (st.IsKeyDown(Keys.Escape))
            {
                // if escape was pressed...
                if (timesEnterPressed < 2)
                {
                    panel.OnUserInput(GamesPanel.UserInput.QUITTING);
                    if (timeEscapeIsPressed > 0.7f)
                        GardenGame.Instance.Exit();
                }
                else
                {
                    panel.OnUserInput(GamesPanel.UserInput.SELECT0);
                }
                timeEscapeIsPressed += p.dt;
            }
            else if (timeEscapeIsPressed > 0f)
            {
                // if ESC was released just now... before quitting
                timeEscapeIsPressed = 0f;
                panel.OnUserInput(GamesPanel.UserInput.ABORT_QUITTING);
            }

            // check - only proceed if a key pressed and some minimal delay has passed...            
            if (timeSinceLastKeypress < MIN_MENU_CHANGE_DELAY)
                return ;
            if (st.GetPressedKeys().Length == 0)
                return;
            
            // -- a key is pressed - check all keys and take action(s)
            bool navKey = false;
            if (st.IsKeyDown(Keys.Left)) {
                panel.OnUserInput(GamesPanel.UserInput.LEFT);
                navKey = true;
            }
            else if (st.IsKeyDown(Keys.Right)) {
                panel.OnUserInput(GamesPanel.UserInput.RIGHT);
                navKey = true;
            }

            else if (st.IsKeyDown(Keys.Up)) {
                panel.OnUserInput(GamesPanel.UserInput.UP);
                navKey = true;
            }

            else if (st.IsKeyDown(Keys.Down)){
                panel.OnUserInput(GamesPanel.UserInput.DOWN);
                navKey = true;
            }

            if (st.IsKeyDown(Keys.Enter))
            {
                timesEnterPressed++;
                if (timesEnterPressed == 1)
                {
                    panel.OnUserInput(GamesPanel.UserInput.SELECT1);
                    infoBox.Target = INFOBOX_SHOWN_POSITION;
                    infoBox.TargetSpeed = INFOBOX_SPEED_MOVE;
                    infoBox.SetGameInfo(panel.SelectedGame);
                }
                else if (timesEnterPressed >= 2)
                {
                    panel.OnUserInput(GamesPanel.UserInput.SELECT2);

                    InstallAndLaunchGame(panel.SelectedGame);
                }

            }
            else if (navKey)
            {
                // if some navigation key is pressed but not Enter, reset the timesEnter count
                timesEnterPressed = 0;
                infoBox.Target = INFOBOX_HIDDEN_POSITION;
            }

            // (time) bookkeeping for next keypress
            lastKeypressTime = p.simTime;
            gameLastLaunched = null; // reset the memory of last launched upon keypress

        }

        private void InstallAndLaunchGame(IndieGame g)
        {
            // check if download+install needed
            if (g.DlAndInstallTask==null && !g.IsInstalled)
            {
                g.DlAndInstallTask = new GameDownloadAndInstallTask(g);
                ITask taskThread = new ThreadedTask(g.DlAndInstallTask);
                taskThread.Start();
            }

            if (g.IsInstalled)
            {
                // if installed, then launch it if possible
                if (launcher == null || launcher.IsFinished() == true)
                {
                    SetNextState(new StatePlayingGame() );

                    launcher = new GameLauncherTask(g);
                    gameLastLaunched = panel.SelectedGame;
                    ThreadedTask taskThread = new ThreadedTask(launcher);
                    taskThread.TaskSuccessEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                    taskThread.TaskFailEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                    taskThread.Start();
                }
            }
        }

        // when a launched process concludes
        void taskThread_TaskFinishedEvent(object sender)
        {
            SetNextState(new StateChooserMenu() );
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // check keyboard inputs from user
            KeyboardControls(ref p);

            // TODO
            if (!Visible)
                GardenGame.Instance.SuppressDraw();

            // update text box with currently selected game info
            infoBox.SetGameInfo(panel.SelectedGame);

        }

    }
}
