// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Download;
using IndiegameGarden.Base;
using MyDownloader.Core;

namespace IndiegameGarden.Install
{

    /// <summary>
    /// a Task to both download and install a game. If game file already exists locally, download is skipped.
    /// If game is already installed locally, install is skipped.
    /// </summary>
    public class GameDownloadAndInstallTask: Task
    {
        /// <summary>
        /// how much of the progress percentage is allocated to downloading (remainder is for installing)
        /// </summary>
        const double FRACTION_OF_PROGRESS_FOR_DOWNLOAD = 0.90;

        InstallTask installTask;
        GameDownloader downloadTask;
        IndieGame game;

        /// <summary>
        /// create new Download and Install task
        /// </summary>
        /// <param name="game">info of game to download and install</param>
        public GameDownloadAndInstallTask(IndieGame game)
        {
            this.game = game;
            status = ITaskStatus.CREATED;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;

            // do the checking if already installed
            game.Refresh();
            if (game.IsInstalled)
            {
                status = ITaskStatus.SUCCESS;
                return;
            }

            // start the download task
            downloadTask = new GameDownloader(game);
            downloadTask.Start();

            if (downloadTask.IsFinished() && downloadTask.Status() != ITaskStatus.FAIL )
            {
                // if download ready and OK, start install
                installTask = new InstallTask(game);
                installTask.Start();
                status = installTask.Status();
            }
            else
            {
                // error in downloading process - no install
                status = ITaskStatus.FAIL;
            }
            game.Refresh();
        }

        public override void Abort()
        {
            if (IsDownloading())
            {
                downloadTask.Abort();
                statusMsg = downloadTask.StatusMsg();
                status = ITaskStatus.FAIL;
            }
            else if (IsInstalling())
            {
                installTask.Abort();
                statusMsg = installTask.StatusMsg();
                status = ITaskStatus.FAIL;
            }
            else
            {
                if(downloadTask != null)
                    downloadTask.Abort();
                if(installTask != null)
                    installTask.Abort();
                if (status != ITaskStatus.SUCCESS)
                    status = ITaskStatus.FAIL;
            }
        }

        /// <summary>
        /// check whether currently downloading
        /// </summary>
        /// <returns>true if downloading, false otherwise</returns>
        public bool IsDownloading()
        {
            return (downloadTask != null && downloadTask.IsStarted() && !downloadTask.IsFinished() );
        }

        /// <summary>
        /// check whether currently installing
        /// </summary>
        /// <returns>true if installing, false otherwise</returns>
        public bool IsInstalling()
        {
            return (installTask != null && installTask.IsStarted() && !installTask.IsFinished() );
        }

        public override double Progress()
        {            
            if (IsDownloading())
                return ProgressDownload() * FRACTION_OF_PROGRESS_FOR_DOWNLOAD;
            if (IsInstalling())
                return (ProgressInstall() * (1.0 - FRACTION_OF_PROGRESS_FOR_DOWNLOAD) + FRACTION_OF_PROGRESS_FOR_DOWNLOAD);
            if (installTask != null && installTask.IsFinished() )
                return 1.0;
            return 0.0;            
        }

        /// <summary>
        /// check progressContributionSingleFile in downloading task
        /// </summary>
        /// <returns>progressContributionSingleFile value 0...1</returns>
        public double ProgressDownload()
        {
            if (downloadTask == null)
                return 0;
            return downloadTask.Progress();
        }

        /// <summary>
        /// check progressContributionSingleFile in installing task
        /// </summary>
        /// <returns>progressContributionSingleFile value 0...1</returns>
        public double ProgressInstall()
        {
            if (installTask == null)
                return 0;
            return installTask.Progress();
        }

    }
}
