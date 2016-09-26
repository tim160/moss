//This is a modified version of AsyncAppender that shipped with Log4net
//Location: Log4net\examples\net\1.0\Appenders\SampleAppendersApp\cs\src\Appender\AsyncAppender.cs
//The Sample AsyncAppender above doesn't keep the message ordering.  
//I modified it to keep the ordering

#region Copyright & License

//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using log4net.Appender;
using log4net.Core;
using log4net.Util;
using Castle.Core.Logging;
using EC.Common.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Appender that processes LoggingEvents asynchronously
    /// </summary>
    /// <remarks>
    /// This appender forwards LoggingEvents to a list of attached appenders.
    /// The events are forwarded asynchronously using the WorkQueue.
    /// This allows the calling thread to be released quickly and it guarantees
    /// the ordering of events delivered to the attached appenders.
    /// </remarks>
    /// <remarks>
    /// <para>
    /// The implementation makes use of a couple of utility classes that are
    /// normally created via Castle/Windsor. However, since the appender is
    /// allocated directly by log4net, we don't have access to dependency
    /// injection here, and we need to satisfy the dependencies of the
    /// utility classes ourselves. These dependencies are fairly simple: the IDBC singleton,
    /// and an ILogger instance.
    /// </para>
    /// <para>
    /// Since the DBC class is essentially stateless, we can simply allocate the object
    /// directly and pass it in as though we were castle. The logging implemention is
    /// much more complex, and since we're actually inside of it here, we just
    /// pass NullLogger.Instance. 
    /// </para>
    /// </remarks>
   
    public sealed class AsyncAppender :  IOptionHandler, IAppenderAttachable
    {
        public void Close()
        {
            // Remove all the attached appenders
            using (new ScopedLock(stateLock))
            {
                //shut down and empty out the work queue
                workQueue.Flush();
                workQueue.SyncStop();

                if (appenderAttachedImpl != null)
                {
                    appenderAttachedImpl.RemoveAllAppenders();
                }
            }
        }

        /// <summary>
        /// Add an item for the children appenders to process
        /// </summary>
        /// <param name="loggingEvent">item to add</param>

        public void DoAppend(LoggingEvent loggingEvent)
        {
            using (new ScopedLock(stateLock))
            {
                loggingEvent.Fix = Log4NetFlags;
                workQueue.EnqueueWorkItem(new LoggingEventQueueItem(loggingEvent, ProcessWorkItem));
            }
        }

        /// <summary>
        /// Add an array of items for the children appenders to process
        /// </summary>
        /// <param name="loggingEvents">items to add</param>

        public void DoAppend(LoggingEvent[] loggingEvents)
        {
            using (new ScopedLock(stateLock))
            {
                foreach (LoggingEvent loggingEvent in loggingEvents)
                {
                    loggingEvent.Fix = Log4NetFlags;
                    workQueue.EnqueueWorkItem(new LoggingEventQueueItem(loggingEvent, ProcessWorkItem));
                }
            }
        }

        public void AddAppender(IAppender newAppender)
        {
            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }

            using (new ScopedLock(stateLock))
            {
                if (appenderAttachedImpl == null)
                {
                    appenderAttachedImpl = new AppenderAttachedImpl();
                }

                appenderAttachedImpl.AddAppender(newAppender);
            }
        }

        public AppenderCollection Appenders
        {
            get
            {
                using (new ScopedLock(stateLock))
                {
                    return appenderAttachedImpl != null
                                ? appenderAttachedImpl.Appenders
                                : AppenderCollection.EmptyCollection;
                }
            }
        }

        public IAppender GetAppender(string name)
        {
            using (new ScopedLock(stateLock))
            {
                if (appenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return appenderAttachedImpl.GetAppender(name);
            }
        }

        public void RemoveAllAppenders()
        {
            using (new ScopedLock(stateLock))
            {
                if (appenderAttachedImpl != null)
                {
                    appenderAttachedImpl.RemoveAllAppenders();
                    appenderAttachedImpl = null;
                }
            }
        }

        public IAppender RemoveAppender(IAppender appender)
        {
            using (new ScopedLock(stateLock))
            {
                if (appender != null && appenderAttachedImpl != null)
                {
                    return appenderAttachedImpl.RemoveAppender(appender);
                }
            }
            return null;
        }

        public IAppender RemoveAppender(string name)
        {
            using (new ScopedLock(stateLock))
            {
                if (name != null && appenderAttachedImpl != null)
                {
                    return appenderAttachedImpl.RemoveAppender(name);
                }
            }
            return null;
        }

        /// <summary>
        /// Flush the internal queue items to the appenders and close the WorkQueue.  Remove the appenders.
        /// </summary>
        
        public void ActivateOptions()
        {
        }

        /// <summary>
        /// Delegate method invoked by the work queue when the work queue decides to
        /// process an item.
        /// </summary>
        /// <param name="item">An item in the work queue.</param>
      
        private void ProcessWorkItem(LoggingEventQueueItem item)
        {
            if (appenderAttachedImpl != null)
            {
                DBC.Assert(item != null, "Item is null");
                appenderAttachedImpl.AppendLoopOnAppenders(item.Item);
            }
        }

        /// <summary>
        /// Constructor
        /// Note: this would start a new thread. 
        /// </summary>

        public AsyncAppender()
        {
            try
            {
                Log4NetFlags = FixFlags.ThreadName | FixFlags.Exception; //FixFlags.All is very expensive
                stateLock = new Lock();
                thread = new UtilityThread(NullLogger.Instance);
                workQueue = new WorkQueue<LoggingEventQueueItem>(NullLogger.Instance, thread);
                workQueue.Name = "AsyncFileAppender queue";
                workQueue.Description = "AsyncFileAppender queue of items to be logged";
                workQueue.Batching = false;
                workQueue.AggregationDelayMs = 0;
                workQueue.MaxSize = CAPACITY;
                workQueue.Start();
            }
            catch (Exception ex)
            {
                String msg = String.Format("AsyncAppender: constructor exception - {0}", ex.Message);
                WindowsEventLog.AddEvent("AsyncAppender constructor error", WindowsEventLog.ServiceError);
            }
        }

        // --------------------------------- public state ----------------------------------------

        public FixFlags Log4NetFlags { get; set; }
        public string Name { get; set; }

        // --------------------------------- private state ---------------------------------------

        private const int CAPACITY = 500;
        private readonly Lock stateLock = null;
        private readonly WorkQueue<LoggingEventQueueItem> workQueue = null;
        private AppenderAttachedImpl appenderAttachedImpl = null;
        private IUtilityThread thread = null;

        /// <summary>
        /// Needed this class to support the WorkQueue
        /// </summary>
        
        private class LoggingEventQueueItem : IQueueItem
        {
            private readonly LoggingEvent _item;
            private readonly ProcessWorkItem<LoggingEventQueueItem> _processor;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="item">Item for processing</param>
            /// <param name="processor">Processing function to be invoked when this Queue item is processed</param>
          
            public LoggingEventQueueItem(LoggingEvent item, ProcessWorkItem<LoggingEventQueueItem> processor)
            {
                _item = item;
                _processor = processor;
            }

            public LoggingEvent Item
            {
                get { return _item; }
            }

            /// <summary>
            /// Process the LoggingEvent using the processor delegate
            /// </summary>
          
            public void Process()
            {
                _processor(this);
            }
        }
   }
}