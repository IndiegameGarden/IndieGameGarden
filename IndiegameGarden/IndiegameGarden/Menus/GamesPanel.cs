// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using TTengine.Core;
using IndiegameGarden.Store;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// a parent class for panels displaying a list of games selectable by user
    /// </summary>
    public abstract class GamesPanel: Gamelet
    {
        public enum UserInput { LEFT, RIGHT, UP, DOWN, QUITTING, ABORT_QUITTING};

        // stores the current list obtained with UpdateList()
        protected List<IndieGame> gl = new List<IndieGame>();

        private IndieGame selectedGame = null;

        /// <summary>
        /// which game did the user just select, can be a random jump
        /// </summary>
        public IndieGame SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                if (value != selectedGame)
                    ChangedSelectedGameEvent(value, selectedGame);
                selectedGame = value;
            }
        }

        /// <summary>
        /// user action caused update of the games to be displayed
        /// </summary>
        /// <param name="gl">new list of games to use</param>
        public abstract void UpdateList(List<IndieGame> gl);

        /// <summary>
        /// user action triggered a changed selection event
        /// </summary>
        /// <param name="newSel">previously selected game</param>
        /// <param name="oldSel">newly selected game</param>
        public abstract void ChangedSelectedGameEvent(IndieGame newSel, IndieGame oldSel);

        /// <summary>
        /// user input to panel is directed to this method
        /// </summary>
        /// <param name="inp">category of user input</param>
        public abstract void SendUserInput(UserInput inp);

    }
}
