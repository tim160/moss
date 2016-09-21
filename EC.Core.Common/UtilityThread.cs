using System;
using System.Reflection;
using System.Threading;
using Castle.Core;
using Castle.Windsor;
using Castle.Core.Logging;
using EC.Common.Interfaces;
using MarineLMS.Core.Base;

namespace EC.Core.Common
{
    /// <summary>
    /// Implementation of IUtilityThread
    /// </summary>
    /// <remarks>
    /// ThreadID's aren't as simple as one might think. The managed thread id (from Thread.CurrentThread.ManagedThreadId) is stable insofar
    /// it works with fibers, but is not the id that appears in the threads window in the debugger. The thread window id is obtained from
    /// AppDomain.GetCurrentThreadId(), but that method is now obsolete (see MSDN under AppDomain.GetCurrentThreadId() for a discussion).
    /// Thus we provide both, the first being mManagedThreadID, the second being mDebugThreadID.
    /// </remarks>
 
    [TransientType]
    public sealed class UtilityThread : IUtilityThread
    {  
        /// <summary>
        /// Execute the thread body bound to this utility thread.
        /// </summary>
        
        public void Start()
        {
            lock (this)
            {
                DBC.NonNull(Name, "Name is null");
                DBC.NonNull(Description, "Description is null");
                DBC.NonNull(EntryPoint, "ThreadBody is null");
                DBC.Assert(State == UtilityThreadState.Pristine, String.Format("Start()[{0}] called multiple times", Name));
                Thread = new Thread(InternalRun) {Name = Name, Priority = ThreadPriority};
                State = UtilityThreadState.Starting;
                bool res = stopEvent.Reset(); // not stopped
                DBC.Assert(res, string.Format("Thread {0} mStopEvent.Reset() returned false", Name));
                res = deadEvent.Reset(); // not dead
                DBC.Assert(res, string.Format("Thread {0} mDeadEvent.Reset() returned false", Name));
                Thread.Start(); // start the actual thread
            }
        }

        /// <summary>
        /// Signals that the thread should stop - asynchronously wrt the caller. That is, this method
        /// does not block until the thread has stopped.
        /// </summary>
        
        public void AsyncStop()
        {
            lock (this)
            {
                log.InfoFormat("UtilityThread({0})[Id={1}] is being stopped", Name, DebugThreadId);
                bool res;

                // If we are Pristine, become stopped and dead, and return

                if (State == UtilityThreadState.Pristine)
                {
                    log.WarnFormat("UtilityThread::AsyncStop(): called on thread {0} which is Pristine (not started)", Name);
                    State = UtilityThreadState.Dead;
                    res = stopEvent.Set();
                    DBC.Assert(res, string.Format("Thread {0} mStopEvent.Set() returned false", Name));
                    res = deadEvent.Set();
                    DBC.Assert(res, string.Format("Thread {0} mDeadEvent.Reset() returned false", Name));
                    return;
                }

                // if we're dead or stopped

                if (State != UtilityThreadState.Starting && State != UtilityThreadState.Running)
                {
                    log.WarnFormat("UtilityThread::AsyncStop(): called on thread {0} which is already stopped (or dead)", Name);
                    return;
                }

                State = UtilityThreadState.Stopping;
                res = stopEvent.Set();
                DBC.Assert(res, string.Format("Thread {0} mStopEvent.Set() returned false", Name));
            }
        }

        /// <summary>
        /// Waits until a thread has stopped. 
        /// <remarks>
        /// Intended to be used after an AsyncStop has been called.
        /// </remarks>
        /// </summary>
        
        public void WaitForStopComplete()
        {
            while (true)
            {
                if (State == UtilityThreadState.Dead) { break; }
                if (deadEvent.WaitOne(WAIT_TIME, false)) { break; }
                log.WarnFormat("UtilityThread::WaitForStopComplete() - Thread {0} is taking a long time to stop, its state is {1}", Name, State);
            }

            DBC.Assert(State == UtilityThreadState.Dead, "Thread state not dead after dead event received");
            log.InfoFormat("UtilityThread::WaitForStopComplete() - Thread {0} is now in the dead state", Name);
        }

        /// <summary>
        /// Signals that the thread should stop - synchronously wrt the caller. That is, this method
        /// does block until the thread has stopped.
        /// </summary>

        public void SyncStop()
        {
            AsyncStop();
            WaitForStopComplete();
        }

        /// <summary>
        /// Indicates whether the current thread is this thread.
        /// </summary>
        
        public bool IsCurrentThread()
        {
            return Thread.CurrentThread.ManagedThreadId == this.ManagedThreadId;
        }

        /// <summary>
        /// Will return after sleeping for the time <paramref name="msecs"/> or if the UtilityThread
        /// has been signalled to stop. The return value indicates whether the thread has been
        /// stopped. That is, if the call returns true, then the caller should return from the 
        /// the thread entry point. Failure to do so will cause the SyncStop() method to block.
        /// A return value of false indicates that the timeout occurred.
        /// <remarks>
        /// Intended for timely shutdown of a thread that sleeps for periods of time.
        /// </remarks>
        /// </summary>
        /// <returns>true if the caller should return from the thread's main body</returns>

        public bool SleepOnStopEvent(int msecs)
        {
            DBC.Assert(msecs >= 0, "msecs < 0");
            return stopEvent.WaitOne(msecs, false);
        }

        /// <summary>
        /// IoC creation entry point.
        /// </summary>
        
        public UtilityThread(ILogger l)
        {
            log = l;
            DebugThreadId = -1;
            ManagedThreadId = -1;
            State = UtilityThreadState.Pristine;
        }

        /// <summary>
        /// Finalizer - Verify that the thread has been properly shutdown. There is not much we
        /// can do if it hasn't been, so we just Assert.
        /// </summary>
        
        ~UtilityThread()
        {
            DBC.Assert(State == UtilityThreadState.Pristine || State == UtilityThreadState.Dead,
                    String.Format("Destructor: Thread {0} is finalizing and it is not Pristine or Dead. Its state is {1}",
                    Name, State));
        }

        /// <summary>
        /// Release resources. Note that the underlying Thread class is not disposable, and we're
        /// assuming that the thread has already completed, so there is nothing to do to the
        /// underlying thread itself. But we do need to Dispose() the Windows Event objects that
        /// we're using.
        /// </summary>
        /// <remarks>
        /// If the utility thread's state is Pristine, then the Thread member variable will be null.
        /// </remarks>

        public void Dispose()
        {
            DBC.Assert(State == UtilityThreadState.Pristine || State == UtilityThreadState.Dead,
                    String.Format("Dispose: Thread {0} is finalizing and it is not Pristine or Dead. Its state is {1}",
                    Name, State));

            if (Thread != null) { log.InfoFormat("UtilityThread::Dispose - Thread[{0}] - State is {1}", Name, Thread.ThreadState.ToString()); }
            stopEvent.Dispose();
            deadEvent.Dispose();
            Thread = null;
        }

        // ----------------------------------- Public properties ----------------------------------

        public Thread Thread { get; private set; }
        public int ManagedThreadId { get; private set; }
        public int DebugThreadId { get; private set; }
        public UtilityThreadState State { get; private set; }
        public ThreadPriority ThreadPriority { get; private set; }
        public WaitHandle ThreadStopHandle { get { return stopEvent; } }

        // State set at creation time

        public string Name { get; set; }
        public string Description { get; set; }
        public ThreadBody EntryPoint { get; set; }

        // --------------------------------- Private Methods ---------------------------------------

        private void InternalRun()
        {
            try
            {
                bool shouldRun = false;

                lock (this)
                {
                    if (State == UtilityThreadState.Starting)
                    {
                        State = UtilityThreadState.Running;
                        shouldRun = true;
                        ManagedThreadId = Thread.CurrentThread.ManagedThreadId;

                        // AppDomain.GetCurrentThreadId(); is obsolete so it generates a warning. However, we
                        // want its value since that id matches the thread id in the debugger's threads window.

                        #pragma warning disable 618
                        DebugThreadId = AppDomain.GetCurrentThreadId();
                        #pragma warning restore 618
                    }
                }

                if (shouldRun)
                {
                    EntryPoint(this);
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("UtilityThread::internalRun() Caught exception {0}", e.Message);
            }
            finally
            {
                State = UtilityThreadState.Dead;
                var res = deadEvent.Set();
                DBC.Assert(res, "UtilityThread::internalRun() mDeadEvent.Reset() returned false");
            }
        }

        // -------------------------------- Injected instance data --------------------------------

        private ILogger log = null;

        // -----------------------------  Local Instance Data -------------------------------------

        private const int WAIT_TIME = 1000;
        private readonly ManualResetEvent deadEvent = new ManualResetEvent(true);    // indicates that the thread has stopped and is dead
        private readonly ManualResetEvent stopEvent = new ManualResetEvent(true);    // indicates that the thread has begun stopping
    }
}