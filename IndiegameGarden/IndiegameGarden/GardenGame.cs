﻿// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
// defines for global settings (debug etc)
// -> defines set in Visual Studio Profiles: DEBUG, RELEASE

using System;
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
        /// configuration and parameters store
        /// </summary>
        public GardenConfig Config;
        
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
        int myWindowWidth = 1280; //1024; //1280; //1440; //1280;
        int myWindowHeight = 768; //768; //720; //900; //720;
        public DebugMessage DebugMsg; // DEBUG
        Exception initError = null;
        MusicEngine musicEngine;

        #region Constructors
        public GardenGame()
        {
            Instance = this;
            Content.RootDirectory = "Content";

            // create the TTengine for this game
            TTengineMaster.Create(this);

            // basic XNA graphics init here (before Initialize() and LoadContent() )
            graphics = new GraphicsDeviceManager(this);
            myWindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            myWindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = myWindowWidth;
            graphics.PreferredBackBufferHeight = myWindowHeight;
            graphics.IsFullScreen = false;
            IsFixedTimeStep = false;            
            graphics.SynchronizeWithVerticalRetrace = true;
        }
        #endregion

        protected override void Initialize()
        {
            // music engine
            musicEngine = MusicEngine.GetInstance();
            musicEngine.AudioPath = ".";
            if (!musicEngine.Initialize())
                throw new Exception(musicEngine.StatusMsg); // FIXME display error box

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

            music = new GardenMusic();
            TreeRoot.Add(music);

            // MyDownloader configuration
            myDownloaderProtocol = new HttpFtpProtocolExtension();

            // load config
            if (LoadConfig())
            {
                if (DownloadConfig())
                {
                    if (LoadGameLibrary())
                    {
                        // game chooser menu
                        GameChooserMenu menu = new GameChooserMenu();
                        mainScreenlet.Add(menu);
                    }
                    else
                    {
                        Exit();
                    }
                }
                else
                {
                    Exit();
                }
            }
            else
            {
                Exit();
            }

            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();

        }

        protected override void Update(GameTime gameTime)
        {
            // update params, and call the root gamelet to do all.
            TTengineMaster.Update(gameTime, TreeRoot);

            // update any other XNA components
            base.Update(gameTime);

            if (launcher != null && !launcher.IsFinished() && 
                launcher.IsGameShowingWindow && loadingDisplay.IsLoadingState() )
            {
                loadingDisplay.SetPlayingGame(3.0f);
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
        /// indicate to game that asap we should clean up and exit
        /// </summary>
        public void ExitGame()
        {
            if (TreeRoot != null)
            {
                TreeRoot.Dispose();
                GameLib.Dispose();
                TreeRoot = null;
                GameLib = null;
            }
            System.GC.Collect();
            Exit();
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

        public void ActionLaunchWebsite(IndieGame g)
        {
            ITask t = new ThreadedTask(new SiteLauncherTask(g));
            t.Start();
        }

        /// <summary>
        /// called by a child GUI component to launch a game
        /// </summary>
        /// <param name="g">game to launch</param>
        public void ActionLaunchGame(IndieGame g)
        {
            if (g.IsInstalled)
            {
                if (g.IsPlayable)
                {
                    // if installed, then launch it if possible
                    if ((launcher == null || launcher.IsFinished() == true) &&
                         (launchGameThread == null || launchGameThread.IsFinished()))
                    {
                        loadingDisplay.SetLoadingGame(g);
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
                    music.Play(Config.GetExeFilepath(g) , g.SoundVolume );
                }
            }
        }

        // called when a launched process concludes
        void taskThread_TaskFinishedEvent(object sender)
        {
            // set menu state back to 'menu viewing' state
            GardenGame.Instance.TreeRoot.SetNextState(new StateBrowsingMenu());
        }

        /// <summary>
        /// called by a child GUI component to install a game
        /// </summary>
        /// <param name="g">game to install</param>
        public void ActionDownloadAndInstallGame(IndieGame g)
        {
            // check if download+install task needs to start or not
            if (g.DlAndInstallTask == null && g.ThreadedDlAndInstallTask == null && !g.IsInstalled)
            {
                g.DlAndInstallTask = new GameDownloadAndInstallTask(g);
                g.ThreadedDlAndInstallTask = new ThreadedTask(g.DlAndInstallTask);
                g.ThreadedDlAndInstallTask.Start();
            }
        }

        /// <summary>
        /// load, download (if needed) and check the configuration and game library
        /// Sets initError to exception in case of fatal errors.
        /// </summary>
        protected bool LoadConfig()
        {
            // first try loading from file
            try
            {
                Config = new GardenConfig();
            }
            catch (Exception ex)
            {
                initError = ex;
                return false;
            }
            return true;
        }

        protected bool DownloadConfig()
        {
            // download config - if needed or if could not be loaded
            ConfigDownloader dl = new ConfigDownloader(Config);
            configDownloadThread = new ThreadedTask(dl);
            if (dl.IsDownloadNeeded() || Config==null )
            {
                // start the task
                configDownloadThread.Start();
                
                // then wait for a short while until success of the task thread
                long timer = 0;
                long blockingWaitPeriodTicks = System.TimeSpan.TicksPerSecond * 1;  // TODO const in config
                if (Config == null || !Config.IsValid() )
                    blockingWaitPeriodTicks = System.TimeSpan.TicksPerSecond * 30;  // TODO const in config
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
                        Config = dl.NewConfig;
                        break;

                    case ITaskStatus.FAIL:
                        initError = new Exception( dl.StatusMsg() );
                        break;

                    case ITaskStatus.CREATED:
                    case ITaskStatus.RUNNING:
                        // let the downloading simply finish in the background. Load it another time.
                        break;
                }
            }

            // if still not ok after attempted download, warn the user and exit
            if (Config==null || !Config.IsValid() )
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
                GameLibraryDownloader gldl = new GameLibraryDownloader(Config.NewestGameLibraryVersion);
                gldl.Start();
                GameLib = new GameLibrary(Config.NewestGameLibraryVersion);
            }
            catch (Exception ex)
            {
                MsgBox.Show("Could not load game library file", "Could not load game library file. Technical:\n"+ex.Message+";\n"+ex.StackTrace); 
                initError = ex;
                return false;
            }

            return true;
        }

    }
}
