using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.Core.Logging;
using Castle.MicroKernel;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;
using EC.Constants;

namespace EC.Core.Common
{
    /// <summary>
    /// Implementation of IStatsLogManager. The StatsLogmanager runs the background thread
    /// that is responsible for periodically calling flush on all stats collectors to cause
    /// them to output their data to the log.
    /// </summary>
    
    [SingletonType]
    [RegisterAsType(typeof(IStatsLogManager), typeof(IHaveBackgroundThread), "StatsLogManager")]

    public class StatsLogManager : IStatsLogManager, IHaveBackgroundThread, IDisposable
    {
        /// <summary>
        /// Add a stats collector to the set managed by this object.
        /// </summary>
        /// <param name="s">the collector to be managed</param>
        
        public ILogger Add(IStatsCollector s)
        {
            DBC.NonNull(s, "Stats collector cannot be null");
   
            using (new ScopedLock(stateLock))
            {
                collectors.Add(s);
            }

            return statsLog;
        }

        /// <summary>
        /// Remove a collector from the manager.
        /// </summary>
        /// <param name="s">collector to remove</param>
        
        public void Remove(IStatsCollector s)
        {
            DBC.NonNull(s, "Stats collector cannot be null");

            using (new ScopedLock(stateLock))
            {
                collectors.Remove(s);
            }
        }

        /// <summary>
        /// Start the manager. 
        /// </summary>

        public void StartBackgroundThread()
        {
            logger.InfoFormat("StatsLog Manager: Start called");
            flusherThread.Start();
        }

        /// <summary>
        /// Return the background thread for the timer.
        /// </summary>

        public IUtilityThread GetBackgroundThread()
        {
            return this.flusherThread;
        }

        /// <summary>
        /// Called automatically by container when container is disposed.
        /// </summary>
        /// <remarks>
        /// Important: The SyncStop() must be outside the stateLock because
        /// the main thread body also uses the stateLock. Having the SyncStop()
        /// inside the lock can cause a deadlock.
        /// </remarks>

        public void StopBackgroundThread()
        {
            logger.Info("StatsLog Manager: Stop called");

            if (flusherThread != null) flusherThread.SyncStop();
            logger.Info("StatsLog Manager: Stopping - SyncStop processed.");
            
            using (new ScopedLock(stateLock))
            {
                logger.Info("StatsLog Manager: Stopping - 'stateLock' acquired'.");
                if (collectors != null) { collectors.ForEach(c => c.Flush()); }
                flusherThread = null;
                collectors = null;
            }

            logger.Info("StatsLog Manager: Stop complete");
        }
      
        /// <summary>
        /// The stats log manager uses a single background thread to handle flushing
        /// data from all stats log collectors. Every stats log collector
        /// needs to register itself with the manager.
        /// </summary>
        
        public StatsLogManager(IKernel k, IUtilityThread t, ILock l, ILogger log)
        {
            Kernel = k;
            stateLock = l;
            flusherThread = t;
            logger = log;
            statsLog = Kernel.Resolve<ILogger>(IoCSetup.StatsLogName);
            collectors = new List<IStatsCollector>();
            flusherThread.Name = "Stats Manager Thread";
            flusherThread.Description = "Handles periodic logging of stats data";
            flusherThread.EntryPoint = FlusherThreadRun;
            log.InfoFormat("StatsLog Manager Constructor complete");
        }

        /// <summary>
        /// Release disposable objects and kernel components which were directly resolved
        /// (Castle will call Dispose on any components injected via the constructor).
        /// </summary>
        
        public void Dispose()
        {
            if (flusherThread != null)
            {
                logger.WarnFormat("StatsLog Manager: Dispose called before Stop");
                return;
            }

            if (statsLog != null) Kernel.ReleaseComponent(statsLog);
        }

        // ------------------------------------ Private Methods -----------------------------------

        /// <summary>
        /// Main entry point for the internal flusher thread.
        /// </summary>
        
        private void FlusherThreadRun(IUtilityThread thread)
        {
            DBC.Assert(stateLock != null, "stateLock == null");
            DBC.Assert(collectors != null, "collectors == null");
            logger.InfoFormat("StatsLog Manager: Background thread running (ID = {0})", this.flusherThread.DebugThreadId);
            var contextMgr = this.Kernel.Resolve<IManageRequestContexts>();
            contextMgr.InitializeThreadRequestContext();
            var context = contextMgr.GetContext();
            context.CurrentUserId = UserIdConstants.SYSTEM_USER_ID;
            context.DisplayName = "Stat Log Manager Background Thread";
            int waitTimeout = 1000; // short initial wait period for first flush

            while (!thread.SleepOnStopEvent(waitTimeout))
            {
                try
                {
                    using (new ScopedLock(stateLock))
                    {
                        if (collectors.Count == 0) continue;

                        foreach (IStatsCollector sc in collectors)
                        {
                            if (sc.GetTimeToNextFlush() <= TIME_OUT_PRECISION_IN_MS)
                            {
                                sc.Flush();
                            }
                        }

                        // check how long should we wait for the next iteration

                        waitTimeout = MAX_FLUSH_PROBE_INTERVAL;

                        foreach (IStatsCollector sc in collectors)
                        {
                            long nextTime = sc.GetTimeToNextFlush();

                            if (nextTime < waitTimeout)
                            {
                                waitTimeout = (int) nextTime;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Unexpected exception {0}", e.Message);
                }
            }
        }
     
        // -------------------------------- Instance Data ------------------------------------------

        // constants

        private const int MAX_FLUSH_PROBE_INTERVAL = 30000; // max time between flush attempts
        private const int TIME_OUT_PRECISION_IN_MS = 10;    // the shortest time to wait

        // injected

        private IKernel Kernel = null;
        private IUtilityThread flusherThread = null;
        private ILogger logger = null;
        private ILock stateLock = null;

        // local

        private ILogger statsLog;
        private IList<IStatsCollector> collectors; 
    }
}