// (c) 2010-2014 IndiegameGarden.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices; //required for SetForegroundWindow
using TTengine.Util;
using IndiegameGarden.Base;

namespace IndiegameGarden
{
    public class Launcher : IDisposable
    {
        /// <summary>
        /// where does all data, games, zips etc reside
        /// </summary>
        const string DATA_DIR = "IndiegameGarden_data";

        /// <summary>
        /// where do IndiegameGarden installs (igg* folders) reside inside the data dir?
        /// </summary>
        const string IGG_DIR = "config";

        //const string IGG_PATH = DATA_DIR + "\\" + IGG_DIR;

        /// <summary>
        /// what is the IndiegameGarden executable within the igg* folders ?
        /// </summary>
        const string IGG_EXECUTABLE = "IndiegameGarden.exe";

        /// <summary>
        /// title of any error message box
        /// </summary>
        const string ERROR_BOX_TITLE = "IndiegameGarden Fatal Error (Sorry, it's still in Beta!)";

        //Import the SetForeground API to activate it
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public Launcher()
        {
        }

        public string IggVersionsPath
        {
            get
            {
                return GardenConfig.Instance.ConfigFilesFolder;
            }
        }

        public bool Run()
        {
            try
            {
                string iggDir = FindMostRecentIGGDirectory();
                if (iggDir != null)
                {
                    System.IO.Directory.SetCurrentDirectory(IggVersionsPath);
                    System.IO.Directory.SetCurrentDirectory(iggDir);
                    Process proc = System.Diagnostics.Process.Start(IGG_EXECUTABLE,"-igglaunched");
                    if (proc != null)
                        SetForegroundWindow(proc.MainWindowHandle.ToInt32());
                    return true;
                }
                else
                {
                    //MsgBox.Show(ERROR_BOX_TITLE, "IndiegameGarden's executable not found in " + IGG_PATH + ";\nlikely a damaged installation.");
                    return false;
                }
            }
            catch (Exception)
            {
                //MsgBox.Show(ERROR_BOX_TITLE, ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// finds most recent directory that looks like IndiegameGarden
        /// </summary>
        /// <returns>relative path to most recent IGG directory or null if nothing found</returns>
        string FindMostRecentIGGDirectory()
        {
            string[] dirs;
            try
            {
                dirs = Directory.GetDirectories(IggVersionsPath);
            }
            catch (Exception)
            {
                return null;
            }
            string dirFound = null;

            // check the dirs, are they igg, and which one's most recent
            int versionFound = -1;
            for (int i = 0; i < dirs.Length; i++)
            {
                //string d = dirs[i];
                DirectoryInfo di = new DirectoryInfo(dirs[i]);
                if (di.Name.StartsWith("igg"))
                {
                    // check if exe file's in there
                    if (File.Exists(Path.Combine(dirs[i], IGG_EXECUTABLE)))
                    {
                        int v = ExtractVersionNumber(di.Name);
                        if (v > versionFound)
                        {
                            dirFound = di.Name;
                            versionFound = v;
                        }
                    }
                }
            }

            return dirFound;
        }

        /// <summary>
        /// extract version number info from igg folder name e.g. "igg" or "igg_v23"
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns>version number or -1 if failed</returns>
        int ExtractVersionNumber(string dirName)
        {
            int idx = dirName.LastIndexOf("_v");
            if (idx == -1)
            {
                if (dirName.Equals("igg"))  // version 1 is a special case
                    return 1;
                return -1;
            }
            // check version nr
            return int.Parse(dirName.Substring(idx + 2));
        }

        public void Dispose()
        {
        }
    }
}
