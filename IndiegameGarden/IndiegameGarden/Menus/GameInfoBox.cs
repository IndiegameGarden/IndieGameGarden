// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;
using TTengine.Modifiers;
using TTengine.Util;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A box showing game information, download status, helpful messages etc.
    /// </summary>
    public class GameInfoBox: Drawlet
    {
        // relative position w.r.t. right of user's display
        static Vector2 PROGRESS_BAR_POSITION_RELATIVE = new Vector2(-0.5f, 0.0f);

        GameTextBox titleBox;
        GameTextBox descriptionBox;
        
        /// <summary>
        /// the game for which info will be displayed, or null if none yet
        /// </summary>
        public GardenItem game;

        public GameInfoBox()
            : base()
        {
            InitComponents();
            MySpriteBatch = new TTSpriteBatch(Screen.graphicsDevice);
        }

        private void InitComponents()
        {
            titleBox = new GameTextBox("m41_lovebit");
            titleBox.Motion.Position = new Vector2(0.0f, 0.0f);
            titleBox.DrawInfo.LayerDepth = 0.04f;
            Add(titleBox);

            descriptionBox = new GameTextBox("GameDescriptionFont");
            descriptionBox.Motion.Position = new Vector2(0.0f, 0.04f);
            descriptionBox.DrawInfo.LayerDepth = 0.04f;
            Add(descriptionBox);

            DarkeningHBar darkBar = new DarkeningHBar(0.6f, 0.63f); // TODO GUI constants?
            darkBar.Motion.Position.Y = -0.01f;
            darkBar.DrawInfo.LayerDepth = 0.05f;
            Add(darkBar);
        }

        /// <summary>
        /// specify which game the info box should show status/info of
        /// </summary>
        /// <param name="g"></param>
        public void SetGameInfo(GardenItem g)
        {
            game = g;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (game != null)
            {
                string title = game.Name;
                if (game.Status != null)
                {
                    title += game.Status;
                }

                string desc = game.Description + "\n";
                if (  (game.IsGrowable && game.IsInstalled) || game.IsWebGame)
                {
                    desc += "Yes, it's in your garden! Hold ENTER or mouseclick to play.\n";
                }
                else if (game.IsGrowable)
                {
                    if (game.DlAndInstallTask == null)
                    {

                        desc += "This " + game.ItemName + " is not in your garden. Hold ENTER or mouseclick to grow it.\n";
                    }
                    else if (game.DlAndInstallTask != null &&
                        game.ThreadedDlAndInstallTask != null &&
                        !game.ThreadedDlAndInstallTask.IsFinished())
                    {
                        if (game.DlAndInstallTask.IsDownloading())
                        {
                            desc += "Growing " + game.ItemName + "... please wait. Watch the progress bar.\n"; // TODO some abort possibility message
                        }
                        else if (game.DlAndInstallTask.IsInstalling())
                        {
                            desc += "Growing " + game.ItemName + "... almost done.\n";
                        }
                    }
                    else
                    {
                        if (game.ThreadedDlAndInstallTask != null && 
                            !game.ThreadedDlAndInstallTask.IsSuccess() && 
                            game.ThreadedDlAndInstallTask.IsFinished())
                        {
                            desc += "Growth problem: " + game.ThreadedDlAndInstallTask.StatusMsg() ;
                        }
                    }
                }

                titleBox.Text = title;
                descriptionBox.Text = desc;
                
            }
            else
            {
                titleBox.Text = ""; // no text if no game chosen
                descriptionBox.Text = "";
            }
        }

    }
}
