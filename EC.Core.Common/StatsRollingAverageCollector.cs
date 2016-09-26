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
    /// Implementation for IStatsRollingAverageCollector.
    /// </summary>

    [TransientType]
    [RegisterAsType(typeof(IStatsRollingAverageCollector))]

    public class StatsRollingAverageCollector : IStatsRollingAverageCollector
    {
        // NOTE: Every 500 data points, remove data outside our window.
        // This is to avoid caller keeps adding data points, but the object never gets flushed (or didn't
        // retrieve the Value)

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

        public void Flush()
        {
            using (new ScopedLock(dataLock))
            {
                lastFlushTicks = DateTime.Now.Ticks;
                RemoveDataOutsideWindow();
                double total = dataPointList.Sum(dp => dp.Value);
                var avg = total / dataPointList.Count;

                if (dataPointList.Count > 0)
                {
                    statsLogger.InfoFormat("Avg Time:  [{0}] (last {1}ms) (#datapoints, rolling avg): {2}, {3:F2} {4}", Name, Window, dataPointList.Count, avg, DataUnitName);
                }
                else if (LogWhenEmpty)
                {
                    statsLogger.InfoFormat("Avg Time:  [{0}] (last {1}ms): No data", Name, Window);
                }

                if (ClearDataAfterFlush) Reset();
            }
        }

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

        public StatsRollingAverageCollector(IKernel k, ILock l, IStatsLogManager m)
        {
            kernel = k;
            dataLock = l;
            Name = "Rolling Average Collector";
            ClearDataAfterFlush = true;
            LogWhenEmpty = false;
            FlushPeriod = StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD; // 30 * 1000, 30 seconds
            Window = StatsCollectorConstants.ONE_MIN_FLUSH_PERIOD;       // 60 * 1000, 60 seconds
            DataUnitName = string.Empty;
            lastFlushTicks = 0;
            dataPointList = new List<DataPoint>(100);
            statsLogger = m.Add(this);
        }

        // ----------------------------------- Public properties ----------------------------------

        public string Name { get; set; }
        public long Window { get; set; }
        public bool LogWhenEmpty { get; set; }
        public bool ClearDataAfterFlush { get; set; }
        public long FlushPeriod { get; set; }
        public long lastFlushTicks { get; private set; }
        public string DataUnitName { get; set; }

        // ----------------------------------- Internal Methods -----------------------------------

        /// <summary>
        ///  Helper method to remove data points that are outside of our collecting window.
        ///  NOTE: It is assumed that the caller holds the lock for accessing instance
        ///  data.
        /// </summary>

        private void RemoveDataOutsideWindow()
        {
            // Note caller needs to get a lock

            long nowTicks = DateTime.Now.Ticks;

            // remove all the datapoints that are out of the mWindow

            dataPointList.RemoveAll(delegate(DataPoint dp)
            {
                long diff = nowTicks - dp.Ticks;
                TimeSpan elaspedSpan = new TimeSpan(diff);
                return (elaspedSpan.TotalMilliseconds > Window);
            });
        }

        // -----------------------------  Injected Instance Data ----------------------------------

        /// <summary>
        /// IOC kernel.
        /// </summary>

        private IKernel kernel = null;

        /// <summary>
        /// Lock for concurrency protection of all instance data.
        /// </summary>

        private ILock dataLock = null;

        // -----------------------------  Local Instance Data -------------------------------------

        /// <summary>
        /// Output sink for stats logging information.
        /// </summary>

        private ILogger statsLogger = null;

        /// <summary>
        /// Stores the set of data points for this collector.
        /// </summary>

        private readonly List<DataPoint> dataPointList = null;

        // --------------------------------- Private Data Structures ------------------------------

        /// <summary>
        /// Data points needs to be stored with a timestamp so that we can remove
        /// datapoints that are outside of the window over which the average is
        /// computed.
        /// </summary>

        private struct DataPoint
        {
            public long Ticks;    // the DateTime.Now.Ticks
            public double Value;  // the actual value
        }
    }
}