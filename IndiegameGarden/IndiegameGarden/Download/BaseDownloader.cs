// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

using IndiegameGarden.Base;
namespace IndiegameGarden.Download
{
    /**
     * A base class for various file downloader classes. Wraps a MyDownloader.Downloader as a ITask
     */
    public abstract class BaseDownloader: Task
    {
        protected Downloader downloader;
        protected string localFile;

        // TODO check if allowed/needed
        ~BaseDownloader()
        {
            Abort();
        }

        public override double Progress()
        {
            if (downloader == null)
                return 0;
            
            return downloader.Progress / 100;
        }

        /// <summary>
        /// actual download operations should be put in the Start() method
        /// </summary>
        public override void Start()
        {
            throw new NotImplementedException("BaseDownloader subclasses should implement/override Start()");
        }

        public override void Abort()
        {
            if (downloader != null)
            {
                DownloadManager.Instance.RemoveDownload(downloader);
                // try to delete file
                try
                {
                    if(localFile != null)
                        File.Delete(localFile);
                }
                catch (Exception)
                {
                    ;
                }
            }
            downloader = null;
            status = ITaskStatus.FAIL;
        }

        /// <summary>
        /// class-internal method to perform a download without mirrors. See description of InternalDoDownload() method
        /// with mirrors.
        /// </summary>
        /// <param name="urlPath">full URL gameDirPath of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="overwriteExisting">if true, overwrites any existing file 'filename'</param>
        protected void InternalDoDownload(string urlPath, string filename, string toLocalFolder, bool overwriteExisting)
        {
            InternalDoDownload(urlPath,filename,toLocalFolder, overwriteExisting, new string[] {} );
        }

        /// <summary>
        /// class-internal method to perform a download with mirrors. Has blocking wait and sets ITask status to FAIL
        /// in case of failure.
        /// </summary>
        /// <param name="urlPath">full URL gameDirPath of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="mirrors">optional set of mirrors for urlPath, may be empty string[] for none</param>
        /// <param name="overwriteExisting">if true, overwrites any existing file 'filename'</param>
        protected void InternalDoDownload(string urlPath, string filename, string toLocalFolder, bool overwriteExisting, string[] mirrors )
        {
            // make sure protocol is specified
            if (!urlPath.Contains("://"))
                urlPath = "http://" + urlPath;
            
            localFile = toLocalFolder + "\\" + filename ;

            // check if file already there
            if (File.Exists(localFile))
            {
                if (!overwriteExisting)
                {                    
                    return; // we're done! file is there already.
                }
                else
                {
                    // remove existing file - downloader puts a new one there.
                    try
                    {
                        File.Delete(localFile);
                    }
                    catch (Exception)
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "Could not delete " + localFile;
                        return;
                    }
                }
            }

            // TODO check segments count
            downloader = DownloadManager.Instance.Add(  ResourceLocation.FromURL(urlPath), 
                                                        ResourceLocation.FromURLArray(mirrors), 
                                                        localFile, 3, true);
            if (downloader != null)
            {
                downloader.WaitForConclusion();
                if (!downloader.State.Equals(DownloaderState.Ended))
                    status = ITaskStatus.FAIL;
                else
                    status = ITaskStatus.SUCCESS;
            }
            else
            {
                status = ITaskStatus.FAIL;
            }
        }

    }
}
