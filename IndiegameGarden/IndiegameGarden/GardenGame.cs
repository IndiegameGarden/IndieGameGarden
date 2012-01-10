// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
// defines for global settings (debug etc)
// -> defines set in Visual Studio Profiles: DEBUG, RELEASE

using System;
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

using IndiegameGarden.Download;
using IndiegameGarden.Unpack;
using IndiegameGarden.Menus;
using IndiegameGarden.Base;

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

        // --- internal + TTengine related
        GraphicsDeviceManager graphics;
        int preferredWindowWidth = 1420; //1024; //1280; //1440; //1280;
        int preferredWindowHeight = 880; //768; //720; //900; //720;
        Screenlet toplevelScreen;        
        // treeRoot is a pointer, set to the top-level Gamelet to render
        Gamelet treeRoot;
        Gamelet gameletsRoot;
        SpriteBatch spriteBatch;
        HttpFtpProtocolExtension myDownloaderProtocol;

        #region Constructors
        public GardenGame()
        {
            Instance = this;
            Content.RootDirectory = "Content";

            // create the TTengine for this game
            TTengineMaster.Create(this);

            // basic XNA graphics init here (before Initialize() and LoadContent() )
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = preferredWindowWidth;
            graphics.PreferredBackBufferHeight = preferredWindowHeight;            
#if RELEASE
            graphics.IsFullScreen = true;
#else
            graphics.IsFullScreen = false;
#endif
            this.IsFixedTimeStep = false;
            
            graphics.SynchronizeWithVerticalRetrace = true;
        }
        #endregion

        protected override void Initialize()
        {
            Exception initError = null;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            toplevelScreen = new Screenlet(preferredWindowWidth, preferredWindowHeight);
            Gamelet physicsModel = new FixedTimestepPhysics();

            toplevelScreen.Add(physicsModel);
            toplevelScreen.Add(new FrameRateCounter(1.0f, 0f)); // TODO
            toplevelScreen.Add(new ScreenZoomer()); // TODO remove
            toplevelScreen.DrawColor = Color.Black;
            treeRoot = toplevelScreen;
            gameletsRoot = physicsModel;

            // MyDownloader Config
            myDownloaderProtocol = new HttpFtpProtocolExtension();

            // GardenConfig
            try
            {
                Config = new GardenConfig();
            }
            catch (Exception ex)
            {
                TTengine.Util.MsgBox.Show("Could not load configuration", "Could not load configuration file."); // TODO
                initError = ex;
                Exit();
            }

            // game library
            try
            {
                GameLib = new GameLibrary();
            }
            catch (Exception ex)
            {
                MsgBox.Show("Could not load game library file", "Could not load game library file."); // TODO
                initError = ex;
                Exit();
            }

            if (initError == null)
            {
                // game chooser menu
                GameChooserMenu menu = new GameChooserMenu();
                gameletsRoot.Add(menu);
            }

            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();

        }

        protected override void Update(GameTime gameTime)
        {
            // update params, and call the root gamelet to do all.
            TTengineMaster.Update(gameTime, treeRoot);

            // update any other XNA components
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // draw all my gamelet items
            GraphicsDevice.SetRenderTarget(null); // TODO
            TTengineMaster.Draw(gameTime, treeRoot);

            // then buffer drawing on screen at right positions                        
            GraphicsDevice.SetRenderTarget(null); // TODO
            //GraphicsDevice.Clear(Color.Black);
            Rectangle destRect = new Rectangle(0, 0, toplevelScreen.RenderTarget.Width, toplevelScreen.RenderTarget.Height);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(toplevelScreen.RenderTarget, destRect, Color.White);
            spriteBatch.End();

            // then draw other (if any) game components on the screen
            base.Draw(gameTime);

        }

        public void ExitGame()
        {
            this.gameletsRoot.Dispose();
            this.gameletsRoot = null;
            System.GC.Collect();
            /*
            for (int i = 0; i < DownloadManager.Instance.Downloads.Count; i++)
                DownloadManager.Instance.RemoveDownload(DownloadManager.Instance.Downloads[i]);
             */
            Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                treeRoot.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
