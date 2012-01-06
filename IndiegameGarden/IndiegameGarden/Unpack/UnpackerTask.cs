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
        UnzipTask unzipTask;
        UnrarTask unrarTask;        

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

        public override void Start()
        {
            status = ITaskStatus.RUNNING;         
            // TODO error handling e.g. incomplete archives
            try
            {
                switch (filetype)
                {
                    case PackedFileType.ZIP:
                        unzipTask = new UnzipTask(filename, destfolder);
                        unzipTask.Start();
                        break;
                    case PackedFileType.RAR:
                        unrarTask = new UnrarTask(filename, destfolder);
                        unrarTask.Start();
                        break;
                    default:
                        throw new NotImplementedException("unpackertask filetype");

                }
                status = ITaskStatus.SUCCESS;
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
            if (status == ITaskStatus.RUNNING)
            {
                switch (filetype)
                {
                    case PackedFileType.ZIP:
                        if (unzipTask != null)
                        {
                            return unzipTask.Progress();
                        }
                        break;
                    case PackedFileType.RAR:
                        if (unrarTask != null)
                        {
                            return unrarTask.Progress();
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
