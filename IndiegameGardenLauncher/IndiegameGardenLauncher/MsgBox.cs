// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Runtime.InteropServices; //required for DllImport

namespace IndiegameGardenLauncher
{
    public class MsgBox
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        /**
         * Shows a Windows MessageBox with title and content 'msg'
         */
        public static void Show(String windowTitle, String msg)
        {
            MessageBox(new IntPtr(0), msg, windowTitle, 0);
        }
    }
}
