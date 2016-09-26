using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// IUtilityThread wraps the native .Net Thread class, and can be used for
    /// any threading needs.
    /// <para>The primary functionality that IUtilityThread provides is the ability
    /// to predictably stop a thread - both synchronously and asynchronously wrt
    /// the calling thread. IUtilityThread uses a delegate to specify what code
    /// the thread should run.
    /// </para>
    /// <remarks>Note that IUtilityThread does not inherit from Thread. If you
    /// need to access its Thread, you may use its Thread property.</remarks>
    /// </summary>

    public interface IUtilityThread : IDisposable
    {
        /// <summary>
        /// Starts the thread.
        /// </summary>
       
        void Start();

        /// <summary>
        /// Signals that the thread should stop - synchronously wrt the caller. That is, this method
        /// does block until the thread has stopped.
        /// </summary>
        /// 
        void SyncStop();

        /// <summary>
        /// Signals that the thread should stop - asynchronously wrt the caller. That is, this method
        /// does not block until the thread has stopped.
        /// </summary>
        
        void AsyncStop();

        /// <summary>
        /// Waits until a thread has stopped. 
        /// <remarks>Intended to be used after an AsyncStop has been called.</remarks>
        /// </summary>
        
        void WaitForStopComplete();

        /// <summary>
        /// Indicates whether the current thread is this thread.
        /// </summary>

        bool IsCurrentThread();

        /// <summary>
        /// Will return after sleeping for the time <paramref name="msecs"/> or if the UtilityThread
        /// has been signalled to stop. 
        /// <remarks>Intended for timely shutdown of a thread that sleeps for periods of time.</remarks>
        /// </summary>
        /// <param name="msecs">The time, in milliseconds, to sleep.  </param>
        /// <returns>If stop was signalled, it returns true. If it slept for the full time, it returns false.</returns>

        bool SleepOnStopEvent(int msecs);

        /// <summary>
        /// Returns the the debug thread id (should match that in the debugger's thread window).
        /// </summary>

        string Name { get; set; }

        /// <summary>
        /// Description of the thread - mainly useful for debugging.
        /// </summary>

        string Description { get; set; }

        /// <summary>
        /// Used to specify the code that the thread will run.
        /// </summary>

        ThreadBody EntryPoint { get; set; }

        /// <summary>
        /// The state (Pristine, Starting, Running, Stopping, Dead) of the thread.
        /// </summary>

        UtilityThreadState State { get; }

        /// <summary>
        /// Returns the managed thread id (will not match that in the debugger's thread window).
        /// </summary>
        
        int ManagedThreadId { get; }

        /// <summary>
        /// Returns the the debug thread id (should match that in the debugger's thread window).
        /// </summary>

        int DebugThreadId { get; }

        /// <summary>
        /// Returns the underlying .NET ManualResetEvent that indicates whether the thread
        /// has initiated the process of stopping. You may Wait() on this handle, but should
        /// never alter it's state.
        /// </summary>
        
        WaitHandle ThreadStopHandle { get; }

        /// <summary>
        /// Returns the underlying .Net Thread object.
        /// <remarks>USE WITH CAUTION</remarks>
        /// </summary>

        Thread Thread { get; }
    }

    /// <summary>
    /// UtilityThreadState represents the possible states of a UtilityThread.
    /// </summary>

    public enum UtilityThreadState
    {
        /// <summary>
        /// The UtilityThread has been created but not started.
        /// </summary>
        Pristine,
        
        /// <summary>
        /// The UtilityThread has been started but has not yet begun running. This
        /// state should be short-lived and requires no client action to leave it.
        /// </summary>
        Starting,
        
        /// <summary>
        /// The UtilityThread is running. This is the normal state for a started thread.
        /// </summary>
        Running,
        
        /// <summary>
        /// The UtilityThread has been signalled to stop, but has not yet stopped.
        /// </summary>
        Stopping,
        
        /// <summary>
        /// The UtilityThread has stopped and terminated (it is shutdown).
        /// </summary>
        Dead
    } ;

    /// <summary>
    /// ThreadBody is the delegate that objects that wish to use a UtilityThread
    /// must provide.
    /// <para>A copy of the actual UtilityThread is provided to the delegate
    /// so that it may access various pieces of thread specific information.
    /// </para>
    /// </summary>

    public delegate void ThreadBody(IUtilityThread utilityThread);
}
