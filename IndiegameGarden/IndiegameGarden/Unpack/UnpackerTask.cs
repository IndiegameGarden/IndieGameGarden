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
     * a task to unpack a packed (.zip, .rar etc) file. Internally uses specific zip/rar/etc. classes.
     */
    public class UnpackerTask: Task
    {
        private enum PackedFileType { RAR, ZIP, UNKNOWN } ;
        string filename ;
        string destfolder;
        PackedFileType filetype;
        Unzipper unzipper;
        Unrarrer unrarrer;
        

        public UnpackerTask(string filename, string destfolder)
        {
            this.filename = filename;
            this.destfolder = destfolder;
            DetectFileType();
        }

        /// <summary>
        /// the filename of the file that is unpacked by this task
        /// </summary>
        public string Filename
        {
            get
            {
                return filename;
            }
        }

        private void DetectFileType()
        {
            if (filename.EndsWith(".rar"))
                filetype = PackedFileType.RAR;
            else if (filename.EndsWith(".zip"))
                filetype = PackedFileType.ZIP;
            else
                filetype = PackedFileType.UNKNOWN;
        }

        /// <summary>
        /// NOTE: Method blocks until task FINISHED
        /// </summary>
        public override void Start()
        {
            status = ITaskStatus.STARTED;         
            // TODO error handling e.g. incomplete archives
            try
            {
                switch (filetype)
                {
                    case PackedFileType.ZIP:
                        unzipper = new Unzipper(filename, destfolder);
                        unzipper.Unzip();
                        break;
                    case PackedFileType.RAR:
                        unrarrer = new Unrarrer(filename, destfolder);
                        unrarrer.Unrar();
                        break;
                    default:
                        throw new NotImplementedException("unpackertask filetype");

                }
                status = ITaskStatus.FINISHED;
            }
            catch (Exception ex)
            {
                status = ITaskStatus.FAILED;
                msg = ex.ToString();
            }            
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override double Progress()
        {
            if (status == ITaskStatus.IDLE)
                return 0;
            if (status == ITaskStatus.FINISHED || status == ITaskStatus.FAILED)
                return 1;
            if (status == ITaskStatus.STARTED)
            {
                switch (filetype)
                {
                    case PackedFileType.ZIP:
                        if (unzipper != null)
                        {
                            return unzipper.Progress;
                        }
                        break;
                    case PackedFileType.RAR:
                        if (unrarrer != null)
                        {
                            return unrarrer.Progress;
                        }
                        break;
                    default:
                        throw new NotImplementedException("unpackertask filetype");
                }
            }
            return 0;
        }

    }
}
