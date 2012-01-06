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
        protected ITaskStatus status = ITaskStatus.CREATED;
        protected string statusMsg = "";

        public virtual ITaskStatus Status()
        {
            return status;
        }

        public string StatusMsg()
        {
            return statusMsg;
        }

        public virtual bool IsStarted()
        {
            return status != ITaskStatus.CREATED;
        }

        public virtual bool IsRunning()
        {
            return status == ITaskStatus.RUNNING;
        }

        public virtual bool IsFinished()
        {
            return (status == ITaskStatus.SUCCESS) || (status == ITaskStatus.FAIL);
        }

        // default implementation for Tasks that don't support live progress tracking
        public virtual double Progress()
        {
            if (IsFinished())
                return 1;
            else
                return 0;
        }

        public abstract void Start();

        public virtual void Abort()
        {
            // to override if wished
            throw new NotImplementedException("Abort() not implemented for this task");
        }

    }
}
