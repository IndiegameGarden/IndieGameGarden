// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A box showing game information, download status, helpful messages etc.
    /// </summary>
    public class GameInfoBox: Drawlet
    {
        public MotionBehavior MotionB;
        ProgressBar dlProgressBar;
        GameTextBox titleBox;
        GameTextBox descriptionBox;
        IndieGame game;

        public GameInfoBox()
            : base()
        {
            InitComponents();
            MySpriteBatch = new SpriteBatch(Screen.graphicsDevice);
        }

        private void InitComponents()
        {
            MotionB = new MotionBehavior();
            Add(MotionB);

            dlProgressBar = new ProgressBar();
            dlProgressBar.Motion.Position = new Vector2(0.55f, 0.0f);
            dlProgressBar.Visible = false;
            Add(dlProgressBar);

            titleBox = new GameTextBox("");
            titleBox.Motion.Position = new Vector2(0.0f, 0.0f);
            Add(titleBox);

            descriptionBox = new GameTextBox("");
            descriptionBox.Motion.Position = new Vector2(0.0f, 0.08f);
            descriptionBox.Motion.Scale = 0.7f;
            Add(descriptionBox);
        }

        /// <summary>
        /// specify which game the info box should show status/info of
        /// </summary>
        /// <param name="g"></param>
        public void SetGameInfo(IndieGame g)
        {
            game = g;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (game != null)
            {
                string title = game.Name; 
                string desc = game.Description + "\n";
                if (game.IsInstalled)
                {
                    desc += "Installed: Hold ENTER to play!\n";
                    dlProgressBar.Visible = false;
                }
                else
                {
                    if (game.DlAndInstallTask == null)
                    {

                        desc += "Not Installed: Hold ENTER to download!\n";
                    }
                    else if (game.DlAndInstallTask != null &&
                        game.ThreadedDlAndInstallTask != null &&
                        !game.ThreadedDlAndInstallTask.IsFinished())
                    {
                        if (game.DlAndInstallTask.IsDownloading())
                        {
                            desc += "Downloading...\n"; // TODO some abort possibility message
                        }
                        else if (game.DlAndInstallTask.IsInstalling())
                        {
                            desc += "Installing...\n";
                        }
                        dlProgressBar.ProgressTarget = (float)game.DlAndInstallTask.Progress();
                        // make bar visible if not already.
                        if (dlProgressBar.Visible == false)
                        {
                            dlProgressBar.Visible = true;
                            // instantly reset value to new one, if we just made the bar visible
                            dlProgressBar.ProgressValue = dlProgressBar.ProgressTarget;
                        }
                    }
                    else
                    {
                        dlProgressBar.Visible = false;
                    }
                }

                titleBox.Text = title;
                descriptionBox.Text = desc;
                dlProgressBar.Visible = true; // DEBUG
                float n = ( SimTime/4f ) % 1.1f;
                if ( n < dlProgressBar.ProgressTarget)
                {
                    dlProgressBar.ProgressValue = 0f;
                }
                dlProgressBar.ProgressTarget = n;
            }
            else
            {
                titleBox.Text = ""; // no text if no game chosen
                descriptionBox.Text = "";
                dlProgressBar.Visible = false;
            }
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }


    }
}
