// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using IndiegameGarden.Download;
using IndiegameGarden.Store;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// launches a game .exe and keeps track of when it has finished
    /// </summary>
    public class GameLauncher: ITask
    {
        public Process Proc = null;
        public IndieGame Game;

        string filePath;
        string cdPath;
        bool isStarted = false;
        bool isDone = false;
        bool isFailed = false;

        public GameLauncher(IndieGame g)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            cdPath = cwd + "\\" + GardenMain.Instance.storageConfig.UnpackedFilesFolder + "\\" + g.GameID + "\\" + g.CdPath;
            filePath = g.ExeFile;
        }

        public void Start()
        {
            isStarted = true;

            try
            {
                string cwd = System.IO.Directory.GetCurrentDirectory();
                System.IO.Directory.SetCurrentDirectory(cdPath);
                Proc = System.Diagnostics.Process.Start(filePath);
                Proc.Exited += new EventHandler(processExitedEvent);
                Proc.EnableRaisingEvents = true;
                System.IO.Directory.SetCurrentDirectory(cwd);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                isFailed = true;
            }
            catch (System.ObjectDisposedException)
            {
                isFailed = true;
            }
            catch (System.IO.FileNotFoundException)
            {
                isFailed = true;
            }

        }

        public void Abort()
        {
            throw new NotImplementedException("Abort() method");
        }

        public double Progress()
        {
            if (IsDone())
                return 1.0;
            return 0.0;
        }

        public bool IsStarted()
        {
            return isStarted;
        }

        public bool IsDone()
        {
            return isDone || isFailed;
        }

        private void processExitedEvent(object sender, System.EventArgs e)
        {
            isDone = true;
        }

    }
}
