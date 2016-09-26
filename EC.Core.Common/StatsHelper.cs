using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Castle.MicroKernel;
using Castle.Core.Logging;
using EC.Common.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Helper class for statistics.
    /// </summary>

    [SingletonType]
    public class StatsHelper : IStatsHelper, IDisposable
    {
        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <remarks>Flush time is every 30s</remarks>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="code">the code to time</param>
        
        public void TimeCodeWithinMethod(string segmentName, Action code)
        {
            this.TimeCodeWithinMethod(segmentName, StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }
 
        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">the code to time</param>
        
        public void TimeCodeWithinMethod(string segmentName, int flushPeriod, Action code)
        {
            var histogramName = this.GetCallerName() + "." + segmentName;
            TimeCode(histogramName, null, flushPeriod, code);
        }

        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <remarks>Flush period is 30s</remarks>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="code">the code to time</param>
       
        public T TimeCodeWithinMethod<T>(string segmentName, Func<T> code)
        {
            return this.TimeCodeWithinMethod<T>(segmentName, StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }
        
        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">the code to time</param>
       
        public T TimeCodeWithinMethod<T>(string segmentName, int flushPeriod, Func<T> code)
        {
            var histogramName = this.GetCallerName() + "." + segmentName;
            return TimeCode(histogramName, null, flushPeriod, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates both a rate counter and an execution time histogram
        /// for the method. NOTE: This should only be used to wrap the complete code
        /// of a method.
        /// </summary>
        /// <remarks>
        /// The histogram's name is: class name + calling method name.
        /// Flush period is 30s.
        /// </remarks>
        /// <param name="code">code to execute</param>

        public void TimeMethodBody(Action code)
        {
            this.TimeMethodBody(StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates both a rate counter and an execution time histogram
        /// for the method. NOTE: This should only be used to wrap the complete code
        /// of a method.
        /// </summary>
        /// <remarks>
        /// The histogram's name is: class name + calling method name.
        /// </remarks>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">code to execute</param>

        public void TimeMethodBody(int flushPeriod, Action code)
        {
            var callerName = this.GetCallerName();
            TimeCode(callerName, null, flushPeriod, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates both a rate counter and an execution time histogram
        /// for the method. NOTE: This should only be used to wrap the complete code
        /// of a method. This version must be used if the body of the method has
        /// a return value.
        /// </summary>
        /// <remarks>
        /// The default flush period is 30s.
        /// </remarks>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="code">code to run</param>
        /// <returns>the value from executing the code</returns>

        public T TimeMethodBody<T>(Func<T> code)
        {
            return TimeMethodBody<T>(StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates both a rate counter and an execution time histogram
        /// for the method. NOTE: This should only be used to wrap the complete code
        /// of a method. This version must be used if the body of the method has
        /// a return value.
        /// </summary>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">code to run</param>
        /// <returns>the value from executing the code</returns>

        public T TimeMethodBody<T>(int flushPeriod, Func<T> code)
        {
            var callerName = this.GetCallerName();
            return TimeCode(callerName, null, flushPeriod, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates a rate counter for the method, and a series of histograms
        /// to collect execution times. The execution times are grouped according to the
        /// groupName parameter. This allows the execution time of the method body to
        /// be broken down into a set of different cases.  NOTE: This should only be 
        /// used to wrap the complete code of a method.
        /// </summary>
        /// <remarks>
        /// Flush period is 30s.
        /// Two histograms are maintained:
        ///  1.) prefix.caller name
        ///  2.) caller name only
        /// caller name = class name + calling method name
        /// </remarks>
        /// <param name="groupName">Prefix for the log text.</param>
        /// <param name="code">code to execute</param>

        public void TimeMethodBodyWithGrouping(string groupName, Action code)
        {
            this.TimeMethodBodyWithGrouping(groupName, StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }
 
        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates a rate counter for the method, and a series of histograms
        /// to collect execution times. The execution times are grouped according to the
        /// groupName parameter. This allows the execution time of the method body to
        /// be broken down into a set of different cases.  NOTE: This should only be 
        /// used to wrap the complete code of a method.
        /// </summary>
        /// <remarks>
        /// Two histograms are maintained:
        ///  1.) prefix.caller name
        ///  2.) caller name only
        /// caller name = class name + calling method name
        /// </remarks>
        /// <param name="groupName">Prefix for the log text.</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">code to execute</param>

        public void TimeMethodBodyWithGrouping(string groupName, int flushPeriod, Action code)
        {
            var callerName = this.GetCallerName();
            TimeCode(callerName, groupName, flushPeriod, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates a rate counter for the method, and a series of histograms
        /// to collect execution times. The execution times are grouped according to the
        /// groupName parameter. This allows the execution time of the method body to
        /// be broken down into a set of different cases. NOTE: This should only be used
        /// to wrap the complete code of a method. This version must be used if the body 
        /// of the method has a return value.
        /// </summary>
        /// <remarks>
        /// Flush period is 30s.
        /// Two histograms are maintained:
        ///  1.) prefix.caller name
        ///  2.) caller name only
        /// caller name = class name + calling method name
        /// </remarks>
        /// <param name="groupName">Prefix for the log text</param>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="code">code to run</param>
        /// <returns>the value from executing the code</returns>
        
        public T TimeMethodBodyWithGrouping<T>(string groupName, Func<T> code)
        {
            return TimeMethodBodyWithGrouping<T>(groupName, StatsCollectorConstants.HALF_MIN_FLUSH_PERIOD, code);
        }

        /// <summary>
        /// Helper function for collecting timing information for the body of a method.
        /// This code creates a rate counter for the method, and a series of histograms
        /// to collect execution times. The execution times are grouped according to the
        /// groupName parameter. This allows the execution time of the method body to
        /// be broken down into a set of different cases. NOTE: This should only be used
        /// to wrap the complete code of a method. This version must be used if the body 
        /// of the method has a return value.
        /// </summary>
        /// <remarks>
        /// Two histograms are maintained:
        ///  1.) prefix.caller name
        ///  2.) caller name only
        /// caller name = class name + calling method name
        /// </remarks>
        /// <param name="groupName">Prefix for the log text</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="code">code to run</param>
        /// <returns>the value from executing the code</returns>
        
        public T TimeMethodBodyWithGrouping<T>(string groupName, int flushPeriod, Func<T> code)
        {
            var callerName = this.GetCallerName();
            return TimeCode(callerName, groupName, flushPeriod, code);
        }

        /// <summary>
        /// Get call rate collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <remarks>Flush period is 1min</remarks>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return call rate collector.</returns>

        public IStatsRollingRateCollector GetCallRateCollector(string suffix)
        {
            return GetCallRateCollector(suffix, StatsCollectorConstants.ONE_MIN_FLUSH_PERIOD);
        }

        /// <summary>
        /// Get call rate collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <returns>Return call rate collector.</returns>

        public IStatsRollingRateCollector GetCallRateCollector(string suffix, int flushPeriod)
        {
            if (!String.IsNullOrWhiteSpace(suffix)) { suffix = "." + suffix; }
            var callerName = this.GetCallerName() + suffix;
            return GetCallRateCollectorByName(callerName,flushPeriod);
        }

        /// <summary>
        /// Get rolling average collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return the rolling average collector.</returns>

        public IStatsRollingAverageCollector GetAverageCollector(string suffix = "")
        {
            return GetAverageCollector(StatsCollectorConstants.ONE_MIN_FLUSH_PERIOD, suffix);
        }
        
        /// <summary>
        /// Get rolling average collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return the rolling average collector.</returns>

        public IStatsRollingAverageCollector GetAverageCollector(int flushPeriod, string suffix = "")
        {
            if (!String.IsNullOrWhiteSpace(suffix)) { suffix = "-" + suffix; }
            var callerName = this.GetCallerName() + suffix;
            return GetAverageCollectorByName(callerName, flushPeriod);
        }

        /// <summary>
        /// Get execution time collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the underlying class name + caller method name (e.g. 'UserServiceImpl.AddUser').
        /// </summary>
        /// <remarks>Flush period is 1 min</remarks>
        /// <param name="suffix">Optional postfix to identify the calling point. </param>
        /// <returns>Return execution time collector.</returns>

        public IStatsExecutionTimeHistogram GetExecutionTimeCollector(string suffix = "")
        {
            return GetExecutionTimeCollector(StatsCollectorConstants.ONE_MIN_FLUSH_PERIOD, suffix);
        }
        
        /// <summary>
        /// Get execution time collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the underlying class name + caller method name (e.g. 'UserServiceImpl.AddUser').
        /// </summary>
        /// <param name="suffix">Optional postfix to identify the calling point. </param>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <returns>Return execution time collector.</returns>

        public IStatsExecutionTimeHistogram GetExecutionTimeCollector(int flushPeriod, string suffix = "")
        {
            if (!String.IsNullOrWhiteSpace(suffix)) { suffix = "-" + suffix; }
            var callerName = this.GetCallerName() + suffix;
            return GetExecutionTimeCollectorByName(callerName, flushPeriod);
        }

        /// <summary>
        /// Get file extension if the path has a file name at the end.
        /// </summary>
        /// <param name="path">Path to get the file extension name of</param>
        /// <returns>Return file extension. Return empty string if no path or file name exists.</returns>

        public string GetFileExtensionFromPath(string path)
        {
            if ((path == null) || (string.IsNullOrWhiteSpace(path)))
            {
                return string.Empty;
            }

            var extensionSplit = path.Split(new char[] { '.' });
            if (extensionSplit.Length >= 2)
            {
                // Return the last split, which is the file extension...
                return extensionSplit[extensionSplit.Length - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Generate a bin array filled with bin values from [start,end] with <c>interval</c> 
        /// as gaps between the bins.
        /// </summary>
        /// <remarks>
        /// If the <c>interval</c> doesn't fit in for the last interval, it just use the 
        /// remaining as gap between the second last bin and the last bin (see examples for more detail).
        /// </remarks>
        /// <example>
        /// start = 5, end = 15, interval = 5 ==> (5, 10, 15)
        /// start = 5, end = 17, interval = 5 ==> (5, 10, 15, 17)
        /// start = 5, end = 5,  interval = 4 ==> (5)
        /// start = 5, end = 8,  interval = 5 ==> (5, 8)
        /// start = 5, end = 34, interval = 0 ==> (5, 34)
        /// </example>
        /// <param name="start">Start value for the first bin.</param>
        /// <param name="end">Last value for the bin.</param>
        /// <param name="interval">Interval between the bins.</param>
        /// <returns>Return the bin array.</returns>
        /// <exception cref="ArgumentException">If the start is bigger than the end or the interval is smaller than 0</exception>

        public double[] GenerateBins(int start, int end, int interval)
        {
            if (start > end)
            {
                throw new ArgumentException("Start time must be <= than the end time.");
            }
            if (interval < 0)
            {
                throw new ArgumentException("Interval must be >= 0.");
            }

            if ((end - start) == 0)
            {
                // If only one bin...
                return new double[] { start };
            }

            if (interval == 0)
            {
                // Only 2 bins...
                return new double[] { start, end };
            }

            // Add all bins without last bin (=end).
            IList<double> bins = new List<double>();
            double currentBinValue = start;

            while (currentBinValue < end)
            {
                bins.Add(currentBinValue);
                currentBinValue += interval;
            }

            // Add last bin...
            bins.Add(end);

            return bins.ToArray();
        }

        /// <summary>
        /// Time the code in given lambda. A call rate counter and a histogram are created
        /// for the primaryName. A set of histograms are created, one for each unique
        /// groupName. Timing data is grouped by the groupName. If groupName is null, then
        /// only performance data is only collected for the primaryName.
        /// </summary>
        /// <param name="primaryName">name under which performance data will be collected</param>
        /// <param name="groupName">name of group for grouped performance data</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <param name="code">code to time</param>

        private void TimeCode(string primaryName, string groupName, int flushPeriod, Action code)
        {
            var prefixedName = groupName ?? primaryName;
            Guid timerKey1 = Guid.Empty;
            Guid timerKey2 = Guid.Empty;

            try
            {
                GetCallRateCollectorByName(primaryName, flushPeriod).AddDataPoint(1);
                if (groupName != null) { timerKey1 = GetExecutionTimeCollector(flushPeriod, prefixedName).RecordStart(); }
                timerKey2 = GetExecutionTimeCollectorByName(primaryName, flushPeriod).RecordStart();
                code();
            }
            finally
            {
                if (groupName != null) { GetExecutionTimeCollector(flushPeriod, prefixedName).RecordStop(timerKey1); }
                GetExecutionTimeCollectorByName(primaryName, flushPeriod).RecordStop(timerKey2);
            }
        }

        /// <summary>
        /// Time the code in given lambda. A call rate counter and a histogram are created
        /// for the primaryName. A set of histograms are created, one for each unique
        /// groupName. Timing data is grouped by the groupName. If groupName is null, then
        /// only performance data is only collected for the primaryName. This version must
        /// be used if the lambda returns a value.
        /// </summary>
        /// <param name="primaryName">name under which performance data will be collected</param>
        /// <param name="groupName">name of group for grouped performance data</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <param name="code">code to time</param>

        private T TimeCode<T>(string primaryName, string groupName, int flushPeriod, Func<T> code)
        {
            var prefixedName = groupName ?? primaryName;
            Guid timerKey1 = Guid.Empty;
            Guid timerKey2 = Guid.Empty;

            try
            {
                GetCallRateCollectorByName(primaryName, flushPeriod).AddDataPoint(1);
                if (groupName != null) { timerKey1 = GetExecutionTimeCollector(flushPeriod, prefixedName).RecordStart(); }
                timerKey2 = GetExecutionTimeCollectorByName(primaryName, flushPeriod).RecordStart();
                return code();
            }
            finally
            {
                if (groupName != null) { GetExecutionTimeCollector(flushPeriod, prefixedName).RecordStop(timerKey1); }
                GetExecutionTimeCollectorByName(primaryName, flushPeriod).RecordStop(timerKey2);
            }
        }

        /// <summary>
        /// Retrieve or create an execution histogram based on a given name.
        /// </summary>
        /// <param name="name">the name of the histogram</param>
        /// <param name="flushPeriod"></param>
        /// <returns>a collector object which can be used to record data points</returns>

        private IStatsExecutionTimeHistogram GetExecutionTimeCollectorByName(string name, int flushPeriod)
        {
            try
            {
                DBC.Assert(flushPeriod > 1, "Histogram flushPeriod must be greater than 1ms.");

                using (new ScopedLock(this.DataLockExecTime))
                {
                    if (this.execTimeCollectors.ContainsKey(name)) { return this.execTimeCollectors[name]; }
                    var newCollector = this.Kernel.Resolve<IStatsExecutionTimeHistogram>();  // Released in Dispose()
                    newCollector.Setup(name, StatsCollectorConstants.BIN_DEFAULT_TIMING, flushPeriod, false, false);
                    this.execTimeCollectors.Add(name, newCollector);
                    return newCollector;
                }
            }
            catch (Exception ex)
            {
                this.Logger.WarnFormat("GetExecutionTimeCollectorByName: Unexpected exception '{0}': {1}", name, ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get call rate collector by name. Create a new if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <returns>Return call rate collector.</returns>

        private IStatsRollingRateCollector GetCallRateCollectorByName(string name, int flushPeriod)
        {
            try
            {
                DBC.Assert(flushPeriod > 1, "Flush period must be greater than 1ms.");
                using (new ScopedLock(this.DataLockCallRate))
                {
                    if (this.callRateCollectors.ContainsKey(name)) { return this.callRateCollectors[name]; }
                    var newCollector = this.Kernel.Resolve<IStatsRollingRateCollector>();  // Released in Disposed()
                    newCollector.Name = name;
                    newCollector.DataUnitName = "calls";
                    newCollector.FlushPeriod = flushPeriod;
                    this.callRateCollectors.Add(name, newCollector);
                    return newCollector;       
                }
            }
            catch (Exception ex)
            {
                this.Logger.WarnFormat("Error to get the call rate collector '{0}': {1}", name, ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get rolling average collector by name. Create a new if it doesn't exist.
        /// </summary>
        /// <param name="name">Name of the collector</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms. Default is 60,000 ms (1min)</param>
        /// <returns>Return the rolling average collector.</returns>

        private IStatsRollingAverageCollector GetAverageCollectorByName(string name, int flushPeriod = 60000)
        {
            try
            {
                DBC.Assert(flushPeriod > 1, "Flush period of a stats collector can't be < 1ms.");

                using (new ScopedLock(this.DataLockAverage))
                {
                    if (this.avgCollectors.ContainsKey(name)) { return this.avgCollectors[name]; }
                    var newCollector = this.Kernel.Resolve<IStatsRollingAverageCollector>();  // Released in Dispose()
                    newCollector.Name = name;
                    newCollector.DataUnitName = "ms";
                    newCollector.FlushPeriod = flushPeriod;
                    this.avgCollectors.Add(name, newCollector);
                    return newCollector;
                }
            }
            catch (Exception ex)
            {
                this.Logger.WarnFormat("Error to get the rolling average collector '{0}': {1}", name, ex.ToString());
                return null;
            }
        }
        
        /// <summary>
        /// Return information about the code calling into the StatsHelper. NOTE: This
        /// routine heuristically tries to determine the class and method of the code
        /// that calls into the StatsHelper module. It does this by walking up the stack
        /// to the first entry it finds that seems like it might be a real caller. The
        /// two main heuristics used at the moment are: (1) ignore stack frames in which
        /// the class is "ServiceStatsHelper" (2) ignore stack frames in which the 
        /// class name begins with less than or greater than -- these are classes that are generated on the
        /// fly and are usually not the "real" caller we are looking for.
        /// </summary>
        /// <remarks>
        /// stackframe(0) is for this call
        /// stackframe(1) is for the entry point into the StatsHelper
        /// stackframe(2) is for the caller into StatsHelper
        /// </remarks>
        /// <returns>a string of the form ClassName.MethodName</returns>
        
        private string GetCallerName()
        {
            var stack = new StackTrace();

            for (var curr = 0; curr < stack.FrameCount - 1; curr++ )
            {
                var targetFrame = stack.GetFrame(curr);
                var method = targetFrame.GetMethod();
                if (method == null) continue;
                var methodName = method.Name;
                if (methodName == null) continue;
                var declaringType = method.DeclaringType;
                if (declaringType == null) continue;
                var className = declaringType.Name;
                if (className == null) continue;
                if (className == "StatsHelper") continue;
                if (className.StartsWith("<>")) continue;
                return className + "." + methodName;
            }

            return "[Unknown (full stack inspected)]";
        }

        /// <summary>
        /// IoC Constructor.
        /// </summary>
        /// <param name="k">Automatically filled in by IoC.</param>
        /// <param name="l">Automatically filled in by IoC.</param>
        /// <param name="execLock"></param>
        /// <param name="callLock"></param>
        /// <param name="avgLock"></param>

        public StatsHelper(IKernel k, ILogger l, ILock execLock, ILock callLock, ILock avgLock)
        {
            this.Kernel = k;
            this.Logger = l;

            // Set lock objects...
            this.DataLockExecTime = execLock;
            this.DataLockCallRate = callLock;
            this.DataLockAverage = avgLock;
        }

        // -------------------------------- Object State ------------------------------------------

        /// <summary>
        /// Used to lock execution time collector...
        /// </summary>

        private ILock DataLockExecTime { get; set; }
 
        /// <summary>
        /// Used to lock call rate collector...
        /// </summary>
        
        private ILock DataLockCallRate { get; set; }
 
        /// <summary>
        /// Used to lock rolling average collector...
        /// </summary>
        
        public ILock DataLockAverage { get; set; }

        /// <summary>
        /// Execution time collectors identified by caller names.
        /// </summary>
        
        private IDictionary<string, IStatsExecutionTimeHistogram> execTimeCollectors = new Dictionary<string, IStatsExecutionTimeHistogram>();


        /// <summary>
        /// Call rate collectors identified by caller names.
        /// </summary>
        
        private IDictionary<string, IStatsRollingRateCollector> callRateCollectors = new Dictionary<string, IStatsRollingRateCollector>();

        /// <summary>
        /// Average collector identified by callers names.
        /// </summary>

        private IDictionary<string, IStatsRollingAverageCollector> avgCollectors = new Dictionary<string, IStatsRollingAverageCollector>();

        private IKernel Kernel { get; set; }
        private ILogger Logger { get; set; }

        /// <summary>
        /// Release IOC objects resolved in this class.
        /// </summary>

        public void Dispose()
        {
           
            if (this.avgCollectors != null)
            {
                while (this.avgCollectors.Count != 0)
                {
                    var kvPair = this.avgCollectors.First();
                    this.avgCollectors.Remove(kvPair.Key);
                    this.Kernel.ReleaseComponent(kvPair.Value);
                }
            }
            if (this.callRateCollectors != null)
            {
                while (this.callRateCollectors.Count != 0)
                {
                    var kvPair = this.callRateCollectors.First();
                    this.callRateCollectors.Remove(kvPair.Key);
                    this.Kernel.ReleaseComponent(kvPair.Value);
                }
            }
            if (this.execTimeCollectors != null)
            {
                while (this.execTimeCollectors.Count != 0)
                {
                    var kvPair = this.execTimeCollectors.First();
                    this.execTimeCollectors.Remove(kvPair.Key);
                    this.Kernel.ReleaseComponent(kvPair.Value);
                }
            }
        }
    }
}
