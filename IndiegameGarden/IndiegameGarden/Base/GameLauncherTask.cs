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
    public class GameLauncherTask: Task
    {
        public Process Proc = null;
        public IndieGame Game;

        string filePath;
        string cdPath;

        //Import the SetForeground API to activate it
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public GameLauncherTask(IndieGame g)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            cdPath = cwd + "\\" + GardenGame.Instance.Config.GetGameFolder(g.GameID, g.Version) + "\\" + g.CdPath;
            filePath = g.ExeFile;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;
            try
            {
                //TODO wait until task done, keep setting it to fg a few times.
                string cwd = System.IO.Directory.GetCurrentDirectory();
                System.IO.Directory.SetCurrentDirectory(cdPath);
                Proc = System.Diagnostics.Process.Start(filePath);
                SetForegroundWindow(Proc.MainWindowHandle.ToInt32());
                Proc.Exited += new EventHandler(EvHandlerProcessExited);
                Proc.EnableRaisingEvents = true;
            
                // wait until process exits
                int n = 0;
                while (!IsFinished())
                {
                    Thread.Sleep(100);
                    if (n < 5)
                    {
                        SetForegroundWindow(Proc.MainWindowHandle.ToInt32());
                        n++;
                    }
                    
                }

                // set previous dir back
                System.IO.Directory.SetCurrentDirectory(cwd);

                // when done switch back to our Garden app
                Process p = Process.GetCurrentProcess();
                if (p != null)
                    SetForegroundWindow(p.MainWindowHandle.ToInt32());
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
