using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TTengine.Core;
using TTMusicEngine;
using TTMusicEngine.Soundevents;

namespace IndiegameGarden.Menus
{
    public class GardenMusic: Gamelet
    {
        SoundEvent soundScript;
        RenderParams rp = new RenderParams();

        public GardenMusic()
        {
            soundScript = new SoundEvent("GardenMusic");
            SampleSoundEvent evSong = new SampleSoundEvent("aurelic.ogg");
            evSong.Amplitude = 0.5f;
            soundScript.AddEvent(0, evSong);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            rp.Time = SimTime;
            MusicEngine.GetInstance().Render(soundScript, rp);


        }
    }
}
