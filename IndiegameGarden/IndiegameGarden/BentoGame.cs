// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
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
    public class BentoGame : Game
    {
        /// <summary>
        /// singleton instance
        /// </summary>
        public static BentoGame Instance = null;

        /// <summary>
        /// Library of games to select from
        /// </summary>
        public GameLibrary GameLib;

        /// <summary>
        /// the top-level Gamelet
        /// </summary>
        public Gamelet TreeRoot;

        /// <summary>
        /// startup splash screen graphic
        /// </summary>
        public Spritelet SplashScreen;

        /// <summary>
        /// launches a game selected by user (one at a time!)
        /// </summary>
        public GameLauncherTask launcher;

        public GardenMusic music;

        ThreadedTask launchGameThread;

        GraphicsDeviceManager graphics;
        Screenlet mainScreenlet;       
        HttpFtpProtocolExtension myDownloaderProtocol;
        int myWindowWidth; // = 1280; //1024; //1280; //1440; //1280;
        int myWindowHeight; // = 768; //768; //720; //900; //720;
        public DebugMessage DebugMsg; // DEBUG
        Exception initError = null;
        MusicEngine musicEngine;
        bool isExiting = false;

        public BentoGame()
        {
            // frame border
            Form frm = (Form)Form.FromHandle(Window.Handle);
            //frm.FormBorderStyle = FormBorderStyle.None;

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
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            if (!GardenConfig.Instance.VerifyDataPath())
                throw new Exception("Fatal Error - Could not create folders in " + GardenConfig.Instance.DataPath);
            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GardenInit();
        }

        void GardenInit()
        {
            // from here on, main screen
            mainScreenlet = new Screenlet(myWindowWidth, myWindowHeight);
            TTengineMaster.ActiveScreen = mainScreenlet;
            TreeRoot = new FixedTimestepPhysics();
            TreeRoot.SetNextState(new StateStartup()); // set the initial state

            TreeRoot.Add(mainScreenlet);
            mainScreenlet.DrawInfo.DrawColor = Color.White; // new Color(169 * 2 / 3, 157 * 2 / 3, 241 * 2 / 3); // Color.Black;

            // graphics bitmap scaling that adapts to screen resolution 
            mainScreenlet.Motion.Scale = ((float)myWindowHeight) / 900f;

            // splash screen (upon starting Bento)
            Spritelet SplashScreen = new Spritelet("bentologo");
            SplashScreen.DrawInfo.LayerDepth = 1f;
            //SplashScreen.ActiveInState = new StateStartup();
            SplashScreen.Motion.Position = mainScreenlet.Center;
            mainScreenlet.Add(SplashScreen);

            Thread t = new Thread(new ThreadStart(GardenInitInBackground));
            t.Start();
        }

        void GardenInitInBackground() {

            // music engine
            musicEngine = MusicEngine.GetInstance();
            musicEngine.AudioPath = ".";
            if (!musicEngine.Initialize())
                throw new Exception(musicEngine.StatusMsg);

            // music
            music = new GardenMusic();
            TreeRoot.AddNextUpdate(music);

            // MyDownloader configuration
            myDownloaderProtocol = new HttpFtpProtocolExtension();
            MyDownloader.Core.Settings.Default.MaxRetries = 0;

            // load config
            if ((GardenConfig.Instance != null) && LoadGameLibrary())
            {
                // game chooser menu
                GameChooserMenu menu = new GameChooserMenu();
                //menu.ActiveInState = new StateBrowsingMenu();
                mainScreenlet.AddNextUpdate(menu);
            }
            else
            {
                Exit();
            }

            // activate next phase
            TreeRoot.SetNextState(new StateBrowsingMenu());
        }

        protected override void Update(GameTime gameTime)
        {
            // update params, and call the root gamelet to do all.
            TTengineMaster.Update(gameTime, TreeRoot);

            // update any other XNA components
            base.Update(gameTime);

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

        class DownloadsAllPausedTask : Task
        {
            protected override void StartInternal()
            {
                // below method may block for long times when worker threads won't stop
                DownloadManager.Instance.PauseAll();
            }

            protected override void AbortInternal()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// indicate to game that asap we should clean up and exit, no way back
        /// </summary>
        public void SignalExitGame()
        {
            isExiting = true;
            ITask t = new ThreadedTask(new DownloadsAllPausedTask());
            t.Start(); 
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
            IsMouseVisible = true;

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
            TreeRoot.SetNextState(new StatePlayingGame(2f,false));
        }

        public void ActionLaunchWebsitePlayGame(GardenItem g, GameThumbnail thumb)
        {
            ITask t = new ThreadedTask(new SiteLauncherTask(g.ExeFile));
            t.Start();
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
                    if(true) 
                    //if ((launcher == null || launcher.IsFinished() == true) &&
                    //     (launchGameThread == null || launchGameThread.IsFinished()))
                    {
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
        // FIXME - what if multiple processes call this? wrong behavior of menu?
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

        protected bool LoadGameLibrary()
        {
            // if fails, try loading default lib from file, as alternative
            try
            {
                GameLib = new GameLibrary();
                GameLib.LoadBin(Path.Combine(Content.RootDirectory, "gamelib.bin"));
            }
            catch (Exception ex)
            {
                IsMouseVisible = true;
                MsgBox.Show("Could not load game library file", "Could not load game library file. Technical:\n" + ex.Message + ";\n" + ex.StackTrace);
                initError = ex;
                return false;
            }
            return true;
        }

    }
}
