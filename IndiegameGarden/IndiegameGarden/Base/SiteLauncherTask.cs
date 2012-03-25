// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices; //required for SetForegroundWindow

using IndiegameGarden.Download;
using IndiegameGarden.Base;

namespace IndiegameGarden.Base
{
    /// <summary>
    /// launches a game .exe in a separate process and keeps track of when it has finished
    /// </summary>
    public class SiteLauncherTask: Task
    {
        /// <summary>
        /// the Process associated to a launched game, or null if not yet launched
        /// </summary>
        public Process Proc = null;

        /// <summary>
        /// the IndieGame that is to be launched
        /// </summary>
        public IndieGame Game;

        /// <summary>
        /// flag set true once the game window shows up during launch
        /// </summary>
        public bool IsGameShowingWindow = false;

        // internal
        string filePath = "";

        //Import the SetForeground API to activate it
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public SiteLauncherTask(IndieGame g)
        {
            this.Game = g;
            string url = g.DeveloperWebsiteURL;
            if (url.Length > 0)
            {
                if (!url.ToLower().StartsWith("http://"))
                {
                    url = "http://" + url;
                }
                filePath = url;            
            }            
        }

        protected override void StartInternal()
        {
            try
            {
                // check if anything sane needs to be launched.
                if (filePath.Length == 0)
                {
                    status = ITaskStatus.SUCCESS; // no, so done.
                    return;
                }

                Proc = System.Diagnostics.Process.Start(filePath);
                if (Proc == null)
                {
                    status = ITaskStatus.SUCCESS;
                }
                else
                {
                    Proc.Exited += new EventHandler(EvHandlerProcessExited);
                    Proc.EnableRaisingEvents = true;
                    // monitor if process creates window and wait until process exits
                    int n = 0;
                    int gameWindowHandle = 0;
                    while (!IsFinished())
                    {
                        Thread.Sleep(100);
                        if (n < 25 && !IsFinished())
                        {
                            gameWindowHandle = Proc.MainWindowHandle.ToInt32();
                            if (gameWindowHandle != 0)
                            {
                                IsGameShowingWindow = true;
                                SetForegroundWindow(gameWindowHandle);
                                //GardenGame.Instance.DebugMsg.Text = "Handle: " + Proc.MainWindowHandle.ToInt32();
                                n++;
                            }
                        }

                    }

                    status = ITaskStatus.SUCCESS;
                }

                // when done switch back to our Garden app
                //Process p = Process.GetCurrentProcess();
                //if (p != null)
                //    SetForegroundWindow(p.MainWindowHandle.ToInt32());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                status = ITaskStatus.FAIL;
            }
            catch (System.ObjectDisposedException)
            {             
                status = ITaskStatus.FAIL;
            }
            catch (System.IO.FileNotFoundException)
            {             
                status = ITaskStatus.FAIL;
            }
        }

        protected override void AbortInternal()
        {
            if (IsRunning())
            {
                if (Proc != null)
                {
                    // FIXME check if we can abort the process
                }
            }
        }

        public override double Progress()
        {
            if (IsFinished())
                return 1.0;
            return 0.0;
        }

        private void EvHandlerProcessExited(object sender, System.EventArgs e)
        {
            status = ITaskStatus.SUCCESS;
        }

    }
}
