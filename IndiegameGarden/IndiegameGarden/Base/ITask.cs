// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

namespace IndiegameGarden.Base
{
    /// <summary>
    /// task status indication: CREATED if not yet started, RUNNING when started, SUCCESS when 
    /// successfully concluded and FAIL on error/abort/etc.
    /// </summary>
    public enum ITaskStatus { CREATED, RUNNING, SUCCESS, FAIL } 

    /**
     * <summary>
     * a task that takes time such as loading, downloading, installing or processing
     * </summary>
     */
    public interface ITask
    {
        /// <summary>
        /// Get task progressContributionSingleFile indication value. IsFinished()==true implies ProgressTarget()==1 and vice versa.
        /// IsStarted()==false implies ProgressTarget()==0 but not vice versa.
        /// </summary>
        /// <returns>progressContributionSingleFile indication between 0...1. Returns either 0 or 1 without in-between values
        /// for tasks where progressContributionSingleFile tracking is not supported.</returns>
        double Progress();

        /// <summary>
        /// get the current Task status
        /// </summary>
        /// <returns>one of ITaskStatus status indications</returns>
        ITaskStatus Status();

        /// <summary>
        /// an optional user-readable message explaining Status(), such as error message if Status()==ITaskStatus.FAIL
        /// </summary>
        /// <returns></returns>
        string StatusMsg();

        /// <summary>
        /// Start this task and execute it until finished, error, or aborted.
        /// </summary>
        void Start();

        /// <summary>
        /// Abort this task (which may or may not be already running when this is called)
        /// </summary>
        void Abort();

        /// <summary>
        /// check whether Task is/has already been started or not
        /// </summary>
        /// <returns>true if running, completed, or failed, false if not yet started</returns>
        bool IsStarted();

        /// <summary>
        /// check whether Task is currently running
        /// </summary>
        /// <returns>true if status is ITaskStatus.RUNNING</returns>
        bool IsRunning();

        /// <summary>
        /// check whether Task is already finished or not
        /// </summary>
        /// <returns>true if finished (states ITaskStatus.SUCCESS or ITaskStatus.FAIL), false otherwise</returns>
        bool IsFinished();

        /// <summary>
        /// check whether Task has finished successfully.
        /// </summary>
        /// <returns>true if finished successfully, false if not (yet).</returns>
        bool IsSuccess();
    }
}
