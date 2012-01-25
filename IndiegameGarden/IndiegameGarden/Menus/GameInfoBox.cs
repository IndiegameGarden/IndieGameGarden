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
        GameTextBox textBox;
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
            dlProgressBar.Motion.Position = new Vector2(0.55f, 0.04f);
            dlProgressBar.Visible = false;
            Add(dlProgressBar);

            textBox = new GameTextBox("");
            textBox.Motion.Position = new Vector2(0.0f, 0.0f);
            Add(textBox);
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
                string txt = game.Name + "\n\n" + game.Description + "\n";
                if (game.IsInstalled)
                {
                    txt += "Installed - Hold ENTER to play!\n";
                    dlProgressBar.Visible = false;
                }
                else
                {
                    if (game.DlAndInstallTask == null)
                    {

                        txt += "Hold ENTER to download this game!\n";
                    }
                    else if (game.DlAndInstallTask != null &&
                        game.ThreadedDlAndInstallTask != null && 
                        !game.ThreadedDlAndInstallTask.IsFinished())
                    {
                        if (game.DlAndInstallTask.IsDownloading())
                        {
                            txt += "Downloading...\n"; // TODO some abort possibility message
                        }
                        else if (game.DlAndInstallTask.IsInstalling())
                        {
                            txt += "Installing...\n";
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

                textBox.Text = txt;
            }
            else
            {
                textBox.Text = ""; // no text if no game chosen
            }
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }


    }
}
