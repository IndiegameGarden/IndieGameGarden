// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    /**
     * A base class for various file downloader classes
     */
    public abstract class BaseDownloader: ITask
    {
        protected Downloader downloader;
        protected bool isStarted = false;
        protected bool isFinished = false;

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

        public virtual bool IsFinished()
        {
            return isFinished;
        }

        public virtual void Start()
        {
            isStarted = true;
            isFinished = false;
        }

        public virtual void Abort()
        {
            if (downloader != null)
            {
                DownloadManager.Instance.RemoveDownload(downloader);
            }
            downloader = null;
            isFinished = true;
        }

        /// <summary>
        /// class-internal method to start a download without mirrors
        /// </summary>
        /// <param name="urlPath">full URL path of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        protected void InternalStartDownload(string urlPath, string filename, string toLocalFolder)
        {
            InternalStartDownload(urlPath,filename,toLocalFolder, new string[] {} );
        }

        /// <summary>
        /// class-internal method to start a download with mirrors
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
            DownloadManager.Instance.DownloadEnded += new EventHandler<DownloaderEventArgs>(EvHandleDownloadEnded);

            string localFile = toLocalFolder + "\\" + filename ;
            // TODO check segments count
            downloader = DownloadManager.Instance.Add(  ResourceLocation.FromURL(urlPath), 
                                                        ResourceLocation.FromURLArray(mirrors), 
                                                        localFile, 3, true);

        }

        // called by MyDownloader framework upon any download ready/error etc.
        private void EvHandleDownloadEnded(object sender, DownloaderEventArgs e)
        {
            // check if it is my download
            if (e.Downloader == downloader)
            {
                OnDownloadEnded(e.Downloader);
            }
        }

        /// <summary>
        /// called when download task has ended. Can use dl.State to check state of the finished download (e.g. error, success, etc.)
        /// </summary>
        /// <param name="dl">The Downloader class of the file whose downloading has ended</param>
        public abstract void OnDownloadEnded(Downloader dl);

    }
}
