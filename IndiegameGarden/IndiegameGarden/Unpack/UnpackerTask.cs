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
        private enum PackedFileType { RAR, ZIP, EXE, UNKNOWN } ;
        string filename ;
        string destfolder;
        PackedFileType filetype;
        ITask unpackTask;

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
            if (filename.ToLower().EndsWith(".rar"))
                filetype = PackedFileType.RAR;
            else if (filename.ToLower().EndsWith(".zip"))
                filetype = PackedFileType.ZIP;
            else if (filename.ToLower().EndsWith(".exe"))
                filetype = PackedFileType.EXE;
            else
                filetype = PackedFileType.UNKNOWN;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;         
            // TODO error handling e.g. incomplete archives
            try
            {
                switch (filetype)
                {
                    case PackedFileType.ZIP:
                        unpackTask = new UnzipTask(filename, destfolder);
                        break;
                    case PackedFileType.RAR:
                        unpackTask = new UnrarTask(filename, destfolder);
                        break;
                    case PackedFileType.EXE:
                        unpackTask = new CopyFileTask(filename, destfolder);
                        break;
                    default:
                        throw new NotImplementedException("unpackertask filetype");

                }
                unpackTask.Start();                
                status = unpackTask.Status();
                statusMsg = unpackTask.StatusMsg();
            }
            catch (Exception ex)
            {
                status = ITaskStatus.FAIL;
                statusMsg = ex.ToString();
            }            
        }

        public override double Progress()
        {
            if (status == ITaskStatus.CREATED)
                return 0;
            if (status == ITaskStatus.SUCCESS || status == ITaskStatus.FAIL)
                return 1;
            if (status == ITaskStatus.RUNNING && unpackTask != null)
               return unpackTask.Progress();
            return 0;
        }

    }
}
