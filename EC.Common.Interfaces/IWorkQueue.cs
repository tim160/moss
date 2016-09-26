using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// The WorkQueue class is a generic class that provides an asynchronous 
    /// separation between emitting things you must do, and actually doing them.
    /// It is essentially a queue plus a thread. You may queue work items (units
    /// of work or things to do) onto the queue, and the thread will then pull
    /// those items off the queue and process them will a delegate you provide.
    /// <para>The WorkQueue can be used for many different situations where
    /// you do not want to block a thread by performing some work (or to use the
    /// terminology, processing a work item).</para>
    /// <remarks>For example, suppose you wish to log messages to disk. You do not
    /// want to make the thread that makes the write wait for a disk write to 
    /// complete. A WorkQueue is a good solution for this. Essentially you will 
    /// get the user's message and place it into a WorkQueue and immediately return
    /// control to the user. Then, the WorkQueue's thread will wake up and invoke
    /// your delegate that accepts a message, and in that delegate you will write it to
    /// disk. There are many similar programming situations that a WorkQueue may be
    /// used in. Note that this WorkQueue does not have a notion of priorities (of work
    /// items), but that would be easy to add (to either this class or another). Note 
    /// that if you clear a queue you may still have to wait aggregationDelay for
    /// the next Enqueue'd WorkItem to be processed. If there are work items in the
    /// WorkQueue when you stop it, it will process the work items, which may take
    /// some time depending on how many items there are and the time to process them.
    /// </remarks>
    /// </summary>
    
    public interface IWorkQueue<T> where T : IQueueItem
    {
        /// <summary>
        /// Adds a work item to the back of the WorkQueue.
        /// </summary>
        /// <param name="item">The item to be added (do not pass in null).</param>
        /// <returns>
        /// Returns false if the item is not Enqueue'd. This can happen if it's
        /// null or if the WorkQueue has a maxSize and Enqueue'ing the item would exceed this
        /// maxSize.
        /// </returns>

        bool EnqueueWorkItem(T item);

        /// <summary>
        /// Starts the WorkQueue (its thread).
        /// </summary>

        void Start();

        /// <summary>
        /// Signals that the WorkQueue (its thread) should stop - asynchronously wrt the caller. That
        /// is, this method does not block until the WorkQueue (thread) has stopped.
        /// </summary>

        void AsyncStop();

        /// <summary>
        /// Signals that the WorkQueue (its thread) should stop - synchronously wrt the caller. That
        /// is, this method does block until the WorkQueue (thread) has stopped.
        /// </summary>

        void SyncStop();

        /// <summary>
        /// Waits until a WorkQueue (its thread) has stopped. 
        /// <remarks>Intended to be used after an AsyncStop has been called.</remarks>
        /// </summary>

        void WaitForStopComplete();

        /// <summary>
        /// Clears all items from the WorkQueue. Note that these items will not be 
        /// processed.
        /// </summary>

        void ClearWorkQueue();

        /// <summary>
        /// Filters items in the WorkQueue based on the <paramref name="filter"/>. If an item
        /// in the WorkQueue is to be removed (filtered out) then the filter should return true.
        /// </summary>
        /// <param name="filter">
        /// A delegate that will be called on every work item in the
        /// WorkQueue. The delegate should not modify the work item, but should simply inspect
        /// it to see if it should be filtered out (removed) from the WorkQueue.
        /// </param>
        /// <remarks>
        /// Modifying the WorkQueue while filtering is not recommended => you will not
        /// see any new elements you Enqueue(). Also, the performance of this method may be questionable
        /// since it does a copy.
        /// </remarks>

        void FilterWorkQueue(Predicate<T> filter);

        /// <summary>
        /// Flushes all outstanding work items, synchronously with respect to the caller (uses the
        /// callers thread). It is generally intended for shutdown.
        /// </summary>
        /// <remarks>
        /// This will block the calling thread until all items are processed. This method will
        /// throw exceptions from the delegates, and any subsequent work items after the one that threw
        /// will no be processed, so avoid that. Note that if a new work item is added to the queue after
        /// while the Flush() call is in progress, the delegate can get called with work items interleaved
        /// from the Flush() call along with the new work item.
        /// </remarks>

        void Flush();

        /// <summary>
        /// Trims (removes) numItems from the front of the WorkQueue.
        /// </summary>
        /// <param name="numItems">The number of items to trim. Must be > 0.</param>

        void TrimFront(int numItems);

        /// <summary>
        /// Trims (removes) numItems from the front of the WorkQueue.
        /// </summary>
        /// <param name="numItems">The number of items to trim. Must be > 0.</param>

        void TrimBack(int numItems);

        /// <summary>
        /// This bool indicates whether the WorkQueue should batch process work items. This means
        /// that if 10 items are on the queue when the worker thread looks at it, it will process all 10 items. In contrast,
        /// if batching is turned off, it will process 1 item and then check if it (the WorkQueue thread) should stop, and
        /// sleep (if aggregationDelayMs > 0) before processing the next work item. Therefore, one should
        /// note that batching WorkQueue's with many items that can be slow to process will not shut down in as timely a fashion
        /// as a non-batching WorkQueue. However, a non-batching WorkQueue can leave items in its queue unprocessed if told to
        /// shutdown.
        /// </summary>

        bool Batching { get; set; }

        /// <summary>
        /// The amount of time, in milliseconds, that the thread will wait before
        /// processing one or more work items that are available. It can be used for WorkQueue's
        /// that perform batching in order to allow work items to accumulate, but can be
        /// used for non-batching WorkQueue's as well, to provide a minimum period between processing work items.
        /// </summary>

        int AggregationDelayMs { get; set; }

        /// <summary>
        /// The maximum size of the WorkQueue. Note that if this is set to a value >= 0 then Enqueue() will
        /// return false if Enqueue'ing item would overflow the WorkQueue.
        /// </summary>
        
        int MaxSize { get; set; }

        /// <summary>
        /// Returns the name of the WorkQueue (and underlying thread). Can only be set prior to calling
        /// Start().
        /// </summary>

        string Name { get; set; }

        /// <summary>
        /// Returns the description of the WorkQueue (and underlying thread). Can only be set prior to 
        /// calling Start().
        /// </summary>

        string Description { get; set; }

        /// <summary>
        /// Returns the number of work items in the WorkQueue
        /// </summary>

        int Count { get; }  

        /// <summary>
        /// Returns the managed thread id of the WorkQueue's worker thread (will not match that in the debugger's thread window).
        /// </summary>

        int ManagedThreadId { get; }

        /// <summary>
        /// Returns the managed thread id of the WorkQueue's worker thread (will not match that in the debugger's thread window).
        /// </summary>

        int DebugThreadId { get; }
    }

    /// <summary>
    /// Interface containing a Process method for WorkQueue or TimerQueue to process an item.
    /// </summary>

    public interface IQueueItem
    {
        void Process();
    }

    /// <summary>
    /// ProcessWorkItem is a generic delegate that processes a workItem 
    /// (something to work on) of type T. 
    /// <para>T in the delegate must match the T of the WorkQueue 
    /// it is associated with.</para>
    /// </summary>

    public delegate void ProcessWorkItem<T>(T workItem);
}
