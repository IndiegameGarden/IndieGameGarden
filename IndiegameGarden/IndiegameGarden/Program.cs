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

        static void ReportErrorOverNetwork(Exception ex) {
            try
            {
                string stShort = ex.StackTrace;
                if (stShort.Length > 200)
                    stShort = stShort.Substring(0, 200);
                string u = "http://indieget.appspot.com/err?ex=" + ex.Message + "&ts=" + ex.TargetSite + "&st=" + stShort;
                Downloader downloader = DownloadManager.Instance.Add(ResourceLocation.FromURL(u), new ResourceLocation[] { },
                                                "temp23048230948209348230894432.tmp", 1, true);
            }
            catch (Exception)
            {
                ;
            }

        }
    }
#endif
}

