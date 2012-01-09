// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using TTengine.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// a parent class for all panels displaying a list of games selectable by user
    /// </summary>
    public abstract class GamesPanel: Gamelet
    {
        public enum UserInput { LEFT, RIGHT, UP, DOWN, QUITTING, ABORT_QUITTING, SELECT0, SELECT1, SELECT2 };

        // stores the current list obtained with OnUpdateList()
        protected GameCollection gl = new GameCollection();

        protected IndieGame selectedGame = null;

        /// <summary>
        /// which game did the user last select, note this can be a random jump
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
                    OnChangedSelectedGame(value, selectedGame);
                selectedGame = value;
            }
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
        public abstract void OnChangedSelectedGame(IndieGame newSel, IndieGame oldSel);

        /// <summary>
        /// user input to the panel is directed to this method
        /// </summary>
        /// <param name="inp">category of user input from the enum</param>
        public abstract void OnUserInput(UserInput inp);

    }
}
