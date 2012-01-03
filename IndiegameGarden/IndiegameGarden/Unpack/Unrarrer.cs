// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Download;

namespace IndiegameGarden.Unpack
{
    /**
     * a wrapper around Unrar to support progress indication better and offer a 
     * similar interface as Unzipper.
     */
    public class Unrarrer
    {
        string filename;
        string destfolder;
        double progress = 0;
        Unrar unrar;

        public Unrarrer(string filename, string destfolder)
        {
            this.filename = filename;
            this.destfolder = destfolder;
        }

        public double Progress
        {
            get
            {
                return progress;
            }
        }

        public void Unrar()
        {
            unrar = new Unrar(filename);
            unrar.Open();
            //unrar.DataAvailable
            unrar.ExtractionProgress += new ExtractionProgressHandler(EvHandlerExtractionProgress);
            while (unrar.ReadHeader())
            {
                unrar.ExtractToDirectory(destfolder);
            }
            unrar.Close();
        }

        void EvHandlerExtractionProgress(object sender, ExtractionProgressEventArgs e)
        {
            double p = e.PercentComplete;
            progress = p; // TODO
        }

    }
}
