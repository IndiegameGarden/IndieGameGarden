// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;
using TTengine.Modifiers;
using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A box showing game information, download status, helpful messages etc.
    /// </summary>
    public class GameInfoBox: Drawlet
    {
        // relative position w.r.t. right of user's display
        static Vector2 PROGRESS_BAR_POSITION_RELATIVE = new Vector2(-0.32f, 0.0f);

        ProgressBar dlProgressBar;
        GameTextBox titleBox;
        GameTextBox descriptionBox;
        GardenItem game;

        public GameInfoBox()
            : base()
        {
            InitComponents();
            MySpriteBatch = new SpriteBatch(Screen.graphicsDevice);
        }

        private void InitComponents()
        {
            dlProgressBar = new ProgressBar();
            dlProgressBar.Motion.Position = new Vector2(Screen.Width,0f) + PROGRESS_BAR_POSITION_RELATIVE ;
            dlProgressBar.Visible = false;
            dlProgressBar.ProgressValue = 0f;
            dlProgressBar.ProgressTarget = 0f;
            dlProgressBar.BarWidth = 0.4f;
            Add(dlProgressBar);

            titleBox = new GameTextBox("m41_lovebit");
            titleBox.Motion.Position = new Vector2(0.0f, 0.0f);
            Add(titleBox);

            descriptionBox = new GameTextBox("GameDescriptionFont");
            descriptionBox.Motion.Position = new Vector2(0.0f, 0.04f);
            Add(descriptionBox);

            dlProgressBar.Pulsing = false; 
        }

        /// <summary>
        /// specify which game the info box should show status/info of
        /// </summary>
        /// <param name="g"></param>
        public void SetGameInfo(GardenItem g)
        {
            game = g;
        }

        /// <summary>
        /// clears the progress bar (temporarily, until auto re-activated)
        /// </summary>
        public void ClearProgressBar()
        {
            dlProgressBar.Visible = false;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (game != null)
            {
                string title = game.Name; 
                string desc = game.Description + "\n";
                if (game.IsInstalled || game.IsWebGame)
                {
                    desc += "This "+game.ItemName+" now grows in your garden. Hold ENTER to play!\n";
                    //dlProgressBar.Visible = false;
                    dlProgressBar.ProgressTarget = 1.0f;
                    dlProgressBar.ProgressValue = 1.0f;
                    dlProgressBar.Pulsing = false;
                }
                else if (game.IsGrowable)
                {
                    if (game.DlAndInstallTask == null)
                    {

                        desc += "This " + game.ItemName + " is not yet in your garden. Hold ENTER to grow it.\n";
                        dlProgressBar.Visible = false;
                        dlProgressBar.ProgressTarget = 0.0f;
                        dlProgressBar.ProgressValue = 0.0f;
                        dlProgressBar.Pulsing = false;
                    }
                    else if (game.DlAndInstallTask != null &&
                        game.ThreadedDlAndInstallTask != null &&
                        !game.ThreadedDlAndInstallTask.IsFinished())
                    {
                        if (game.DlAndInstallTask.IsDownloading())
                        {
                            desc += "Growing " + game.ItemName + "... please wait. Watch the progress bar.\n"; // TODO some abort possibility message
                            dlProgressBar.Pulsing = true;
                        }
                        else if (game.DlAndInstallTask.IsInstalling())
                        {
                            desc += "Growing " + game.ItemName + "... almost done.\n";
                            dlProgressBar.Pulsing = true;
                        }
                        else
                        {
                            dlProgressBar.Pulsing = false;
                        }
                        dlProgressBar.ProgressTarget = (float)game.DlAndInstallTask.Progress();
                        dlProgressBar.ProgressSpeed = (float)game.DlAndInstallTask.DownloadSpeed();
                        // make bar visible if not already. Or if value needs to go down from a previous selected game's value.
                        if (dlProgressBar.Visible == false  || dlProgressBar.ProgressValue > dlProgressBar.ProgressTarget )
                        {
                            dlProgressBar.Visible = true;
                            // instantly reset value to new one, if we just made the bar visible
                            dlProgressBar.ProgressValue = dlProgressBar.ProgressTarget;
                        }
                    }
                    else
                    {
                        dlProgressBar.Visible = false;
                        dlProgressBar.Pulsing = false;
                        dlProgressBar.ProgressTarget = 1.0f;
                        dlProgressBar.ProgressValue = 1.0f;
                        if (game.ThreadedDlAndInstallTask != null && 
                            !game.ThreadedDlAndInstallTask.IsSuccess() && 
                            game.ThreadedDlAndInstallTask.IsFinished())
                        {
                            desc += "Problem during growth: " + game.ThreadedDlAndInstallTask.StatusMsg() ;
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
                dlProgressBar.Visible = false;
                dlProgressBar.ProgressValue = 0f;
                dlProgressBar.ProgressTarget = 0f;
                dlProgressBar.Pulsing = false;
            }
        }

    }
}
