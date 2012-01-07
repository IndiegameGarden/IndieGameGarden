// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TTengine.Core;

using IndiegameGarden.Base;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// A box showing game information, download status, helpful messages etc.
    /// </summary>
    public class GameInfoBox: MovingEffectSpritelet
    {
        ProgressBar dlProgressBar;
        GameTextBox textBox;
        IndieGame game;

        public GameInfoBox()
            : base()
        {
            InitComponents();
        }

        private void InitComponents()
        {
            dlProgressBar = new ProgressBar();
            dlProgressBar.Position = new Vector2(0.4f, 0.07f);
            dlProgressBar.Visible = false;
            Add(dlProgressBar);

            textBox = new GameTextBox("");
            textBox.Position = new Vector2(0.0f, 0.0f);
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
                    txt += "Installed - Press ENTER to play!\n";
                    dlProgressBar.Visible = false;
                }
                else
                {
                    if (game.DlAndInstallTask != null)
                    {
                        if (game.DlAndInstallTask.IsDownloading())
                        {
                            txt += "Downloading...\n";
                        }
                        else if (game.DlAndInstallTask.IsInstalling())
                        {
                            txt += "Installing...\n";
                        }
                        dlProgressBar.Visible = true;
                        dlProgressBar.Progress = (float) game.DlAndInstallTask.Progress();
                    }

                    if (game.DlAndInstallTask == null)
                    {
                        dlProgressBar.Visible = false;
                        txt += "Press ENTER to download this game!\n";
                    }
                }

                textBox.Text = txt;
            }
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }


    }
}
