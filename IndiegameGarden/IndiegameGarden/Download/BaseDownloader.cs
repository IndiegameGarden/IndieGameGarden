// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Threading;
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
        /// <summary>
        /// number of times the download may be tried (first try including retries)
        /// </summary>
        public int MaxRetries = Settings.Default.MaxRetries;

        protected Downloader downloader;
        protected string localFile;
        protected int segmentsUsedInDownload = 1;

        public override double Progress()
        {
            if (downloader == null)
                return 0;

            if (IsFinished())
                return 1;

            return downloader.Progress / 100;
        }

        public virtual double DownloadSpeed()
        {
            if (downloader==null)
                return 0;
            return downloader.Rate;
        }

        protected override void StartInternal()
        {
            throw new NotImplementedException("BaseDownloader subclasses should implement/override Start()");
        }

        protected override void AbortInternal()
        {
            if (downloader != null )
            {
                if (IsRunning())
                {
                    DownloadManager.Instance.RemoveDownload(downloader);
                    downloader.WaitForConclusion();
                    status = ITaskStatus.FAIL;
                }

                // try to delete file if an error occurred in download
                if (IsFinished() && !IsSuccess())
                {
                    try
                    {
                        downloader.WaitForConclusion();
                        if (localFile != null && File.Exists(localFile))
                        {
                            try { 
                                File.Delete(localFile); 
                            }
                            catch (Exception) 
                            {
                                try
                                {
                                    Thread.Sleep(100);
                                    File.Delete(localFile);
                                }
                                catch (Exception)
                                {
                                    ;
                                }                                
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
            }
            downloader = null;
            status = ITaskStatus.FAIL;
        }

        /// <summary>
        /// class-internal method to perform a download without mirrors. See description of InternalDoDownload() method
        /// with mirrors.
        /// </summary>
        /// <param name="urlPath">full URL of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="overwriteExisting">if true, overwrites any existing file 'filename'</param>
        protected void InternalDoDownload(string urlPath, string filename, string toLocalFolder, bool overwriteExisting)
        {
            InternalDoDownload(urlPath,filename,toLocalFolder, overwriteExisting, new string[] {} );
        }

        /// <summary>
        /// perform a download with mirrors, retrying each mirror in case main url fails. 
        /// Has blocking wait and sets ITask status to FAIL in case of failure.
        /// </summary>
        /// <param name="urlPath">full URL of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="overwriteExisting">if true, overwrites any existing file 'filename'</param>
        /// <param name="mirrors">optional set of mirrors for urlPath, may be empty string[] for none</param>
        protected void InternalDoDownload_MirrorRetry(string urlPath, string filename, string toLocalFolder, bool overwriteExisting, string[] mirrors)
        {
            InternalDoDownload(urlPath, filename, toLocalFolder, overwriteExisting, mirrors);
            if (status == ITaskStatus.SUCCESS)
                return;
            status = ITaskStatus.RUNNING; // reset back status from FAIL to running
            for (int i = 0; i < mirrors.Length; i++)
            {
                // try each one of the mirrors as main url
                InternalDoDownload(mirrors[i], filename, toLocalFolder, overwriteExisting, mirrors);
                if (status == ITaskStatus.SUCCESS)
                    return;
                status = ITaskStatus.RUNNING; // reset back status from FAIL to running
            }
            status = ITaskStatus.FAIL;
        }

        /// <summary>
        /// class-internal method to perform a download with mirrors. Has blocking wait and sets ITask status to FAIL
        /// in case of failure.
        /// </summary>
        /// <param name="urlPath">full URL of file, optionally leaving out protocol http://</param>
        /// <param name="filename">local name under which to store the file</param>
        /// <param name="toLocalFolder">local folder where to store file</param>
        /// <param name="overwriteExisting">if true, overwrites any existing file 'filename'</param>
        /// <param name="mirrors">optional set of mirrors for urlPath, may be empty string[] for none</param>
        protected void InternalDoDownload(string urlPath, string filename, string toLocalFolder, bool overwriteExisting, string[] mirrors)
        {
            localFile = toLocalFolder + "\\" + filename;
            
            // check if file already there and overwriting is unwanted.
            if (File.Exists(localFile) && !overwriteExisting)
            {
                status = ITaskStatus.SUCCESS;
                return; // yes we're done! no download needed
            }

            // make sure protocol is specified
            if (!urlPath.Contains("://"))
                urlPath = "http://" + urlPath;
            
            // construct temp file names
            string tempFile;
            tempFile = Path.GetTempPath() + "IndiegameGarden_" + Path.GetRandomFileName();
            TryDeleteFile(tempFile);

            downloader = DownloadManager.Instance.Add(  ResourceLocation.FromURL(urlPath), 
                                                        ResourceLocation.FromURLArray(mirrors),
                                                        tempFile, segmentsUsedInDownload, false);
            
            if (downloader != null)
            {
                downloader.MaxRetries = MaxRetries;
                downloader.Start();
                if (downloader != null) // null may happen, see below
                {

                    downloader.WaitForConclusion();
                    if (downloader == null)  // null may happen! (on basedownloader cleanup in other thread)
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "Download aborted";
                    }
                    else if (!downloader.State.Equals(DownloaderState.Ended))
                    {
                        if (downloader.LastError != null)
                            statusMsg = downloader.LastError.Message;
                        else
                            statusMsg = "Download aborted or timed out";
                        status = ITaskStatus.FAIL;
                    }
                    else
                        status = ITaskStatus.SUCCESS;

                    // remove the temp file if failed
                    if (File.Exists(tempFile) && !IsSuccess())
                    {
                        TryDeleteFile(tempFile);
                    }
                    // move temp file to localFile on success
                    else if (IsSuccess())
                    {
                        try
                        {
                            TryDeleteFile(localFile);
                            File.Move(tempFile, localFile);
                            status = ITaskStatus.SUCCESS;
                        }
                        catch (Exception ex)
                        {
                            status = ITaskStatus.FAIL;
                            statusMsg = "Couldn't move downloaded file to " + localFile + ": " + ex.ToString();
                        }
                        finally {
                            TryDeleteFile(tempFile);
                        }
                    }
                }
            }
            else
            {
                status = ITaskStatus.FAIL;
                statusMsg = "failed to create downloader by DownloadManager.";
            }
        }

        private bool TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
