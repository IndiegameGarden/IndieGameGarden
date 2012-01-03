// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Download;

namespace IndiegameGarden.Unpack
{    
    /**
     * a task to unpack a packed (.zip, .rar etc) file. Internally uses specific zip/rar/etc. classes.
     */
    public class UnpackerTask: ITask
    {
        bool isStarted = false;
        bool isDone = false;
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

        public void Start()
        {
            isStarted = true;
            isDone = false;
            // TODO error handling e.g. incomplete archives, overwriting files etc.
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
            isDone = true;
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public bool IsStarted()
        {
            return isStarted;
        }

        public bool IsFinished()
        {
            return isDone;
        }

        public double Progress()
        {
            if (isStarted)
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
