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
                // first cd to the game's folder
                string cwd = Directory.GetCurrentDirectory();
                string gameFolder = Game.GameFolder;
                if (!Directory.Exists(gameFolder))
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = "GameFolder not found: " + gameFolder;
                    return;
                }
                Directory.SetCurrentDirectory(gameFolder);

                // find the .exe file if needed, and if it's still unknown abort.
                AutoDetectExeFileIfNeeded(Game);
                if (Game.ExeFile.Length == 0)
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = "No ExeFile found/defined";
                    return;
                }

                // if a cd path is given, then cd to it
                if (Game.CdPath.Length > 0) // if given
                {
                    if (!Directory.Exists(Game.CdPath))
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "CdPath not found: " + Game.CdPath;
                        return;
                    }
                    Directory.SetCurrentDirectory(Game.CdPath);
                }

                // check & start the .exe file
                if (!File.Exists(Game.ExeFile))
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = "ExeFile not found: "+Game.ExeFile;
                    // set previous dir back
                    Directory.SetCurrentDirectory(cwd);
                    return;
                }
                Proc = System.Diagnostics.Process.Start(Game.ExeFile);
                Proc.Exited += new EventHandler(EvHandlerProcessExited);
                Proc.EnableRaisingEvents = true;
                // set previous dir back after starting process
                Directory.SetCurrentDirectory(cwd);
            
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
                            n++;
                        }                        
                    }
                    
                }

                // when done switch back to our Garden app
                Process p = Process.GetCurrentProcess();
                if (p != null)
                    SetForegroundWindow(p.MainWindowHandle.ToInt32());
            }
            catch (Exception ex)
            {
                status = ITaskStatus.FAIL;
                statusMsg = ex.Message;
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

        /// <summary>
        /// Autodetect the ExeFile and CdPath of 'gi' only if no ExeFile given yet.
        /// Store the detected ExeFile/CdPath into gi.
        /// PRECONDITION: caller has changed / cd'ed to folder gi.GameFolder already.
        /// </summary>
        /// <param name="gi"></param>
        protected void AutoDetectExeFileIfNeeded(GardenItem gi)
        {
            // check if autodetection is needed - only if exe not given
            if (gi.ExeFile.Length > 0)
                return;
            
            // search for .exe files in gamefolder, recursively
            DirectoryInfo gameFolderInfo = new DirectoryInfo(".");
            if(!gameFolderInfo.Exists)
                return;
            FileInfo[] aFi = gameFolderInfo.GetFiles("*.exe", SearchOption.AllDirectories);
            if (aFi.Length == 0)
                return;

            // get the exe's name and store in gi
            FileInfo exeFile = aFi[0];
            gi.ExeFile = exeFile.Name;

            // get the CD path and store in gi
            DirectoryInfo cdPathInfo = exeFile.Directory;
            List<string> dirs = new List<string>();
            while (cdPathInfo.FullName != gameFolderInfo.FullName) {
                dirs.Insert(0, cdPathInfo.Name);
                cdPathInfo = cdPathInfo.Parent;
            }
            if (dirs.Count > 0)
                gi.CdPath = Path.Combine(dirs.ToArray());

        }

    }
}
