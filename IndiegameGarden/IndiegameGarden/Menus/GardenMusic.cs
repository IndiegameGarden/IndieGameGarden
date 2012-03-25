using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Base;
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
        double fadeSpeed = 0.3;
        SampleSoundEvent currentSong = null;
        List<SampleSoundEvent> oldSongs = new List<SampleSoundEvent>();
        Object songChangeLock = new Object();

        public GardenMusic()
        {
            soundScript = new SoundEvent("GardenMusic");            
            rp.Ampl = 1;            
            Play("Content\\aurelic.ogg", 0.5);
            FadeIn();
        }

        /// <summary>
        /// An ITask to load new music from file, in background
        /// </summary>
        public class MusicLoader : Task
        {
            GardenMusic parent;
            string musicFile;
            double volume;
            bool isAbort = false;

            public MusicLoader(GardenMusic parent, string musicFile, double volume)
                : base()
            {
                this.parent = parent;
                this.musicFile = musicFile;
                this.volume = volume;
            }

            protected override void StartInternal()
            {
                if (isAbort) return;
                SampleSoundEvent ev = new SampleSoundEvent(musicFile);
                ev.Amplitude = volume;
                parent.soundScript.AddEvent(parent.rp.Time + 0.3, ev);
                if (isAbort) return;

                lock (parent.songChangeLock)
                {
                    if (parent.currentSong != null)  // if needed, fade out a current playing song
                    {
                        parent.oldSongs.Add(parent.currentSong);
                    }
                    parent.currentSong = ev;
                }
            }

            protected override void AbortInternal()
            {
                isAbort = true;
            }

        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            lock (songChangeLock)
            {
                List<SampleSoundEvent> songsToRemove = new List<SampleSoundEvent>();
                foreach (SampleSoundEvent ev in oldSongs)
                {
                    ev.Amplitude -= fadeSpeed * p.Dt;

                    // remove songs from list if completely faded out.
                    if (ev.Amplitude <= 0)
                    {
                        ev.Active = false;
                        songsToRemove.Add(ev);
                    }
                }
                foreach (SampleSoundEvent ev in songsToRemove)
                {
                    oldSongs.Remove(ev);
                }

                if (currentSong != null)
                {

                    if (isFadeIn)
                    {
                        currentSong.Amplitude += fadeSpeed * p.Dt;
                        if (currentSong.Amplitude >= 1)
                        {
                            currentSong.Amplitude = 1;
                            isFadeIn = false;
                        }
                    }

                    if (isFadeOut)
                    {
                        currentSong.Amplitude -= fadeSpeed * p.Dt;
                        if (currentSong.Amplitude <= 0)
                        {
                            currentSong.Amplitude = 0;
                            isFadeOut = false;
                        }
                    }

                    // advance time only if volume nonzero
                    if (currentSong.Amplitude > 0)
                        rp.Time += p.Dt;
                }
            }

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
            ITask t = new ThreadedTask(new MusicLoader(this, musicFile, volume));
            t.Start();
        }
    }
}
