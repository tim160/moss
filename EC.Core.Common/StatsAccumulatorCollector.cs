using System;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Core.Logging;
using EC.Common.Interfaces;
using EC.Common.Base;

namespace EC.Core.Common
{
   /// <summary>
   /// Implementation for an IStatsCollector that just keeps a running total 
   /// of its data points. It also keeps an accumulated value that is never
   /// reset.
   /// </summary>
   
    [TransientType]

    public sealed class StatsAccumulatorCollector : IStatsCollector
    {
        
        public void AddDataPoint(double value)
        {
            using (new ScopedLock(dataLock))
            {
                totalAccumulator += value;
                accumulator += value;
                hasData = true;
            }
        }
        
        public void Flush()
        {
            using (new ScopedLock(dataLock))
            {
                lastFlushTicks = DateTime.Now.Ticks;

                if (hasData)
                {
                    statsLog.InfoFormat("Stats: {0}: (acc since last flush, total): {1:F3},{2:F3} {3}", Name, accumulator, totalAccumulator, DataUnitName);
                }
                else if (LogWhenEmpty)
                {
                    statsLog.InfoFormat("Stats: {0}: No data", Name);
                }

                if (ClearDataAfterFlush) Reset();
            }
        }

        public void Reset()
        {
            using (new ScopedLock(dataLock))
            {
                accumulator = 0;
                hasData = false;
            }
        }
        
        // NOTE: No lock needed since we just read LastFlushTicks and FlushPeriod.

        public long GetTimeToNextFlush()
        {
            long nextTick = lastFlushTicks + (FlushPeriod * 10000); // multiply 10000 for converting to 100-ns
            long curTick = DateTime.Now.Ticks;

            // if we are late already, return 0 (i.e. should flush immediately)

            if (curTick >= nextTick)
            {
                return 0;
            }

            return (nextTick - curTick) / 10000; // divided by 10000 for converting from Ticks (100 ns) to ms
        }

        public StatsAccumulatorCollector(IKernel k, ILock l, IStatsLogManager m)
        {
            kernel = k;
            dataLock = l;
            accumulator = 0;
            hasData = false;
            Name = "Simple Accumulator";
            ClearDataAfterFlush = true;
            LogWhenEmpty = false;
            FlushPeriod = StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD; // 30 * 10000, 30 seconds
            lastFlushTicks = 0;
            DataUnitName = string.Empty;
            statsLog = m.Add(this);
        }

        // ----------------------------------- Public properties ----------------------------------

        public string Name { get; set; }
        public bool LogWhenEmpty { get; set; }
        public bool ClearDataAfterFlush { get; set; }
        public long FlushPeriod { get; set; }
        public string DataUnitName { get; set; }

        // -------------------------------- Injected instance data --------------------------------
 
        /// <summary>
        /// IOC container.
        /// </summary>
        
        private IKernel kernel = null;
        
        /// <summary>
        /// Used to lock all instance data
        /// </summary>
        
        private ILock dataLock = null;

        // --------------------------------- Local instance data ----------------------------------

        /// <summary>
        /// Logger for stats data.
        /// </summary>
        
        private ILogger statsLog = null;
       
        /// <summary>
        /// The accumulated value (supports reset)
        /// </summary>
        
        private double accumulator = 0;

        /// <summary>
        /// The accumulated value (over the entire lifetime of the collector)
        /// </summary>

        private double totalAccumulator = 0;
        
        /// <summary>
        /// True --> there is data to log
        /// </summary>

        private bool hasData = false;

        /// <summary>
        /// Time (in ticks) of last flush.
        /// </summary>

        public long lastFlushTicks = 0;
    }
}