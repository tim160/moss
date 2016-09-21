using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using Castle.MicroKernel;
using Castle.Core.Logging;
using EC.Common.Base;
using EC.Common.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace EC.Core.Common
{
    /// <summary>
    /// Implementation for IStatsHistogram collector.
    /// </summary>
    
    [TransientType]
    [RegisterAsType(typeof(IStatsHistogramCollector))]

    public sealed class StatsHistogramCollector : IStatsHistogramCollector
    {      
        // There is a potential race condition when AddDataPoint is called before
        // SetupBins. In this case bins will be null and we just drop the data
        // point.

        public void AddDataPoint(double value)
        {
            if (bins == null) return;

            using (new ScopedLock(dataLock))
            {
                bool found = false;
                int i;

                for (i = 0; i < binRanges.Length; i++)
                {
                    if (value <= binRanges[i])
                    {
                        bins[i]++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // this is for the last bin (value greater than the highest range)
                    bins[i]++;
                }

                hasData = true;
            }
        }

        public void Flush()
        {
            if (bins == null) { return; }
            if (binRanges == null) { return; }

            using (new ScopedLock(dataLock))
            {
                lastFlushTicks = DateTime.Now.Ticks;

                if (hasData)
                {                   
                    // First bin...
                    if (((bins[0] > 0) && (!this.LogEmptyBins)) || (LogEmptyBins))
                    {
                        statsLog.InfoFormat("Histogram: [{0}] [     <= {1,5}]: {2} {3}", Name, binRanges[0], bins[0], DataUnitName);
                    }

                    int i;
                    double lowRange = binRanges[0];

                    for (i = 1; i < binRanges.Length; i++)
                    {
                        if (((bins[i] > 0) && (!this.LogEmptyBins)) || (LogEmptyBins))
                        {
                            statsLog.InfoFormat("Histogram: [{0}] [{1,5} - {2,5}]: {3} {4}", Name, lowRange, binRanges[i], bins[i], DataUnitName);
                        }
                        lowRange = binRanges[i];
                    }

                    // Last bin...
                    if (((bins[i] > 0) && (!this.LogEmptyBins)) || (LogEmptyBins))
                    {
                        statsLog.InfoFormat("Histogram: [{0}] [      > {1,5}]: {2} {3}", Name, lowRange, bins[i], DataUnitName);
                    }
                }
                else if (LogWhenEmpty)
                {
                    statsLog.InfoFormat("Histogram: [{0}] No data", Name);
                }

                if (ClearDataAfterFlush) { Reset(); }
            }
        }

        public void Reset()
        {
            using (new ScopedLock(dataLock))
            {
                Array.Clear(bins, 0, bins.Length);
                hasData = false;
            }
        }

        public void SetupBins(double[] binLimits, bool logEmptyBins = true)
        {
            binRanges = new double[binLimits.Length];
            Array.Copy(binLimits, binRanges, binLimits.Length);
            bins = new int[binRanges.Length + 2];
            this.LogEmptyBins = logEmptyBins;
            
            double lastVal = binRanges[0];

            for (int i = 1; i < binRanges.Length; i++)
            {
                double curVal = binRanges[i];

                if (lastVal >= curVal)
                {
                    var ex = new BinsNotInAscendingOrderException("Bin limits must be in ascending order");
                    ex.BinLimit = curVal;
                    ex.WhichBin = i;
                    ex.HistogramName = Name;
                    throw ex;
                }

                lastVal = curVal;
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

        // The number of bins 2 more than the size of the bin ranges because we want to capture the 
        // data less than the lowest range and data greater than the highest range

        public StatsHistogramCollector(IKernel k, ILock d, IStatsLogManager m)
        {
            kernel = k;
            dataLock = d;
            manager = m;
            Name = "Histogram Collector";
            LogWhenEmpty = true;
            LogEmptyBins = true;
            ClearDataAfterFlush = true;
            FlushPeriod = 60 * 1000;
            DataUnitName = string.Empty;
            hasData = false;
            statsLog = manager.Add(this);
        }

    // ----------------------------------- Public properties ----------------------------------

        public string Name { get; set; }
        public bool LogWhenEmpty { get; set; }
        public bool ClearDataAfterFlush { get; set; }
        public long FlushPeriod { get; set; }
        public bool LogEmptyBins { get; set; }
        public string DataUnitName { get; set; }

    // -------------------------------- Injected instance data --------------------------------

        /// <summary>
        /// Used to lock all instance data
        /// </summary>

        private ILock dataLock = null;

        /// <summary>
        /// The log manager.
        /// </summary>

        private IStatsLogManager manager = null;

    // -----------------------------  Local Instance Data -------------------------------------
        
        /// <summary>
        /// IOC kernel.
        /// </summary>

        private IKernel kernel = null;

        /// <summary>
        /// Logger for stats data.
        /// </summary>

        private ILogger statsLog = null;
        
        /// <summary>
        /// Stores the values that define the range of data values for each bin.
        /// </summary>
        
        public double[] binRanges = null;
        
        /// <summary>
        /// Bins in which histogram counts are stored.
        /// </summary>
        
        private int[] bins = null;

        /// <summary>
        /// Indicates whether any data has arrived since the last flush.
        /// </summary>

        private bool hasData = false;
        
        /// <summary>
        /// Time (in ticks) of last flush.
        /// </summary>
        
        private long lastFlushTicks = 0;
    }

    /// <summary>
    ///  A wrapper around the StatsHistogram class designed for measuring method
    ///  execution times.
    ///  It also maintains stats for rolling average for the calls.
    /// </summary>
   
    [TransientType]
    public class ExecutionTimeHistogram : IStatsExecutionTimeHistogram
    {
        /// <summary>
        /// Initialize the histogram.
        /// </summary>
        /// <remarks>
        /// The <paramref name="flushPeriod"/> should be smaller than the <paramref name="averageWindow"/>. 
        /// It doesn't make sense to have the <paramref name="flushPeriod"/> greater than the <paramref name="averageWindow"/>.
        /// If the <paramref name="flushPeriod"/> is set greater than <paramref name="averageWindow"/> the average window
        /// is set to <paramref name="flushPeriod"/>.
        /// </remarks>
        /// <param name="name">Name used in logging.</param>
        /// <param name="binLimits">Define the data value ranges for the histogram bins</param>
        /// <param name="flushPeriod">Time period for logging out data</param>
        /// <param name="logWhenEmpty">true -> log to output even when there is no data</param>
        /// <param name="logEmptyBins">Flag whether to log empty bins. 
        /// If <c>true</c> write all bins to the log. 
        /// If <c>false</c> omit empty bins to write to log.</param>
        /// <param name="averageWindow">Time window for rolling average.</param>
        
        public void Setup(string name, double[] binLimits, int flushPeriod, bool logWhenEmpty, bool logEmptyBins = true, long averageWindow = 30000)
        {
            histogram.Name = name;
            histogram.FlushPeriod = flushPeriod;
            histogram.LogWhenEmpty = logWhenEmpty;
            histogram.SetupBins(binLimits, logEmptyBins);
            histogram.DataUnitName = "calls";

            average.Name = name;
            average.FlushPeriod = flushPeriod;
            average.LogWhenEmpty = logWhenEmpty;
            average.Window = Math.Max(flushPeriod, averageWindow);
            average.DataUnitName = "ms";
        }

        /// <summary>
        /// Start a new execution time measurement. In order to allow for multiple threads to
        /// time concurrent events, this method returns a GUID that must be supplied on the
        /// corresponding call to stop the timer.
        /// </summary>
        /// <returns>a GUID to uniquely identify this timer</returns>

        public Guid RecordStart()
        {
            long ticks;
            PreciseTickCount.GetPreciseTickCount(out ticks);
            Guid key = Guid.NewGuid();

            using (new ScopedLock(StartTimeLock))
            {
               startTime.Add(key, PreciseTickCount.ToMilliSeconds(ticks));
            }

            return key;
        }

        /// <summary>
        /// Stop an execution time measurement. 
        /// </summary>
        /// <param name="startKey">The GUID from the corresponding RecordStart call</param>
        
        public void RecordStop(Guid startKey)
        {
            long ticks;
            PreciseTickCount.GetPreciseTickCount(out ticks);
            double stopTime = PreciseTickCount.ToMilliSeconds(ticks);

            using (new ScopedLock(StartTimeLock))
            {
                if (!startTime.ContainsKey(startKey)) {logger.Warn("RecordStop: Could not find start key"); return;}
                DBC.Assert(startTime[startKey] > 0, "Start time less than 0");
                histogram.AddDataPoint(stopTime - startTime[startKey]);  
                average.AddDataPoint(stopTime - startTime[startKey]);  
                startTime.Remove(startKey);
            }
        }

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public ExecutionTimeHistogram(IKernel k, ILogger l, IStatsHistogramCollector h, IStatsRollingAverageCollector avg, ILock lck)
        {
            histogram = h;
            average = avg;
            kernel = k;
            logger = l;
            this.StartTimeLock = lck; 
        }

        // ------------------------------------------ Local State ---------------------------------

        /// <summary>
        /// Histogram for holding timing data.
        /// </summary>
        
        private IStatsHistogramCollector histogram = null;
        
        /// <summary>
        /// Average collector for computing average time over data.
        /// </summary>
        
        private IStatsRollingAverageCollector average = null;

        /// <summary>
        /// Lock object to add/remove items from the dictionary.
        /// </summary>
        
        private ILock StartTimeLock { get; set; }

        /// <summary>
        /// Holds data for all outstanding timers.
        /// </summary>

        private IDictionary<Guid, double> startTime = new Dictionary<Guid, double>();

        /// <summary>
        /// IoC Kernel
        /// </summary>

        private IKernel kernel;

        /// <summary>
        /// For logging
        /// </summary>

        private ILogger logger;
    }
}