// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
    /// a Task to both download and install a game
    /// </summary>
    public class GameDownloadAndInstallTask: GameDownloader
    {
        InstallTask installTask;
        enum TaskPhase { DOWNLOAD, INSTALL };
        TaskPhase phase;

        /// <summary>
        /// create new Download and Install task
        /// </summary>
        /// <param name="game">info of game to download and install</param>
        public GameDownloadAndInstallTask(IndieGame game): base(game)
        {
            phase = TaskPhase.DOWNLOAD;
        }

        public override void Start()
        {
            // first start the download task (my base class)
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

        /// <summary>
        /// check whether currently downloading
        /// </summary>
        /// <returns>true if downloading, false otherwise</returns>
        public bool IsDownloading()
        {
            return (phase == TaskPhase.DOWNLOAD);
        }

        /// <summary>
        /// check whether currently installing
        /// </summary>
        /// <returns>true if installing, false otherwise</returns>
        public bool IsInstalling()
        {
            return (phase == TaskPhase.INSTALL);
        }

        public override double Progress()
        {
            switch (phase)
            {
                case TaskPhase.DOWNLOAD:
                    return ProgressDownload();
                case TaskPhase.INSTALL:
                    return ProgressInstall();
            }
            throw new NotImplementedException("Wrong phase");
        }

        /// <summary>
        /// check progress in downloading task
        /// </summary>
        /// <returns>progress value 0...1</returns>
        public double ProgressDownload()
        {
            return base.Progress();
        }

        /// <summary>
        /// check progress in installing task
        /// </summary>
        /// <returns>progress value 0...1</returns>
        public double ProgressInstall()
        {
            if (installTask == null)
                return 0;
            return installTask.Progress();
        }

        // once download ends, start the install task
        public override void OnDownloadEnded(Downloader dl)
        {
            if (dl.State.Equals(DownloaderState.Ended))
            {
                // if download ready and OK, start install
                installTask = new InstallTask(game);
                installTask.Start();
            }
            else
            {
                // TODO
                throw new NotImplementedException("check code OnDownloadEnded");
            }
        }

    }
}
