// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Base
{
    /**
     * Task that runs a (typically non-threaded) other Task in a background thread
     */
    public class ThreadedTask: Task
    {
        ITask task;
        Thread thread;

        public ThreadedTask(ITask taskToRun)
        {
            task = taskToRun;
        }

        public override double Progress()
        {
            return task.Progress();
        }

        public override void Start()
        {
            thread = new Thread(new ThreadStart(StartTaskBlocking));
            thread.Start();
        }

        protected void StartTaskBlocking()
        {
            task.Start();
            status = task.Status();
            statusMsg = task.StatusMsg();
        }

        public override void Abort()
        {
            // TODO
            base.Abort();
        }
    }
}
