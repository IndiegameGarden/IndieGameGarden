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
            System.IO.Directory.SetCurrentDirectory("Debug");
            Process proc = System.Diagnostics.Process.Start("IndiegameGarden.exe");
            SetForegroundWindow(proc.MainWindowHandle.ToInt32());
        }

        public void Dispose()
        {
        }
    }
}
