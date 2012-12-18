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
        public bool UserWantsMusic = false;

        SoundEvent soundScript;
        RenderParams rp = new RenderParams();
        bool isFadeOut = false;
        bool isFadeIn = false;
        double fadeSpeed = 0.3;
        SampleSoundEvent currentSong = null;
        double currentSongStartTime = 0;
        string lastMusicFile = null;
        List<SampleSoundEvent> oldSongs = new List<SampleSoundEvent>();
        Object songChangeLock = new Object();

        public GardenMusic()
        {
            soundScript = new SoundEvent("GardenMusic");            
            rp.Ampl = 1;
            PlayDefaultSong();
        }

        public bool IsFadedIn
        {
            get
            {
                return (currentSong != null) && (currentSong.Amplitude == 1);
            }
        }

        public void PlayDefaultSong()
        {
            Play( GardenGame.Instance.Content.RootDirectory + "\\Torley_Departuring.ogg", 0.5, 0f);
        }

        public void PlayLastSong()
        {
            if (lastMusicFile == null)
                PlayDefaultSong();
            else
                Play(lastMusicFile, 0.5, 0f);
        }

        public bool IsPlaying
        {
            get
            {
                return (currentSong != null) && (currentSong.Amplitude > 0);
            }
        }
        /// <summary>
        /// An ITask to load new music from file, in background
        /// </summary>
        public class MusicLoader : Task
        {
            GardenMusic parent;
            string musicFile;
            double volume;
            double musicStartTime;
            bool isAbort = false;

            public MusicLoader(GardenMusic parent, string musicFile, double volume, double musicStartTime)
                : base()
            {
                this.parent = parent;
                this.musicFile = musicFile;
                this.volume = volume;
                this.musicStartTime = musicStartTime;
            }

            protected override void StartInternal()
            {
                try
                {
                    if (isAbort)
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "Task aborted";
                        return;
                    }

                    // check file
                    if (!System.IO.File.Exists(musicFile))
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "File not found: " + musicFile;
                        return;
                    }

                    SampleSoundEvent ev = new SampleSoundEvent(musicFile);
                    ev.Amplitude = volume;
                    parent.soundScript.AddEvent(parent.rp.Time - musicStartTime, ev);
                    if (isAbort)
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "Task aborted";
                        return;
                    }

                    lock (parent.songChangeLock)
                    {
                        if (parent.currentSong != null)  // if needed, fade out a current playing song
                        {
                            parent.oldSongs.Add(parent.currentSong);
                        }
                        parent.currentSong = ev;
                        parent.currentSong.Amplitude = 0;
                        parent.currentSongStartTime = parent.rp.Time;
                        parent.FadeIn();
                    }

                    // if all ok, record this as last song played
                    parent.lastMusicFile = musicFile;

                }
                catch (Exception ex)
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = "Failed: " + ex.Message;
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

                // check if current song is still on list
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

                    // remove current song if done playing
                    if (rp.Time - currentSongStartTime > currentSong.Duration + 0.3)
                        currentSong = null;
                }
            }

            if (currentSong != null && currentSong.Amplitude > 0)
                MusicEngine.GetInstance().Render(soundScript, rp);  
        }

        /// <summary>
        /// fade out the music
        /// </summary>
        public void FadeOut()
        {
            isFadeOut = true;
            isFadeIn = false;
        }

        /// <summary>
        /// fades in the music. If nothing was playing it starts the last played song or else default song again.
        /// </summary>
        public void FadeIn()
        {
            isFadeOut = false;
            isFadeIn = true;
            if (UserWantsMusic && currentSong == null)
            {
                PlayLastSong();
            }
        }

        /// <summary>
        /// manually toggles the music between on and off (uses fading). Plays last song
        /// if no song is currently playing.
        /// </summary>
        public void ToggleMusic()
        {
            UserWantsMusic = !UserWantsMusic;
            if (!UserWantsMusic )
                FadeOut();
            else 
                FadeIn();
        }

        /// <summary>
        /// change music to another music track
        /// </summary>
        /// <param name="musicFile">filename of a .wav or .ogg music file to play</param>
        public void Play(string musicFile, double volume, double musicStartTime)
        {
            ITask t = new ThreadedTask(new MusicLoader(this, musicFile, volume, musicStartTime));
            UserWantsMusic = true;
            t.Start();
        }
    }
}
