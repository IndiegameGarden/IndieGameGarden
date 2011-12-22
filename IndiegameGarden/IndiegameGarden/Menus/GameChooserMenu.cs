// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// main menu to choose a game; uses a GamePanel to delegate rendering to
    /// </summary>
    public class GameChooserMenu: Gamelet
    {
        const int POINTER_UNDEFINED = -1;
        List<IndieGame> gamesList;
        IndieGame gameLastLaunched = null;
        float lastChangeTime = 0;
        float timeSinceLastChange = 0;
        double timeEscapeIsDown = 0;
        double timeEnterIsUp = 9999;
        bool changedSelection = false;
        GameLauncher launcher;
        GamesPanel panel;
        GameTextBox textBox;

        const double MIN_MENU_CHANGE_DELAY = 0.2f;

        public GameChooserMenu()
        {
            panel = new GardenGamesPanel();
            panel.Position = new Vector2(0.3f, 0.15f);

            // get the items to display
            gamesList = GardenMain.Instance.gameLibrary.GetList();

            // set my panel and games list
            Add(panel);
            panel.UpdateList(gamesList);

            // background
            Spritelet bg = new Spritelet("flower");
            bg.Position = new Vector2(0.66667f, 0.5f);
            bg.DrawColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            bg.Add(new MyFuncyModifier( delegate(float v) { return v/20.0f; }, "Rotate"));
            Add(bg);

            textBox = new GameTextBox();
            textBox.Position = new Vector2(0.66667f, 0.9f);
            textBox.LayerDepth = 0f;
            Add(textBox);

        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            if (timeEscapeIsDown > 0)
                Screen.DebugText(new Vector2(0f, 0.1f), "ESC is pressed");
        }

        protected void KeyboardControls(ref UpdateParams p)
        {
            timeSinceLastChange = p.simTime - lastChangeTime;
            if (timeSinceLastChange > MIN_MENU_CHANGE_DELAY)
            {
                changedSelection = true;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    panel.SendUserInput(GamesPanel.UserInput.LEFT);

                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    panel.SendUserInput(GamesPanel.UserInput.RIGHT);

                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    panel.SendUserInput(GamesPanel.UserInput.UP);

                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    panel.SendUserInput(GamesPanel.UserInput.DOWN);
                else
                    changedSelection = false;
            }

            if (changedSelection)
            {
                lastChangeTime = p.simTime;
                gameLastLaunched = null; // reset the memory of last launched
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // if escape was pressed...
                panel.SendUserInput(GamesPanel.UserInput.QUITTING);
                timeEscapeIsDown += p.dt;
                if (timeEscapeIsDown > 0.5f)
                    GardenMain.Instance.Exit();
            }
            else if (timeEscapeIsDown > 0f)
            {
                // if ESC was released just now... before quitting
                timeEscapeIsDown = 0f;
                panel.SendUserInput(GamesPanel.UserInput.ABORT_QUITTING);
            }

            // launch the current selection (try it)
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (launcher == null || launcher.IsDone() == true)
                {
                    if (timeEnterIsUp > 1.5f) // only launch if enter was released for some time
                    {
                        IndieGame g = panel.SelectedGame;
                        if (g.ExeFile.Length > 0)
                        {
                            launcher = new GameLauncher(g);
                            gameLastLaunched = panel.SelectedGame;
                            launcher.Start();
                        }
                    }
                }
                timeEnterIsUp = 0f;
            }
            else
            {
                timeEnterIsUp += p.dt; 
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            changedSelection = false;
            base.OnUpdate(ref p);

            KeyboardControls(ref p);

            // update text box
            textBox.Update(panel.SelectedGame);

        }

    }
}
