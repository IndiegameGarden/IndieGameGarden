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
        float stateDuration = 0f;
        float simTime = 0f;
        bool fadeInMusicWhenDone = true;

        public StatePlayingGame(): base()
        {
        }

        public StatePlayingGame(float forDuration, bool fadeInMusicWhenDone)
            : base()
        {
            stateDuration = forDuration;
            this.fadeInMusicWhenDone = fadeInMusicWhenDone;
        }

        public override void OnEntry(Gamelet g)
        {
            base.OnEntry(g);
            GardenGame.Instance.IsMouseVisible = true;
        }

        public override void OnUpdate(Gamelet g, ref UpdateParams p)
        {
            base.OnUpdate(g, ref p);
            simTime += p.Dt;
            if (stateDuration > 0f)
            {
                if (simTime > stateDuration)
                    GardenGame.Instance.TreeRoot.SetNextState(new StateBrowsingMenu());
            }
        }

        public override void OnExit(Gamelet g)
        {
            base.OnExit(g);
            GardenGame.Instance.IsMouseVisible = false;
            if (fadeInMusicWhenDone)
                GardenGame.Instance.music.FadeIn();
        }
    }

}
