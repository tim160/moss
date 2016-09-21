using System;
using System.Reflection;
using System.Threading;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;
using EC.Errors;

namespace EC.Core.Common
{
    /// <summary>
    /// The Lock class is a wrapper on top of the Monitor class
    /// (the Enter and Exit methods).
    /// <para>
    /// We would like to keep more info (such as thread ID, thread name
    /// and acquisition time) to help debugging. Therefore the Lock class
    /// should be used instead of any raw locking calls provided by the
    /// Monitor class.
    /// </para>
    /// </summary>
   
    [TransientType]
    public sealed class Lock : ILock
    {
        /// <summary>
        /// Request to acquire the lock. The current thread will be blocked if the
        /// lock is being held by another thread.
        /// </summary>
        /// <remarks>
        /// This is a reentrant lock.
        /// </remarks>
        
        public void Acquire()
        {
            long startTime;

            PreciseTickCount.GetPreciseTickCount(out startTime);

            lock (this)
            {
                if (unheldLocks != null)
                {
                    CheckUnheldLocks();
                }
                numWaiting++;
            }

            // this will block forever if mObjLock is being held by
            // another thread
            
            Monitor.Enter(objLock);

            lock (this)
            {
                Thread curThread = Thread.CurrentThread;
                holdingThreadId = curThread.ManagedThreadId;
                holdingThreadName = curThread.Name;

                numWaiting--;
                holdingCount++;
                state = LockState.Locked;

                // debugging info
                numAcquires++;

                // Ideally we should get the stopTime right before we exit
                // this method, but then it needs to get the lock again in
                // order to update the mTotalAcquireTicks.
                // So I rather put it here to avoid getting the lock again
                long stopTime;
                PreciseTickCount.GetPreciseTickCount(out stopTime);

                totalAcquireTicks += (stopTime - startTime);
            }
        }

        /// <summary>
        /// Release the lock.
        /// </summary>
        /// <remarks>
        /// Since this is a reentrant lock, calling Release may not release
        /// the underlying lock immediately. If the calling thread has made an
        /// equal number of Acquire and Release calls, then the lock is released.
        /// </remarks>
        /// <seealso cref="Acquire()"/>
        
        public void Release()
        {
            lock (this)
            {
                DBC.Assert(state == LockState.Locked && holdingCount > 0 && Thread.CurrentThread.GetHashCode() == holdingThreadId, "Lock::Release - Inconsistent state");
                holdingCount--;

                if (holdingCount == 0)
                {
                    state = LockState.Unlocked;
                    holdingThreadId = 0;
                    holdingThreadName = "";
                }

                // everything seems OK, release the lock
                Monitor.Exit(objLock);
            }
        }

        /// <summary>
        /// Reset all the internal debugging counters.
        /// </summary>
        
        public void ResetDebugCounters()
        {
            lock (this)
            {
                numAcquires = 0;
                totalAcquireTicks = 0;
            }
        }

        /// <summary>
        /// Setting the list of unheld Locks. These are the Locks that should not be in the
        /// Locked state when the current Lock is being acquired.
        /// </summary>
        /// <param name="locks">Array of Locks</param>
        
        public void SetUnheldLocks(ILock[] locks)
        {
            DBC.Assert(unheldLocks == null, "SetUnheldLocks can only be called once on the same Lock instance");

            lock (this)
            {
                unheldLocks = new Lock[locks.Length];
                Array.Copy(locks, unheldLocks, locks.Length);
            }
        }

        /// <summary>The name of the holding thread.</summary>
        /// <value>Return the mHoldingThreadName data member.</value>
        
        public string HoldingThreadName
        {
            get { return holdingThreadName; }
        }

        /// <summary>The ID of the holding thread.</summary>
        /// <value>Return the mHoldingThreadID data member.</value>
        
        public int HoldingThreadID
        {
            get { return holdingThreadId; }
        }

        /// <summary>The current state of the lock.</summary>
        /// <value>Return the state data member.</value>
        
        public LockState CurrentState
        {
            get { return state; }
        }

        /// <summary>The number of successful acquires.</summary>
        /// <value>Return the numAcquires data member.</value>
        
        public long NumAcquires
        {
            get { return numAcquires; }
        }

        /// <summary>
        /// The cumulative total number of ticks (from the high precision counter)
        /// for all the acquires so far.
        /// </summary>
        /// <value>Return the totalAcquireTicks data member.</value>
        
        public long TotalAcquireTicks
        {
            get { return totalAcquireTicks; }
        }

        /// <summary>
        /// Constructor. It creates the underlying lock object and initializes
        /// all the instance variables.
        /// </summary>
      
        public Lock()
        {
            // initialize instance data

            objLock = new Object();
            holdingThreadId = 0;
            numWaiting = 0;
            holdingCount = 0;
            state = LockState.Unlocked;

            // initialize debugging variables

            numAcquires = 0;
            totalAcquireTicks = 0;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        /// <remarks>
        /// Strictly speaking we don't need the destructor (finalizer) here.
        /// It's defined just to help catching user errors in using the Lock class
        /// (e.g. forgot to release the lock)
        /// </remarks>
        
        ~Lock()
        {
            lock (this)
            {
                if (state != LockState.Unlocked)
                {
                    // log warning msg
                    // for now just using FlushAll, later on we will get the Logger
                    // instance statically or via some well known name...etc

                    String msg = String.Format("Thread '{0}' (id={1}) is still holding a lock during finalization", holdingThreadName, holdingThreadId);
                    WindowsEventLog.AddEvent(msg, WindowsEventLog.LockingError);

                    // no need to release the lock here because the current
                    // the finalize method is invoked from the finalizer thread.
                    // In fact, we can't release the lock because the finalizer thread
                    // doesn't own the object lock (exception will be thrown)
                }
            }
        }

        /// <summary>
        /// Assert if any of the Locks in the mUnheldLocks array is in the Locked state.
        /// </summary>
        
        private void CheckUnheldLocks()
        {
            foreach (Lock unheldLock in unheldLocks)
            {
                if (unheldLock.CurrentState == LockState.Unlocked)
                    throw new OrderException();
            }
        }

        // -------------------------------------- Instance Data -----------------------------------

        private readonly object objLock;     // the actual underlying "lock"
        private uint holdingCount;           // how many times the current thread
        private int holdingThreadId;         // the .NET logical thread id
        private string holdingThreadName;    // the holding thread name
        private long numAcquires;            // the number of successful acquires
        private uint numWaiting;             // number of threads waiting
        private LockState state;             // State of the lock
        private long totalAcquireTicks;      // the cumulative total number of ticks

        // to support "order lock" checking

        private Lock[] unheldLocks;          // the locks that shouldn't be held when
    } ;
}