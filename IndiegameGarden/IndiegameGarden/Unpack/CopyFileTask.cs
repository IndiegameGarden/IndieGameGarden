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
        string destfolder;

        public CopyFileTask(string filename, string destfolder)
        {
            this.filename = filename;
            this.destfolder = destfolder;
        }

        public override void Start()
        {
            
            FileInfo fi = new FileInfo(filename);
            string destFile = fi.Name;
            destFile = Path.Combine(destfolder , destFile);
            try
            {
                Directory.CreateDirectory(destfolder);
                File.Copy(filename, destFile);
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
            return base.Progress();
        }
    }
}
