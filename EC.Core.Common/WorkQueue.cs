using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Castle.Core;
using Castle.Windsor;
using Castle.Core.Logging;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Implementation of IWorkQueue
    /// </summary>
    /// <typeparam name="T">Type of objects on the queue</typeparam>

    // The implementation needs to be thread-safe, so there are various places that
    // manipulate locks, mutexes, etc.

    public sealed class WorkQueue<T> : IWorkQueue<T> where T : IQueueItem
    {
    #region Public Methods

        public void Start()
        {
            DBC.NonNull(Name, "Name is null");
            DBC.NonNull(Description, "Description is null");
            DBC.Assert(AggregationDelayMs >= 0, "AggregationDelayMs < 0");
            thread.Name = Name;
            thread.Description = Description;
            thread.EntryPoint = Run;
            thread.Start();
        }

        public void AsyncStop()
        {
            thread.AsyncStop();
        }

        public void SyncStop()
        {
            thread.SyncStop();
        }

        public void WaitForStopComplete()
        {
            thread.WaitForStopComplete();
        }

        public bool EnqueueWorkItem(T item)
        {
            DBC.NonNull(item, "Item is null");

            using (new ScopedLock(stateLock))
            {
                if (MaxSize > 0 && workItemQueue.Count >= MaxSize)
                {
                    return false;
                }

                bool emptyBefore = (workItemQueue.Count == 0);
                workItemQueue.Add(item);

                if (emptyBefore)
                {
                    notEmptyEvent.Set();
                }
            }

            return true;
        }
      
        public void ClearWorkQueue()
        {
            using (new ScopedLock(stateLock))
            {
                workItemQueue.Clear();
                notEmptyEvent.Reset();
            }
        }

        // We need to make a cope of the items because we're going to modify
        // the original list as we work our way through.

        public void FilterWorkQueue(Predicate<T> filter)
        {
            DBC.NonNull(filter, "Filter is null");

            if (filter == null || Count == 0) return;

            using (new ScopedLock(stateLock))
            {
                IList<T> copy = new List<T>(workItemQueue);

                foreach (T t in copy)
                {
                    if (filter(t))
                        workItemQueue.Remove(t);
                }

                if (workItemQueue.Count == 0)
                {
                    notEmptyEvent.Reset();
                }
            }
        }

        public void Flush()
        {
            IList<T> workQueue;

            using (new ScopedLock(stateLock))
            {
                workQueue = StealWorkQueue();
            }

            foreach (T item in workQueue)
            {
                try
                {
                    item.Process();
                }
                catch (Exception e)
                {
                    log.ErrorFormat("WorkQueue::Flush(): Thread {0} Caught Exception from delegate: {1}", thread.Name, e.Message);
                }
            }
        }

        public void TrimFront(int numItems)
        {
            DBC.Assert(numItems > 0, String.Format("Thread {0} numItems is <= 0", thread.Name));

            using (new ScopedLock(stateLock))
            {
                if (workItemQueue.Count <= numItems)
                {
                    ClearWorkQueue(); // resets the event
                }
                else
                {
                    workItemQueue.RemoveRange(0, numItems);
                }
            }
        }

        public void TrimBack(int numItems)
        {
            DBC.Assert(numItems > 0, String.Format("Thread {0} numItems is <= 0", thread.Name));

            using (new ScopedLock(stateLock))
            {
                if (workItemQueue.Count <= numItems)
                {
                    ClearWorkQueue(); // reset's the event
                }
                else
                {
                    workItemQueue.RemoveRange(workItemQueue.Count - numItems, numItems);
                }
            }
        }

        public int Count 
        { 
            get 
            {
                using (new ScopedLock(stateLock))
                { 
                    return workItemQueue.Count; 
                } 
            } 
        }

    #endregion

    #region Public Properties

        // auto implemented properties

        public int AggregationDelayMs { get; set; }
        public bool Batching { get; set; }
        public int MaxSize { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // computed properties

        public int ManagedThreadId { get { return thread.ManagedThreadId; } }
        public int DebugThreadId { get { return thread.DebugThreadId; } }

    #endregion

    #region Constructors/Destructors

        public WorkQueue(ILogger l, IUtilityThread t)
        {
            log = l;
            thread = t;
            MaxSize = -1;
        }

    #endregion

    #region Private Methods

        private IList<T> StealWorkQueue()
        {
            using (new ScopedLock(stateLock))
            {
                IList<T> workQueue = new List<T>(workItemQueue);
                workItemQueue.Clear();
                notEmptyEvent.Reset();
                return workQueue;
            }
        }

        private void Run(IUtilityThread thread)
        {
            bool lastPass = false;

            while (!lastPass)
            {
                WaitHandle[] handles = {notEmptyEvent, thread.ThreadStopHandle};

                int res = WaitHandle.WaitAny(handles); // TODO: we wait forever - maybe not a great idea?
                if (res == 1 || thread.State != UtilityThreadState.Running) // should we stop?
                {
                    lastPass = true;
                }

                if (!lastPass && AggregationDelayMs > 0)
                {
                    lastPass = thread.ThreadStopHandle.WaitOne(AggregationDelayMs, false);
                }

                IList<T> workQueue;
                using (new ScopedLock(stateLock))
                {
                    if (Batching)
                    {
                        workQueue = StealWorkQueue();
                    }
                    else
                    {
                        workQueue = new List<T>();
                        if (workItemQueue.Count > 0)
                        {
                            workQueue.Add(workItemQueue[0]);
                            workItemQueue.RemoveAt(0);
                        }
                        if (workItemQueue.Count == 0)
                        {
                            notEmptyEvent.Reset();
                        }
                    }
                }

                foreach (T item in workQueue)
                {
                    try
                    {
                        item.Process();
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("WorkQueue::run(): Thread {0} Caught Exception from delegate: {1}", thread.Name, e.Message);
                    }
                }
            }
        }

    #endregion

    #region Instance Data

        // dependencies

        private ILogger log = null;
        private IUtilityThread thread = null;

        // private state

        private Lock stateLock = new Lock();
        private readonly ManualResetEvent notEmptyEvent = new ManualResetEvent(false);
        private readonly List<T> workItemQueue = new List<T>();

    #endregion
   }
}