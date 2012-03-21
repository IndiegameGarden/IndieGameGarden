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
     * Task that runs a (typically non-threaded) other Task in a background thread. It provides
     * event notification upon task success or task failure
     */
    public class ThreadedTask: Task
    {
        /// <summary>
        /// event notification when task succeeds
        /// </summary>
        public event TaskEventHandler TaskSuccessEvent;

        /// <summary>
        /// event notification when task fails
        /// </summary>
        public event TaskEventHandler TaskFailEvent;

        ITask task;
        Thread thread;

        /// <summary>
        /// returns the embedded ITask that is being run in a thread
        /// </summary>
        public ITask TaskToRun
        {
            get
            {
                return task;
            }
        }

        /// <summary>
        /// create new task thread which runs taskToRun when Started.
        /// </summary>
        /// <param name="taskToRun"></param>
        public ThreadedTask(ITask taskToRun)
        {
            task = taskToRun;
        }

        ~ThreadedTask()
        {
            Abort();
        }

        public override double Progress()
        {
            return task.Progress();
        }

        protected override void StartInternal()
        {
            thread = new Thread(new ThreadStart(StartTaskBlocking));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        protected void StartTaskBlocking()
        {
            try
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
            catch (ThreadAbortException)
            {
                status = ITaskStatus.FAIL;
                if (TaskFailEvent != null)
                    TaskFailEvent(this);
            }
        }

        /// <summary>
        /// aborts the thread running the task in a blocking way 
        /// </summary>
        protected override void AbortInternal()
        {
            if (task != null)
            {
                task.Abort();
            }

            /*
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
             * */
            if (thread != null)
                thread.Join();
        }
    }
}
