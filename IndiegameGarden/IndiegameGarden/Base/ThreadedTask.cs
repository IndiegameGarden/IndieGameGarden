// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Base
{
    public delegate void TaskEventHandler(object sender);

    /**
     * Task that runs a (typically non-threaded) other Task in a background thread
     */
    public class ThreadedTask: Task
    {
        public event TaskEventHandler TaskSuccessEvent;
        public event TaskEventHandler TaskFailEvent;

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
            if (status == ITaskStatus.FAIL && TaskFailEvent != null)
            {
                TaskFailEvent(this);
            }
            if (status == ITaskStatus.SUCCESS && TaskSuccessEvent != null)
            {                
                TaskSuccessEvent(this);
            }
        }

        public override void Abort()
        {
            // TODO
            base.Abort();
        }
    }
}
