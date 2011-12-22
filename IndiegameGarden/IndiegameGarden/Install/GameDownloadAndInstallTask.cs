// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Download;
using IndiegameGarden.Store;
using MyDownloader.Core;

namespace IndiegameGarden.Install
{
    /// <summary>
    /// a Task to download and install a game
    /// </summary>
    public class GameDownloadAndInstallTask: GameDownloader
    {
        InstallTask installTask = new InstallTask();

        public GameDownloadAndInstallTask(IndieGame game): base(game)
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Abort()
        {
            base.Abort();
            installTask.Abort();
        }

        public override bool IsStarted()
        {
            return IsStarted();
        }

        public override double Progress()
        {
            return (ProgressDownload() + ProgressInstall()) / 2;
        }

        public double ProgressDownload()
        {
            return base.Progress();
        }

        public double ProgressInstall()
        {
            return installTask.Progress();
        }

        public override void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e)
        {
            installTask.Start();
        }

    }
}
