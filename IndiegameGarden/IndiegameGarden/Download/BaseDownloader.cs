// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    public abstract class BaseDownloader: ITask
    {
        protected Downloader downloader;
        protected bool isStarted = false;

        public virtual double Progress()
        {
            if (downloader == null)
                return 0;
            return downloader.Progress;
        }

        public virtual bool IsStarted()
        {
            return isStarted;
        }

        public virtual void Start()
        {
            isStarted = true;
        }

        public virtual void Abort()
        {
            if (downloader != null)
            {
                DownloadManager.Instance.RemoveDownload(downloader);
            }
            downloader = null;
        }

        /// <summary>
        /// internal method to start a download without mirrors
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="filename"></param>
        /// <param name="toLocalFolder"></param>
        protected void InternalStartDownload(string urlPath, string filename, string toLocalFolder)
        {
            InternalStartDownload(urlPath,filename,toLocalFolder, new string[] {} );
        }

        /// <summary>
        /// internal method to start a download with mirrors
        /// </summary>
        /// <param name="urlPath">full URL path of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="mirrors">optional set of mirrors for urlPath, may be empty string[] for none</param>
        protected void InternalStartDownload(string urlPath, string filename, string toLocalFolder, string[] mirrors )
        {
            // make sure protocol is specified
            if (!urlPath.Contains("://"))
                urlPath = "http://" + urlPath;

            // starts to listen to the event 'DownloadEnded' from DownloadManager
            DownloadManager.Instance.DownloadEnded += new EventHandler<DownloaderEventArgs>(HandleDownloadEndedEvent);

            string localFile = toLocalFolder + "\\" + filename ;
            // TODO check segments count
            downloader = DownloadManager.Instance.Add(ResourceLocation.FromURL(urlPath), ResourceLocation.FromURLArray(mirrors), localFile, 10, true);

        }

        /// <summary>
        /// called when download task has ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e);

    }
}
