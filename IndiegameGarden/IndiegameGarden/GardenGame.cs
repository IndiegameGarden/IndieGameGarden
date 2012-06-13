// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
// defines for global settings (debug etc)
// -> defines set in Visual Studio Profiles: DEBUG, RELEASE

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using TTengine.Core;
using TTengine.Util;
using TTengine.Modifiers;

using TTMusicEngine;
using TTMusicEngine.Soundevents;

using IndiegameGarden.Download;
using IndiegameGarden.Unpack;
using IndiegameGarden.Menus;
using IndiegameGarden.Base;
using IndiegameGarden.Install;

using MyDownloader.Core.Extensions;
using MyDownloader.Extension;
using MyDownloader.Extension.Protocols;
using MyDownloader.Core;

namespace IndiegameGarden
{
    /**
     * <summary>
     * Main game class and singleton for IndiegameGarden
     * </summary>
     */
    public class GardenGame : Game
    {
        /// <summary>
        /// singleton instance
        /// </summary>
        public static GardenGame Instance = null;

        /// <summary>
        /// Library of games to select from
        /// </summary>
        public GameLibrary GameLib;

        /// <summary>
        /// the top-level Gamelet
        /// </summary>
        public Gamelet TreeRoot;

        /// <summary>
        /// loading screen with text
        /// </summary>
        LoadingDisplay loadingDisplay;

        /// <summary>
        /// launches a game selected by user (one at a time!)
        /// </summary>
        GameLauncherTask launcher;

        public GardenMusic music;

        ThreadedTask launchGameThread;
        ThreadedTask configDownloadThread;

        GraphicsDeviceManager graphics;
        Screenlet mainScreenlet, loadingScreenlet;       
        HttpFtpProtocolExtension myDownloaderProtocol;
        int myWindowWidth; // = 1280; //1024; //1280; //1440; //1280;
        int myWindowHeight; // = 768; //768; //720; //900; //720;
        public DebugMessage DebugMsg; // DEBUG
        Exception initError = null;
        MusicEngine musicEngine;
        bool isExiting = false;

        public GardenGame()
        {
            Instance = this;
            Content.RootDirectory = "Content";

            // create the TTengine for this game
            TTengineMaster.Create(this);

            // basic XNA graphics manager init here (before Initialize() and LoadContent() )
            graphics = new GraphicsDeviceManager(this);
            
            // DEBUG: insert below values eg = 1024; to test other resolutions 
            myWindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; 
            myWindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = myWindowWidth;
            graphics.PreferredBackBufferHeight = myWindowHeight;
            graphics.IsFullScreen = false;
            IsFixedTimeStep = false;            
            graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // music engine
            musicEngine = MusicEngine.GetInstance();
            musicEngine.AudioPath = ".";
            if (!musicEngine.Initialize())
                throw new Exception(musicEngine.StatusMsg);

            // loading screen
            loadingScreenlet = new Screenlet(myWindowWidth, myWindowHeight);
            TTengineMaster.ActiveScreen = loadingScreenlet;
            loadingScreenlet.ActiveInState = new StatePlayingGame();
            loadingScreenlet.DrawInfo.DrawColor = Color.Black;
            loadingDisplay = new LoadingDisplay();
            loadingScreenlet.Add(loadingDisplay);

            // from here on, main screen
            mainScreenlet = new Screenlet(myWindowWidth, myWindowHeight);
            TTengineMaster.ActiveScreen = mainScreenlet;
            mainScreenlet.ActiveInState = new StateBrowsingMenu();
            TreeRoot = new FixedTimestepPhysics();
            TreeRoot.SetNextState(new StateBrowsingMenu()); // set the initial state

            TreeRoot.Add(mainScreenlet);
            TreeRoot.Add(loadingScreenlet);
            mainScreenlet.DrawInfo.DrawColor = new Color(169 * 2 / 3, 157 * 2 / 3, 241 * 2 / 3); // Color.Black;

            // graphics bitmap scaling that adapts to screen resolution 
            mainScreenlet.Motion.Scale = ((float)myWindowHeight) / 900f;
            loadingScreenlet.Motion.Scale = mainScreenlet.Motion.Scale;

            // MyDownloader configuration
            myDownloaderProtocol = new HttpFtpProtocolExtension();
            Settings.Default.MaxRetries = 0;

            // load config
            if ((GardenConfig.Instance != null) && DownloadConfig() && LoadGameLibrary())
            {
                // game chooser menu
                GameChooserMenu menu = new GameChooserMenu();
                mainScreenlet.Add(menu);
            }
            else
            {
                Exit();
            }

            // music
            music = new GardenMusic();
            TreeRoot.Add(music);

        }

        protected override void Update(GameTime gameTime)
        {
            // update params, and call the root gamelet to do all.
            TTengineMaster.Update(gameTime, TreeRoot);

            // update any other XNA components
            base.Update(gameTime);

            // TODO document
            if (launcher != null && !launcher.IsFinished() && 
                launcher.IsGameShowingWindow && loadingDisplay.IsLoadingState() )
            {
                loadingDisplay.SetPlayingGame(3.0f);
            }

            if (isExiting && !music.IsPlaying )
            {
                Exit(); // finally really exit XNA if music is faded out.
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // draw all my gamelet items
            TTengineMaster.Draw(gameTime, TreeRoot);

            // then draw other (if any) XNA game components on the screen
            base.Draw(gameTime);
        }

        /// <summary>
        /// indicate to game that asap we should clean up and exit, no way back
        /// </summary>
        public void SignalExitGame()
        {
            isExiting = true;
            DownloadManager.Instance.PauseAll();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // call dispose to abort all threads/activity for entire game library (GardenItems)
            if (GameLib != null)
                GameLib.Dispose();

            base.OnExiting(sender, args);
        }

        protected override void Dispose(bool disposing)
        {
            if (configDownloadThread != null)
            {
                configDownloadThread.Abort();
            }

            if (disposing && TreeRoot != null)
            {
                TreeRoot.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ActionLaunchWebsite(GardenItem g, GameThumbnail thumb)
        {
            ITask t = new ThreadedTask(new SiteLauncherTask(g));
            t.Start();
            loadingDisplay.SetLoadingGame(g, thumb);
            TreeRoot.SetNextState(new StatePlayingGame(2f,false));
        }

        public void ActionLaunchWebsitePlayGame(GardenItem g, GameThumbnail thumb)
        {
            ITask t = new ThreadedTask(new SiteLauncherTask(g, g.ExeFile));
            t.Start();
            loadingDisplay.SetLoadingGame(g, thumb);
            music.FadeOut();
            TreeRoot.SetNextState(new StatePlayingGame(2f,false));
        }

        /// <summary>
        /// called by a child GUI component to launch a game
        /// </summary>
        /// <param name="g">game to launch</param>
        public void ActionLaunchGame(GardenItem g, GameThumbnail thumb)
        {
            if (g.IsInstalled)
            {
                if (g.IsPlayable)
                {
                    // if installed, then launch it if possible
                    if ((launcher == null || launcher.IsFinished() == true) &&
                         (launchGameThread == null || launchGameThread.IsFinished()))
                    {
                        loadingDisplay.SetLoadingGame(g, thumb);
                        // set state of game to 'game playing state'
                        TreeRoot.SetNextState(new StatePlayingGame());

                        launcher = new GameLauncherTask(g);
                        launchGameThread = new ThreadedTask(launcher);
                        launchGameThread.TaskSuccessEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                        launchGameThread.TaskFailEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                        launchGameThread.Start();
                    }
                }
                if (g.IsMusic)
                {
                    music.Play(g.ExeFilepath , 0.5f , 0f); // TODO vary audio volume per track.
                }
            }
        }

        // called when a launched process concludes
        void taskThread_TaskFinishedEvent(object sender)
        {
            // set menu state back to 'menu viewing' state
            TreeRoot.SetNextState(new StateBrowsingMenu());
        }

        /// <summary>
        /// called by a child GUI component to install a game
        /// </summary>
        /// <param name="g">game to install</param>
        public void ActionDownloadAndInstallGame(GardenItem g)
        {
            // check if download+install task needs to start or not. Can start if not already started before (and game's not installed)
            // OR if the previous install attempt failed.
            if ( (g.ThreadedDlAndInstallTask == null && !g.IsInstalled) ||
                 (g.ThreadedDlAndInstallTask != null && g.ThreadedDlAndInstallTask.IsFinished() && !g.ThreadedDlAndInstallTask.IsSuccess())
                )
            {
                g.DlAndInstallTask = new GameDownloadAndInstallTask(g);
                g.ThreadedDlAndInstallTask = new ThreadedTask(g.DlAndInstallTask);
                g.ThreadedDlAndInstallTask.Start();
            }
        }

        protected bool DownloadConfig()
        {
            // download config - if needed or if could not be loaded
            ConfigDownloader dl = new ConfigDownloader(GardenConfig.Instance);
            configDownloadThread = new ThreadedTask(dl);
            if (dl.IsDownloadNeeded() )
            {
                // start the task
                configDownloadThread.Start();
                
                // then wait for a short while until success of the task thread
                long timer = 0;
                long blockingWaitPeriodTicks = System.TimeSpan.TicksPerSecond * 0;  // TODO const in config
                if (!GardenConfig.Instance.IsValid() )
                    blockingWaitPeriodTicks = System.TimeSpan.TicksPerSecond * 30;  // TODO const in config

                if (blockingWaitPeriodTicks > 0)
                {
                    while (configDownloadThread.Status() == ITaskStatus.CREATED)
                    {
                        // block until in RUNNING state
                    }
                    while (configDownloadThread.Status() == ITaskStatus.RUNNING && timer < blockingWaitPeriodTicks)
                    {
                        Thread.Sleep(100);
                        timer += (System.TimeSpan.TicksPerMillisecond * 100);
                    }

                    switch (dl.Status())
                    {
                        case ITaskStatus.SUCCESS:
                            GardenConfig.Instance = dl.NewConfig;
                            break;

                        case ITaskStatus.FAIL:
                            initError = new Exception(dl.StatusMsg());
                            break;

                        case ITaskStatus.CREATED:
                        case ITaskStatus.RUNNING:
                            // let the downloading simply finish in the background. Load it another time.
                            break;
                    }
                }
            }

            // if still not ok after attempted download, warn the user and exit
            // a missing config counts as a valid one (then uses default params to create a new config)
            if (!GardenConfig.Instance.IsValid() )
            { 
                TTengine.Util.MsgBox.Show("Could not load configuration", "Could not load configuration file. Is it missing or corrupted?"); 
                return false;
            }
            return true;
        }

        protected bool LoadGameLibrary()
        {
            // load game library
            try
            {
                GameLibraryDownloader gldl = new GameLibraryDownloader(GardenConfig.Instance.NewestGameLibraryVersion);
                gldl.Start();
                GameLib = new GameLibrary();
                GameLib.LoadBin(GardenConfig.Instance.NewestGameLibraryVersion);
            }
            catch (Exception ex)
            {
                // if fails, try loading default lib from file, as alternative
                try
                {
                    GameLib = new GameLibrary();
                    GameLib.LoadBin(Path.Combine(Content.RootDirectory, "gamelib.bin"));
                }
                catch (Exception)
                {
                    MsgBox.Show("Could not load game library file", "Could not load game library file. Technical:\n" + ex.Message + ";\n" + ex.StackTrace);
                    initError = ex;
                    return false;
                }
            }

            return true;
        }

    }
}
