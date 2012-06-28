// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Util;
using MyDownloader.Core;
using IndiegameGarden.Base;
using IndiegameGarden.Util;

namespace IndiegameGarden
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    // behave as a launcher (potentially)
                    using (Launcher launcher = new Launcher())
                    {
                        bool couldLaunch = launcher.Run();
                        if (!couldLaunch)
                        {
                            using (GardenGame game = new GardenGame())
                            {
                                game.Run();
                            }
                        }
                    }
                }
                else
                {
                    using (GardenGame game = new GardenGame())
                    {
                        game.Run();
                    }
                }
            }
            catch(Exception ex) {
                ReportErrorOverNetwork(ex);
                MsgBox.Show("IndiegameGarden: critical error",
                            "Critical error - sorry! " + ex.Message + "\n" + ex.ToString() );                
            }
        }

        static void ReportErrorHttpPost(Exception ex)
        {
            string u = "http://indieget.appspot.com/errPost" ;
            string payload = ex + "\n" + ex.TargetSite + "\n" + ex.StackTrace;
            HttpPost.HttpPostText(u,payload);
        }

        /// <summary>
        /// misuse HTTP GET to deliver an error report to the server log
        /// </summary>
        /// <param name="ex"></param>
        static void ReportErrorOverNetwork(Exception ex) {
            try
            {
                const int MAX_URL_LENGTH = 2000;
                string u = "http://indieget.appspot.com/err?ex=" + ex + "&ts=" + ex.TargetSite + "&st=" + ex.StackTrace;
                u = u.Replace(' ', '-'); // avoid the %20 substitution to save space
                u = u.Replace('\\', '/');
                u = u.Replace("\r", "");
                u = u.Replace("\n", "-");
                u = u.Replace('\t', '-');
                u = u.Replace("----", "-"); // remove excess space
                u = u.Replace("---", "-");
                u = u.Replace("--", "-");
                u = u.Replace("--", "-");
                u = u.Replace("Microsoft.Xna.Framework", "XNA");
                u = u.Replace("IndiegameGarden", "IGG");
                u = u.Replace("Exception", "EX");

                if (u.Length > MAX_URL_LENGTH)
                    u = u.Substring(0, MAX_URL_LENGTH);
                //Downloader downloader = DownloadManager.Instance.Add(ResourceLocation.FromURL(u), new ResourceLocation[] { },
                //                                "dummy_file_should_not_be_created_23048230948209348230894432.tmp", 1, true);
                HttpPost.HttpPostText(u, "x");
            }
            catch (Exception)
            {
                ;
            }

        }
    }
#endif
}

