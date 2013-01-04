// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Threading;
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
        static string CRITICAL_ERROR_MSG = "Critical error - just reported to our server in the Google cloud, to help fix it.\n";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                using (GardenGame game = new GardenGame())
                {
                    game.Run();
                }
            }
            catch(Exception ex) {
                ReportErrorOverNetwork(ex);
                MsgBox.Show("Indiegame Garden: critical error",
                            CRITICAL_ERROR_MSG + ex.Message + "\n" + ex.ToString() );                
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                ReportErrorOverNetwork(ex); 
                MsgBox.Show("Indiegame Garden: critical error",
                            CRITICAL_ERROR_MSG + ex.Message + "\n" + ex.ToString());
            }
            finally
            {
                ;
            }
        }   

        /*
        static void ReportErrorHttpPost(Exception ex)
        {
            string u = "http://indieget.appspot.com/errPost" ;
            string payload = ex + "\n" + ex.TargetSite + "\n" + ex.StackTrace;
            HttpPost.HttpPostText(u,payload);
        }
         */

        static void ReportErrorOverNetwork(Exception ex)
        {
            ITask t = new ThreadedTask(new ReportErrorOverNetworkTask(ex));
            t.Start();
        }

        /// <summary>
        /// Tasks that misuses HTTP GET to deliver an error report to the server log
        /// </summary>
        class ReportErrorOverNetworkTask: Task 
        {
            Exception ex;  // exception to report

            public ReportErrorOverNetworkTask(Exception ex)
            {
                this.ex = ex;
            }

            protected override void StartInternal()
            {
                try
                {
                    const int MAX_URL_LENGTH = 2000;
                    string u = "http://indieget.appspot.com/err?v=" + GardenConfig.Instance.ClientVersion + "&ex=" + ex + "&ts=" + ex.TargetSite + "&st=" + ex.StackTrace;
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

            protected override void AbortInternal()
            {
                throw new NotImplementedException();
            }
        }
    }
#endif
}

