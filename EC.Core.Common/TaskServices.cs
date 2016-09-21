using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.Core.Logging;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Interface for services that apply to groups of tasks.
    /// </summary>

    [SingletonType]

    public class MLSTaskService : IMLSTaskServices
    {
        /// <summary>
        /// Wait for all tasks to complete. This is a wrapper around Task.WaitAll(). If
        /// any of the tasks throw an exception, WaitAll() throws an AggregateException.
        /// </summary>
        /// <param name="tasks">array of tasks to wait for</param>

        public void WaitAll(IMLSTask[] tasks)
        {
            Task.WaitAll(tasks.Select(t => t.GetUnderlyingTask()).ToArray());
        }

        /// <summary>
        /// Wait for all the given tasks to complete, and then extract any exceptions from the
        /// tasks. The first exception is re-thrown; others are just logged.
        /// </summary>
        /// <param name="tasks">array of IMLSTasks to wait for</param>

        public void WaitAllAndThrowException(IMLSTask[] tasks)
        {
            Exception result = null;

            try
            {
                WaitAll(tasks);
            }
            catch (AggregateException)
            {
                foreach (var task in tasks)
                {
                    try
                    {
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null) { result = LogAndSetException(result, ex.InnerException); }
                        else { result = LogAndSetException(result, ex); }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("WaitAndGetException: Unexpected exception from WaitAll()", ex);
                result = ex;
            }

            if (result != null) { throw result; }
        }

        /// <summary>
        /// Check to see if there already is an exception. If so, log the new exception (since we're
        /// basically going to throw it away). If not, then return the exception.
        /// </summary>
        /// <param name="current">current exception</param>
        /// <param name="newException">new exception</param>
        /// <returns>the new current exception</returns>
        
        private Exception LogAndSetException(Exception current, Exception newException)
        {
            DBC.NonNull(newException, "LogAndSetException - new exception cannot be null");

            if (current == null)
            {
                current = newException;
            }
            else
            {
                Logger.Error("WaitAndThrowException: Multiple exceptions (logging discarded exception)", newException);
            }

            return current;
        }

        /// <summary>
        /// IoC constructor.
        /// </summary>

        public MLSTaskService(IKernel k, ILogger l)
        {
            Kernel = k;
            Logger = l;
        }

        // ---------------------------- Injected Components ---------------------------------------

        private IKernel Kernel = null;
        private ILogger Logger = null;
    }
}
