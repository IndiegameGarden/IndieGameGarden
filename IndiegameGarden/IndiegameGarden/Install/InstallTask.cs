// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndiegameGarden.Download;

namespace IndiegameGarden.Install
{
    /// <summary>
    /// a task to install a game or program from a compressed file format
    /// </summary>
    public class InstallTask: ITask
    {
        bool isStarted = false;

        public void Start()
        {
        }

        public void Abort()
        {
        }

        public double Progress()
        {
            return 0;
        }

        public bool IsStarted()
        {
            return isStarted;
        }

    }
}
