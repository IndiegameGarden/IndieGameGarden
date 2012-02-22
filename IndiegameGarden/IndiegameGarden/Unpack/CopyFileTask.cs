// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;

using IndiegameGarden.Base;

namespace IndiegameGarden.Unpack
{
    /**
     * Task that copies a file, creating the destination directory if needed
     */
    public class CopyFileTask: Task
    {

        string filename;
        string destFolder;
        string destFilename;

        public CopyFileTask(string filename, string destFolder, string destFilename)
        {
            this.filename = filename;
            this.destFolder = destFolder;
            this.destFilename = destFilename;
        }

        protected override void StartInternal()
        {            
            try
            {
                FileInfo fi = new FileInfo(filename);
                string destFilePath = fi.Name;
                destFilePath = Path.Combine(destFolder, destFilename);
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                File.Copy(filename, destFilePath);
                status = ITaskStatus.SUCCESS;
            }
            catch (Exception ex)
            {
                status = ITaskStatus.FAIL;
                statusMsg = ex.ToString();
            }
        }

        protected override void AbortInternal()
        {
            // do nothing, can't abort this task once running.
        }
    }
}
