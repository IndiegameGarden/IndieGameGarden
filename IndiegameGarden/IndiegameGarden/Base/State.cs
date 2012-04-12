using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TTengine.Core;

namespace IndiegameGarden.Base
{
    /**
     * State of browsing main menu, no active game
     */
    public class StateBrowsingMenu: State
    {
        public override void OnEntry(Gamelet g)
        {
            GardenGame.Instance.IsMouseVisible = false;
        }
    }

    /**
     * state of game playing, no menu shown
     */
    public class StatePlayingGame : State
    {
        public override void OnEntry(Gamelet g)
        {
            GardenGame.Instance.IsMouseVisible = true;
        }

        public override void OnExit(Gamelet g)
        {
            GardenGame.Instance.IsMouseVisible = false;
            GardenGame.Instance.music.FadeIn();
        }
    }

}
