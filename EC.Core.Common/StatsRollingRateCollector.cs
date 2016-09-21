using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Castle.MicroKernel;
using Castle.Core.Logging;
using EC.Common.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Implementation of IStatsRollingRateCollector. This assumes that AddDataPoint() is 
    /// always called with the value of '1' to signal the occurrence of an event. The
    /// logged data is then the rate at which that event occurred, averaged over the 
    /// window size.
    /// <remarks>
    /// The implementation is virtually identical to that of IStatsRollingAverageCollector,
    /// the only difference being that way that the average values are computed. As with
    /// the rolling average, it does not make sense for FlushPeriod &gt; Window.
    /// </remarks>
    /// </summary>
   
    [TransientType]
    [RegisterAsType(typeof(IStatsRollingRateCollector))]

    public class StatsRollingRateCollector : IStatsRollingRateCollector
    {
        /// <summary>
        /// Add a data point to the collector. In order for this collector to display 
        /// meaningful data, the value passed in should always by 1.
        /// </summary>
        /// <remarks>
        /// Every 500 data points, remove data outside our window.
        /// This is to avoid caller keeps adding data points, but the object never gets flushed (or didn't
        /// retrieve the Value)
        /// </remarks>
        /// <param name="value">Must be 1</param>
        
        public void AddDataPoint(double value)
        {
            DBC.Assert(dataPointList != null, "dataPointList == null");

            using (new ScopedLock(dataLock))
            {
                DataPoint dp;
                dp.Ticks = DateTime.Now.Ticks;
                dp.Value = value;
                dataPointList.Add(dp);

                if (dataPointList.Count % 500 == 0)
                {
                    RemoveDataOutsideWindow();
                }
            }
        }

        /// <summary>
        /// Log the current stats information for this collector.
        /// </summary>
        
        public void Flush()
        {
            using (new ScopedLock(dataLock))
            {
                lastFlushTicks = DateTime.Now.Ticks;
                RemoveDataOutsideWindow();
                int eventCount = dataPointList.Sum(dp => (int)dp.Value);
                double rate = (((double)eventCount) / (double) Window) * 1000.0;

                if (dataPointList.Count > 0)
                {
                    statsLog.InfoFormat("Avg Rate:  [{0}] (last {1}ms): (#datapoints, rolling rate): {2}, {3:F2} {4}/s",
                        Name, Window, dataPointList.Count, rate, DataUnitName);
                }
                else if (LogWhenEmpty)
                {
                    statsLog.InfoFormat("Avg Rate:  [{0}] (last {1}ms): No data", Name, Window);
                }

                if (ClearDataAfterFlush) Reset();
            }
        }

        /// <summary>
        /// Remove all data points from the collector.
        /// </summary>
        
        public void Reset()
        {
            using (new ScopedLock(dataLock))
            {
                dataPointList.Clear();
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

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public StatsRollingRateCollector(IKernel k, ILock l, IStatsLogManager m)
        {
            kernel = k;
            dataLock = l;
            Name = "Rolling Rate Collector";
            ClearDataAfterFlush = true;
            LogWhenEmpty = false;
            FlushPeriod = StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD;  // 30 * 1000, 30 seconds
            Window = StatsCollectorConstants.ONE_MIN_FLUSH_PERIOD; // 60 * 1000, 60 seconds
            DataUnitName = string.Empty;
            lastFlushTicks = 0;
            dataPointList = new List<DataPoint>(500);
            statsLog = m.Add(this);
        }

        // ----------------------------------- Internal Methods -----------------------------------

        /// <summary>
        ///  Helper method to remove data points that are outside of our collecting window.
        ///  <remarks>
        ///  Note caller needs to get a lock
        ///  </remarks>
        /// </summary>
        
        private void RemoveDataOutsideWindow()
        {
            long nowTicks = DateTime.Now.Ticks;

            // remove all the data points that are out of the window

            dataPointList.RemoveAll(delegate(DataPoint dp)
                {
                    long diff = nowTicks - dp.Ticks;
                    TimeSpan elaspedSpan = new TimeSpan(diff);
                    return (elaspedSpan.TotalMilliseconds > Window);
                });
        }

        // ----------------------------------- Public properties ----------------------------------

        public string Name { get; set; }
        public long Window { get; set; }
        public bool LogWhenEmpty { get; set; }
        public bool ClearDataAfterFlush { get; set; }
        public long FlushPeriod { get; set; }
        public long lastFlushTicks { get; private set; }
        public string DataUnitName { get; set; }

        // -------------------------------- Injected instance data --------------------------------

        private IKernel kernel = null;
        private ILock dataLock = null;

        // -----------------------------  Local Instance Data -------------------------------------

        private ILogger statsLog = null;
        private readonly List<DataPoint> dataPointList = null;

        // --------------------------------- Private Data Structures ------------------------------

        /// <summary>
        /// Data points needs to be stored with a timestamp so that we can remove
        /// data points that are outside of the window over which the average is
        /// computed.
        /// </summary>
      
        private struct DataPoint
        {
            public long Ticks; // the DateTime.Now.Ticks
            public double Value; // the actual value
        }
    }
}