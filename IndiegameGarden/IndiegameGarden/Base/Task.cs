using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Base
{
    /**
     * a base skeleton implementation of a Task, implementing ITask
     */
    public abstract class Task: ITask
    {
        protected ITaskStatus status = ITaskStatus.IDLE;

        public virtual ITaskStatus Status()
        {
            return status;
        }

        public virtual bool IsStarted()
        {
            return status != ITaskStatus.IDLE;
        }

        public virtual bool IsFinished()
        {
            return (status == ITaskStatus.FINISHED) || (status == ITaskStatus.FAILED);
        }

        public abstract double Progress();
        public abstract void Start();
        public abstract void Abort();

    }
}
