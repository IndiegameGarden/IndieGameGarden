// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Util;
using MyDownloader.Core;

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
                using (GardenGame game = new GardenGame())
                {
                    Form frm = (Form)Form.FromHandle(game.Window.Handle);
                    frm.FormBorderStyle = FormBorderStyle.None;
                    game.Run();
                }
            }
            catch(Exception ex) {
                ReportErrorOverNetwork(ex);
                MessageBox.Show("Critical error - sorry! " + ex.Message + "\n" + ex.ToString(), "IndiegameGarden: critical error (well, I'm just an ALPHA version!)");                
            }
        }

        /// <summary>
        /// misuse HTTP GET to deliver an error report to the server log
        /// </summary>
        /// <param name="ex"></param>
        static void ReportErrorOverNetwork(Exception ex) {
            try
            {
                const int MAX_URL_LENGTH = 580;
                string u = "http://indieget.appspot.com/err?ex=" + ex + "&ts=" + ex.TargetSite + "&st=" + ex.StackTrace;
                u = u.Replace(' ', '-'); // avoid the %20 substitution to save space
                u = u.Replace('\\', '/');
                u = u.Replace("\r", "");
                u = u.Replace("\n", "");
                u = u.Replace('\t', '-');
                if (u.Length > MAX_URL_LENGTH)
                    u = u.Substring(0, MAX_URL_LENGTH);
                Downloader downloader = DownloadManager.Instance.Add(ResourceLocation.FromURL(u), new ResourceLocation[] { },
                                                "dummy_file_should_not_be_created_23048230948209348230894432.tmp", 1, true);
            }
            catch (Exception)
            {
                ;
            }

        }
    }
#endif
}

