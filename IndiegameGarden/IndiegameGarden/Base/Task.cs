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

        public virtual bool IsSuccess()
        {
            return (status == ITaskStatus.SUCCESS);
        }

        // default implementation for Tasks that don't support live progress tracking
        public virtual double Progress()
        {
            if (IsFinished())
                return 1;
            else
                return 0;
        }

        public void Start()
        {
            if (IsStarted())
                return;
            else
            {
                status = ITaskStatus.RUNNING;
                StartInternal();
            }
        }

        protected abstract void StartInternal();
       
        public void Abort()
        {
            if (!IsFinished())
            {
                status = ITaskStatus.FAIL;
                AbortInternal();
            }
        }

        protected abstract void AbortInternal();

    }
}
