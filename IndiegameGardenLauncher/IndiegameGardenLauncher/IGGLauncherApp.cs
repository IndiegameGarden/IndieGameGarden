// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices; //required for SetForegroundWindow

namespace IndiegameGardenLauncher
{    
    public class IGGLauncherApp : IDisposable
    {
        //Import the SetForeground API to activate it
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public IGGLauncherApp()
        {
        }

        public void Run()
        {
            //System.IO.Directory.SetCurrentDirectory("data");
            if (FindMostRecentIGGDirectory())
            {
                Process proc = System.Diagnostics.Process.Start("IndiegameGarden.exe");
                if (proc != null)
                    SetForegroundWindow(proc.MainWindowHandle.ToInt32());
            }
        }

        bool FindMostRecentIGGDirectory()
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory("data\\games");
                int v = 1;
                for (v = 1; v < 999999; v++)
                {
                    string dir = "igg_v" + v;
                    if (!System.IO.Directory.Exists(dir))
                    {
                        v--;
                        break;
                    }
                }
                if (v == 0)
                    return false;
                System.IO.Directory.SetCurrentDirectory("igg_v" + v);
                return true;
            }
            catch (Exception)
            {
                // TODO a message box?
                return false;
            }
        }

        public void Dispose()
        {
        }
    }
}
