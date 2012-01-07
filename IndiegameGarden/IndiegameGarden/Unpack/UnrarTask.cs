// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Base;
using IndiegameGarden.Download;

namespace IndiegameGarden.Unpack
{
    /**
     * Task that Unrars a .rar archive using the Unrar class
     */
    public class UnrarTask: Task
    {
        string filename;
        string destfolder;
        double progress = 0;
        Unrar unrar;

        public UnrarTask(string filename, string destfolder)
        {
            this.filename = filename;
            this.destfolder = destfolder;
        }

        public override double Progress()
        {
            return progress;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;
            unrar = new Unrar(filename);
            unrar.Open();
            unrar.ExtractionProgress += new ExtractionProgressHandler(EvHandlerExtractionProgress);
            try
            {
                while (unrar.ReadHeader())
                {
                    unrar.ExtractToDirectory(destfolder);
                }
                status = ITaskStatus.SUCCESS;
            }
            catch (Exception ex)
            {
                status = ITaskStatus.FAIL;
                statusMsg = ex.ToString();
            }
            finally{
                try
                {
                    unrar.Close();
                }
                catch (Exception ex)
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = ex.ToString();
                }
            }
        }

        // called from the Unrar.cs event handler
        void EvHandlerExtractionProgress(object sender, ExtractionProgressEventArgs e)
        {
            double p = e.PercentComplete;
            progress = p; // TODO
        }

    }
}
