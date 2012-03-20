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
        SampleSoundEvent currentSong = null;
        SampleSoundEvent oldSong = null;

        public GardenMusic()
        {
            soundScript = new SoundEvent("GardenMusic");
            rp.Ampl = 0;
            FadeIn();
            Play("Content\\aurelic.ogg", 0.5);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (oldSong != null)
            {
                oldSong.Amplitude -= fadeSpeed * p.Dt;
                if (oldSong.Amplitude <= 0)
                {
                    oldSong.Active = false;
                    oldSong = null;
                }
            }

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
            
            // advance time only if volume nonzero
            if (rp.Ampl > 0 )
                rp.Time += p.Dt;
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

        /// <summary>
        /// change music to another music track
        /// </summary>
        /// <param name="musicFile">filename of a .wav or .ogg music file to play</param>
        public void Play(string musicFile, double volume)
        {
            if (currentSong != null)  // if needed, fade out a current playing song
            {
                oldSong = currentSong;
            }
            currentSong = new SampleSoundEvent(musicFile);
            currentSong.Amplitude = volume;
            soundScript.AddEvent(rp.Time + 0.3, currentSong);
        }
    }
}
