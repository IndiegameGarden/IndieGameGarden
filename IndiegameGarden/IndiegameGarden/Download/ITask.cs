// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

namespace IndiegameGarden.Download
{
    /**
     * <summary>
     * a task that takes time such as loading, downloading, installing or processing
     * </summary>
     */
    public interface ITask
    {
        /// <summary>
        /// Get task progress indication value
        /// </summary>
        /// <returns>progress indication between 0...1</returns>
        double Progress();

        /// <summary>
        /// Start this task
        /// </summary>
        void Start();

        /// <summary>
        /// Abort this task (which may or may not be already running when this is called)
        /// </summary>
        void Abort();

        /// <summary>
        /// check whether Task is started or not
        /// </summary>
        /// <returns>true if started</returns>
        bool IsStarted();
    }
}
