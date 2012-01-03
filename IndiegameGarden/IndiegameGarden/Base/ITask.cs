// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

namespace IndiegameGarden.Base
{
    /// <summary>
    /// task status indication: IDLE if not yet started, STARTED when started, FINISHED when 
    /// successfully concluded and FAILED on error/abort/etc.
    /// </summary>
    public enum ITaskStatus { IDLE, STARTED, FINISHED, FAILED } 

    /**
     * <summary>
     * a task that takes time such as loading, downloading, installing or processing
     * </summary>
     */
    public interface ITask
    {
        /// <summary>
        /// Get task progress indication value. IsFinished()==true implies Progress()==1 and vice versa.
        /// IsStarted()==false implies Progress()==0 but not vice versa.
        /// </summary>
        /// <returns>progress indication between 0...1</returns>
        double Progress();

        /// <summary>
        /// get the current Task status
        /// </summary>
        /// <returns>one of ITaskStatus status indications</returns>
        ITaskStatus Status();

        /// <summary>
        /// Start this task. Calling this while the task is started should have no effect.
        /// </summary>
        void Start();

        /// <summary>
        /// Abort this task (which may or may not be already running when this is called)
        /// </summary>
        void Abort();

        /// <summary>
        /// check whether Task is already started or not
        /// </summary>
        /// <returns>true if started, false otherwise</returns>
        bool IsStarted();

        /// <summary>
        /// check whether Task is already finished or not
        /// </summary>
        /// <returns>true if finished, false otherwise</returns>
        bool IsFinished();
    }
}
