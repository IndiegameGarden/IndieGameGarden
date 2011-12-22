// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
// defines for global settings (debug etc)
// -> defines set in Visual Studio Profiles: DEBUG, RELEASE


using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
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
using IndiegameGarden.Store;

using MyDownloader.Core.Extensions;
using MyDownloader.Extension;
using MyDownloader.Extension.Protocols;

namespace IndiegameGarden
{
    /**
     * <summary>
     * Main game class and singleton
     * </summary>
     */
    public class GardenMain : Game
    {
        public static GardenMain Instance = null;

        public GameLibrary gameLibrary;
        public StorageConfig storageConfig;

        // gfx/TTengine related
        GraphicsDeviceManager graphics;
        public int preferredWindowWidth = 1024; //1280; //1440; //1280;
        public int preferredWindowHeight = 768; //720; //900; //720;
        public Screenlet toplevelScreen;
        // treeRoot is a pointer, set to the top-level Gamelet to render
        public Gamelet treeRoot;
        //public Gamelet titleScreen;
        public Gamelet gameletsRoot;
        public SpriteBatch spriteBatch;

        // internal
        private HttpFtpProtocolExtension myDownloaderProtocol;

        public GardenMain()
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

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            toplevelScreen = new Screenlet(1024, 768);
            Gamelet physicsModel = new FixedTimestepPhysics();

            toplevelScreen.Add(physicsModel);
            toplevelScreen.Add(new FrameRateCounter(1.0f, 0f));
            toplevelScreen.Add(new ScreenZoomer());
            treeRoot = toplevelScreen;
            gameletsRoot = physicsModel;

            // MyDownloader config
            myDownloaderProtocol = new HttpFtpProtocolExtension();

            // StorageConfig
            storageConfig = new StorageConfig();

            // game library
            gameLibrary = new GameLibrary();

            // game chooser menu
            GameChooserMenu menu = new GameChooserMenu();
            gameletsRoot.Add(menu);

            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
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

        protected void TestRotatingBall()
        {
            Spritelet b = new Spritelet("ball-supernova2");
            b.Position = new Vector2(0.6f, 0.5f);
            b.LayerDepth = 0.91f;
            b.Add(new MyFuncyModifier(delegate(float t) { b.RotateModifier += t / 10f; }));
            gameletsRoot.Add(b);
        }

    }
}
