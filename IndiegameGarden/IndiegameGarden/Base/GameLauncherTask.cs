// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices; //required for SetForegroundWindow

using IndiegameGarden.Download;
using IndiegameGarden.Base;

namespace IndiegameGarden.Base
{
    /// <summary>
    /// launches a program or game .exe in a separate process and keeps track of when it has finished. 
    /// </summary>
    public class GameLauncherTask: Task
    {
        /// <summary>
        /// the Process associated to a launched game, or null if not yet launched
        /// </summary>
        public Process Proc = null;

        /// <summary>
        /// the game that is to be launched
        /// </summary>
        public GardenItem Game;

        /// <summary>
        /// flag set true once the game window shows up during launch
        /// </summary>
        public bool IsGameShowingWindow = false;

        //Import the SetForeground API to activate it
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public GameLauncherTask(GardenItem g)
        {
            this.Game = g;
        }

        protected override void StartInternal()
        {
            try
            {
                // FIXME check for preconditions first. exefile exist, etc
                if (Game.ExeFile.Length > 0)
                {
                    string cwd = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(Game.GameFolder);
                    if (Game.CdPath.Length > 0) // if given
                    {
                        Directory.SetCurrentDirectory(Game.CdPath);
                    }
                    /* FIXME
                    else
                    {
                        Directory.SetCurrentDirectory(AutoDetectedCdPath());
                    }
                     */
                    Proc = System.Diagnostics.Process.Start(Game.ExeFile);
                    // set previous dir back
                    Directory.SetCurrentDirectory(cwd);
                }
                else
                {
                    return;
                }
                Proc.Exited += new EventHandler(EvHandlerProcessExited);
                Proc.EnableRaisingEvents = true;
            
                // monitor if process creates window and wait until process exits
                int n = 0;
                int gameWindowHandle = 0;
                while (!IsFinished())
                {
                    Thread.Sleep(100);
                    if (n < 25 && !IsFinished() )
                    {
                        gameWindowHandle = Proc.MainWindowHandle.ToInt32();
                        if (gameWindowHandle != 0) // FIXME && !IsGameShowingWindow?
                        {
                            IsGameShowingWindow = true;
                            SetForegroundWindow(gameWindowHandle);
                            //GardenGame.Instance.DebugMsg.Text = "Handle: " + Proc.MainWindowHandle.ToInt32();
                            n++;
                        }                        
                    }
                    
                }

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
            catch (FileNotFoundException)
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
                    // games are not aborted currently, user should do it.
                    // TODO check if we can abort the process
                }
            }
        }

        private void EvHandlerProcessExited(object sender, System.EventArgs e)
        {
            status = ITaskStatus.SUCCESS;
        }

        /*
        protected string AutoDetectedCdPath()
        {
            string[] dirs = Directory.GetDirectories(".");
            if (dirs.Length == 0)
                return ".";
            else 
                return dirs[0];
        }

        protected string AutoDetectedExeFile()
        {
            string[] exeFiles = Directory.GetFiles(".", "*.exe");
            if (exeFiles.Length == 0)
                return "";
            else
                return exeFiles[0];
        }
         */
    }
}
