// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ionic.Zip;

namespace IndiegameGarden.Unpack
{
    /**
     * wraps the Ionic.Zip unzipping library to get ease of use and progress indication
     */
    public class Unzipper
    {
        public string zipfile;
        public string destdir;
        double progress = 0;

        public Unzipper(string zipfile, string destdir)
        {
            this.zipfile = zipfile;
            this.destdir = destdir;
        }

        public double Progress
        {
            get{
                return progress;
            }
        }

        public void Unzip()
        {
            try
            {
                // Specifying Console.Out here causes diagnostic msgs to be sent to the Console
                // In a WinForms or WPF or Web app, you could specify nothing, or an alternate
                // TextWriter to capture diagnostic messages.

                var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                using (ZipFile zip = ZipFile.Read(zipfile, options))
                {
                    zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(EvHandlerExtractProgress);
                    // This call to ExtractAll() assumes:
                    //   - none of the entries are password-protected.
                    //   - want to extract all entries to current working directory
                    //   - none of the files in the zip already exist in the directory;
                    //     if they do, the method will throw.
                    zip.ExtractAll(destdir);
                }
            }
            catch (System.Exception ex1)
            {
                throw(ex1);
            }

        }

        void EvHandlerExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer >= 0)
            {
                progress = ((double)e.BytesTransferred) / ((double)e.TotalBytesToTransfer);
            }
        }
    }
}
