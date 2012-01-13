// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ionic.Zip;
using IndiegameGarden.Base;

namespace IndiegameGarden.Unpack
{
    /**
     * wraps the Ionic.Zip unzipping library as a ITask, task to unzip a single archive
     */
    public class UnzipTask: Task
    {
        public string zipfile;
        public string destdir;
        long bytesDoneWithinFile = 0;
        long totalSize = 0;
        long doneSize = 0;


        public UnzipTask(string zipfile, string destdir)
        {
            this.zipfile = zipfile;
            this.destdir = destdir;
        }

        public override double Progress()
        {
            if (totalSize > 0)
                return ((double)(doneSize + bytesDoneWithinFile)) / ((double)totalSize);
            else
                return 0;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;
            try
            {
                // Specifying Console.Out here causes diagnostic msgs to be sent to the Console
                // In a WinForms or WPF or Web app, you could specify nothing, or an alternate
                // TextWriter to capture diagnostic messages.
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out }; // TODO change to another writer
                using (ZipFile zip = ZipFile.Read(zipfile, options))
                {
                    //zip.Entries
                    ICollection<ZipEntry> zipFiles = zip.Entries;
                    foreach (ZipEntry ze in zipFiles)
                    {
                        totalSize += ze.UncompressedSize;
                    }
                    
                    zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(EvHandlerExtractProgress);

                    foreach (ZipEntry ze in zipFiles)
                    {
                        ze.Extract(destdir);
                        doneSize += ze.UncompressedSize;
                    }

                    // This call to ExtractAll() assumes:
                    //   - none of the entries are password-protected.
                    //   - want to extract all entries to current working directory
                    //   - none of the files in the zip already exist in the directory;
                    //     if they do, the method will throw.
                    //zip.ExtractAll(destdir);
                }
                status = ITaskStatus.SUCCESS;
            }
            catch (System.Exception ex1)
            {
                status = ITaskStatus.FAIL;
                statusMsg = ex1.ToString();
            }

        }

        // called by the ZipFile class progressContributionSingleFile event
        void EvHandlerExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            try
            {
                if (e.EventType.Equals(ZipProgressEventType.Extracting_EntryBytesWritten))
                {
                    bytesDoneWithinFile = e.BytesTransferred;
                }
            }
            catch(Exception)
            {
                ;
            }
        }
    }
}
