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
        bool isFadeOut = false;
        bool isFadeIn = false;
        double fadeSpeed = 0.5;

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

            if (isFadeIn)
            {
                rp.Ampl += fadeSpeed * p.Dt;
                if (rp.Ampl >= 1)
                {
                    rp.Ampl = 1;
                    isFadeIn = false;
                }
            }

            if (isFadeOut)
            {
                rp.Ampl -= fadeSpeed * p.Dt;
                if (rp.Ampl <= 0)
                {
                    rp.Ampl = 0;
                    isFadeOut = false;
                }
            }
            
            rp.Time = SimTime;
            MusicEngine.GetInstance().Render(soundScript, rp);
        }

        public void FadeOut()
        {
            isFadeOut = true;
            isFadeIn = false;
        }

        public void FadeIn()
        {
            isFadeOut = false;
            isFadeIn = true;
        }
    }
}
