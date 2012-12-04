using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IggLib.Base;
using IggLib.Download;

using MyDownloader.Core;
using MyDownloader.Core.Extensions;
using MyDownloader.Extension;
using MyDownloader.Extension.Protocols;

namespace IggLib
{
    public class IggLibrary
    {
        ThreadedTask launchGameThread;
        HttpFtpProtocolExtension myDownloaderProtocol;

        /// <summary>
        /// launches a game selected by user (one at a time!)
        /// </summary>
        GameLauncherTask launcher;

        public void test()
        {
            // MyDownloader configuration
            myDownloaderProtocol = new HttpFtpProtocolExtension();
            Settings.Default.MaxRetries = 0;

        }

        /// <summary>
        /// Launch a site in browser and show browser window in foreground. Can be used
        /// to launch webgames or developer websites.
        /// </summary>
        /// <returns>The site-launching task which finishes when browser shows</returns>
        /// <param name="url">url to launch in browser, optionally starting with "http://" or "https://"</param>
        public ITask ActionLaunchWebsite(string url)
        {
            ITask t = new ThreadedTask(new SiteLauncherTask(url));
            t.Start();
            return t;
        }

        /// <summary>
        /// launches installed GardenItems and keeps track of launch progress. Limits the max number
        /// of concurrent launches to one.
        /// </summary>
        /// <param name="g">game to launch</param>
        public void ActionLaunchGame(GardenItem g)
        {
            if (g.IsInstalled)
            {
                if (g.IsPlayable)
                {
                    // if installed, then launch it if possible (no ongoing game launch)
                    if ((launcher == null || launcher.IsFinished() == true) &&
                         (launchGameThread == null || launchGameThread.IsFinished()))
                    {
                        launcher = new GameLauncherTask(g);
                        launchGameThread = new ThreadedTask(launcher);
                        launchGameThread.TaskSuccessEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                        launchGameThread.TaskFailEvent += new TaskEventHandler(taskThread_TaskFinishedEvent);
                        launchGameThread.Start();
                    }
                }
                if (g.IsMusic)
                {
                    music.Play(g.ExeFilepath, 0.5f, 0f); // TODO vary audio volume per track.
                }
            }
        }

        // called when a launched process concludes
        void taskThread_TaskFinishedEvent(object sender)
        {
            // set menu state back to 'menu viewing' state
            TreeRoot.SetNextState(new StateBrowsingMenu());
        }

        /// <summary>
        /// called by a child GUI component to install a game
        /// </summary>
        /// <param name="g">game to install</param>
        public void ActionDownloadAndInstallGame(GardenItem g)
        {
            // check if download+install task needs to start or not. Can start if not already started before (and game's not installed)
            // OR if the previous install attempt failed.
            if ((g.ThreadedDlAndInstallTask == null && !g.IsInstalled) ||
                 (g.ThreadedDlAndInstallTask != null && g.ThreadedDlAndInstallTask.IsFinished() && !g.ThreadedDlAndInstallTask.IsSuccess())
                )
            {
                g.DlAndInstallTask = new GameDownloadAndInstallTask(g);
                g.ThreadedDlAndInstallTask = new ThreadedTask(g.DlAndInstallTask);
                g.ThreadedDlAndInstallTask.Start();
            }
        }

    }
}
