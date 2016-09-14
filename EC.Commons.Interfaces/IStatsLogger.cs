using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using EC.Common.Base;

namespace EC.Common.Interfaces
{
    /// <summary>
    ///    IStatsCollector is an interface representing functionality common to all
    ///    stats collecting implementations. It is based on the notion that a
    ///    collector accumulates data points over time, and then logs information
    ///    about those out to a text file.
    ///    log file (<see cref = "Logger.sStatsLog" />).
    ///    <list type = "bullet">
    ///       <item><description>StatsAccumulatorCollector</description></item>
    ///       <item><description>StatsHistogramCollector</description></item>
    ///       <item><description>StatsRollingAverageCollector</description></item>
    ///       <item><description>StatsRollingRateCollector</description></item>
    ///    </list>
    /// </summary>
    
    public interface IStatsCollector
    {
        /// <summary>
        /// Add the specified data point to the StatsCollector.
        /// </summary>
        /// <param name = "value">The data point to add.</param>
       
        void AddDataPoint(double v);

        /// <summary>
        /// Resets the collector to an empty state.
        /// </summary>

        void Reset();

        /// <summary>
        ///    Invoked by the flusher thread when its time to flush the stats data
        ///    to the log file.
        /// </summary>
        
        void Flush();

        /// <summary>
        /// Compute the number of milliseconds until the collector should next be flushed.
        /// </summary>
        /// <returns>Milliseconds until next flush required</returns>

        long GetTimeToNextFlush();

        /// <summary>
        /// Name of this collector. Used in logging displays.
        /// </summary>

        string Name { get; set; }

        /// <summary>
        /// Time (in milliseconds) between log flushes for this collector. Should only
        /// be set at initialization time.
        /// </summary>
        
        long FlushPeriod { get; set; }
        
        /// <summary>
        /// If true, indicates that the data in the collector should be totally cleared
        /// out after the collector has been flushed.
        /// </summary>
        
        bool ClearDataAfterFlush { get; set; }

        /// <summary>
        /// Forces the collectors to add an entry to the log file, even if it has not
        /// collected any data.
        /// </summary>
        
        bool LogWhenEmpty { get; set; }

        /// <summary>
        /// Unit name which is appended to the log.
        /// Default: ""
        /// </summary>

        string DataUnitName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    
    public interface IStatsAccumulatorCollector : IStatsCollector
    {
    }

    /// <summary>
    /// Maintains a rolling average for a set of data points over a given
    /// time window.
    /// <remarks>
    /// NOTE: It does not make sense to have the Window &lt; FlushPeriod 
    /// because you would not report on some data.
    /// </remarks>
    /// </summary>
    
    public interface IStatsRollingAverageCollector : IStatsCollector
    {
        /// <summary>
        /// The time window (in milliseconds) over which the rolling average is
        /// accumulated.
        /// </summary>
        
        long Window { get; set; }
    }

    /// <summary>
    /// Maintains a rolling average of a rate-valued variable over a given time window.
    /// The normal mode of use is that AddDataPoint() is always called with the value
    /// '1'; essentially signalling the occurrence of a given event. The information
    /// logged is then the rate at which these events have occurred over the specific 
    /// time window.
    /// <remarks>
    /// NOTE: It does not make sense to have the Window &lt; FlushPeriod 
    /// because you would not report on some data.
    /// </remarks>
    /// </summary>
    
    public interface IStatsRollingRateCollector : IStatsCollector
    {
        /// <summary>
        /// The time window (in milliseconds) over which the rolling average is
        /// accumulated.
        /// </summary>
        
        long Window { get; set; }
    }

    /// <summary>
    /// Maintains a histogram for set of data points over a given time period (the flush period).
    /// StatsHistorgram collects all the data points and put them into the corresponding
    /// bin within a flushing period.
    /// <para>
    ///  When it is time to flush the output (determined by the flushing period), the 
    ///  number of data points of each bin will be written to the Stats log. If
    ///  ClearDataOnFlush is set, the bins will be emptied.
    ///  </para>
    ///  <para>
    ///  E.g. The bins are defined to be 0-10, 10-20, and 20-30. If the data points
    ///  of 3, 5, 21, 22, and 29 are added in the current reporting period, then the output
    ///  will be like:
    ///  </para>
    ///    &lt; 0: 0,
    ///    0-10: 2,
    ///    10-20: 0,
    ///    20-30: 3,
    ///    &gt; 30: 0
    /// <remarks>
    /// NOTE: It does not make sense to have the Window &lt; FlushPeriod 
    /// because you would not report on some data.
    /// </remarks>
    /// </summary>

    public interface IStatsHistogramCollector : IStatsCollector
    {
        /// <summary>
        /// Defines the set of bins which the histogram will collect data
        /// for. Two implicit bins are created, one for values less than
        /// BinLimits[0] and one for values greater than
        /// the last element in BinLimits.
        /// </summary>
        /// <param name="binLimits">Data bins</param>
        /// <param name="logEmptyBins">Flag whether to log empty bins. 
        /// If <c>true</c> write all bins to the log. 
        /// If <c>false</c> omit empty bins to write to log.</param>

        void SetupBins(double[] binLimits, bool logEmptyBins = true);

        /// <summary>
        /// Flag whether to log empty bins. 
        /// If <c>true</c> write all bins to the log. 
        /// If <c>false</c> omit empty bins to write to log.
        /// </summary>
        
        bool LogEmptyBins { get; set; }
    }

    /// <summary>
    /// A simple wrapper around IStatsHistogramCollector designed to make it easy to
    /// collect a data set on the execution time of a segment of code or other event
    /// that can be bracketed in the code.
    /// </summary>
    
    public interface IStatsExecutionTimeHistogram
    {
        /// <summary>
        /// Initialize the histogram.
        /// </summary>
        ///  <remarks>
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

        void Setup(string name, double[] binLimits, int flushPeriod, bool logWhenEmpty, bool logEmptyBins = true, long averageWindow = 30000);
        
        /// <summary>
        /// Start a new execution time measurement. In order to allow for multiple threads to
        /// time concurrent events, this method returns a GUID that must be supplied on the
        /// corresponding call to stop the timer.
        /// </summary>
        /// <returns>a GUID to uniquely identify this timer</returns>
        
        Guid RecordStart();
        
        /// <summary>
        /// Stop an execution time measurement. 
        /// </summary>
        /// <param name="startKey">The GUID from the corresponding RecordStart call</param>
        
        void RecordStop(Guid startKey);
    }

    /// <summary>
    /// Handles centralized control over all IStatsCollector instances. In particular,
    /// the manager is responsible for causing the collectors to periodically flush their
    /// data out to the stats log.
    /// </summary>
    
    public interface IStatsLogManager
    {
        /// <summary>
        /// Notify the stats log manager of the existence of a collector. This is also
        /// how the collector gets the actual stats log for logging to.
        /// </summary>
        /// <param name="c">the collector to add</param>
        /// <returns>the stats logger</returns>
        
        ILogger Add(IStatsCollector c);

        /// <summary>
        /// Remove a collector from the manager.
        /// </summary>
        /// <param name="c">the collector to remove</param>

        void Remove(IStatsCollector c);
    }

    /// <summary>
    ///    Thrown when a bin array is provided that is not in ascending order.
    /// </summary>

    public class BinsNotInAscendingOrderException : Exception
    {
        public BinsNotInAscendingOrderException()
        {
        }

        public BinsNotInAscendingOrderException(string msg) : base(msg)
        {
        }

        public int WhichBin { get; set; }
        public double BinLimit { get; set; }
        public string HistogramName { get; set; }
    }
}
