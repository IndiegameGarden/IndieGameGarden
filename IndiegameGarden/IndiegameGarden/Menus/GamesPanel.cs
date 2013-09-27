// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TTengine.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// a parent class for any panels displaying a list of games selectable by user
    /// </summary>
    public abstract class GamesPanel: Drawlet
    {

        public enum UserInput { LEFT, RIGHT, UP, DOWN, START_EXIT, STOP_EXIT, 
                                START_SELECT, STOP_SELECT, POSITION_SELECT, MOUSE_OVER, LAUNCH_WEBSITE, TOGGLE_MUSIC };

        // stores the current list obtained with OnUpdateList()
        protected GameCollection gl = null;

        // stores currently selected game
        private GardenItem selectedGame = null;

        /// <summary>
        /// which game did the user last select, note this can be a random jump
        /// </summary>
        public GardenItem SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                if (value != selectedGame)
                    OnChangedSelectedGame(value, selectedGame);
                selectedGame = value;
            }
        }

        // stores the currently launched game, or null if none running/launched.
        public GardenItem LaunchedGame = null;

        public GamesPanel()
        {
        }

        /// <summary>
        /// user action caused update of the games to be displayed
        /// </summary>
        /// <param name="gl">new list of games to use</param>
        public abstract void OnUpdateList(GameCollection gl);

        /// <summary>
        /// user action triggered a changed selection event
        /// </summary>
        /// <param name="newSel">previously selected game</param>
        /// <param name="oldSel">newly selected game</param>
        public abstract void OnChangedSelectedGame(GardenItem newSel, GardenItem oldSel);

        /// <summary>
        /// user input to the panel is directed to this method
        /// </summary>
        /// <param name="inp">category of user input from the enum</param>
        public abstract void OnUserInput(UserInput inp);

        /// <summary>
        /// user input in selecting items on screen is directed to this method
        /// </summary>
        /// <param name="inp">category of user input from the enum</param>
        /// <param name="pointerPos">pointer position input by user</param>
        public abstract void OnUserInput(GamesPanel.UserInput inp, Vector2 pointerPos);
    }
}
