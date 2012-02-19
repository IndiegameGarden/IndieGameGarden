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
     * a task to unpack a packed (.zip, .rar etc) file. Internally uses specific zip/rar/exe/etc. classes.
     */
    public class UnpackerTask: Task
    {
        private enum PackedFileType { RAR, ZIP, EXE_NOT_PACKED, EXE_SELFEXTRACTING, UNKNOWN } ;
        string filename ;
        string destFolder;
        string exeFile;
        PackedFileType fileType;
        ITask unpackTask;

        /// <summary>
        /// create a new unpacker task for a given file and destination folder to unpack to.
        /// </summary>
        /// <param name="gameExeFile">the relative path to the game's .exe file used e.g. to test unpack result</param>
        public UnpackerTask(string filename, string destFolder, string gameExeFile)
        {
            this.filename = filename;
            this.destFolder = destFolder;
            this.exeFile = gameExeFile;
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
                fileType = PackedFileType.RAR;
            else if (filename.ToLower().EndsWith(".zip"))
                fileType = PackedFileType.ZIP;
            else if (filename.ToLower().EndsWith(".exe"))
                fileType = PackedFileType.EXE_NOT_PACKED;
            else
                fileType = PackedFileType.UNKNOWN;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;         
            // TODO error handling e.g. incomplete archives
            try
            {
                switch (fileType)
                {
                    case PackedFileType.ZIP:
                        unpackTask = new UnzipTask(filename, destFolder);
                        break;
                    case PackedFileType.RAR:
                        unpackTask = new UnrarTask(filename, destFolder);
                        break;
                    case PackedFileType.EXE_NOT_PACKED:
                        unpackTask = new CopyFileTask(filename, destFolder, exeFile);
                        break;
                    default:
                        throw new NotImplementedException("unpackertask fileType");

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

        public override void Abort()
        {
            if (status == ITaskStatus.SUCCESS || status == ITaskStatus.FAIL)
                return;
            if (unpackTask != null)
                unpackTask.Abort();
            status = ITaskStatus.FAIL;
            statusMsg = "aborted task";
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
