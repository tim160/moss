using System;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Helper class for statistics.
    /// </summary>
    
    public interface IStatsHelper
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

        void TimeCodeWithinMethod(string segmentName, Action code);
 
        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">the code to time</param>

        void TimeCodeWithinMethod(string segmentName, int flushPeriod, Action code);

        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <remarks>Flush period is 30s</remarks>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="code">the code to time</param>

        T TimeCodeWithinMethod<T>(string segmentName, Func<T> code);
        
        /// <summary>
        /// A helper method to collect timing information from a code sequence
        /// within a method. Timing information is collected as both a 
        /// call rate counter and a historgram.The histogram's name is: 
        /// calling method's name . segment name.
        /// </summary>
        /// <param name="segmentName">a string which identifies the sequence of code to be timed</param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <param name="code">the code to time</param>

        T TimeCodeWithinMethod<T>(string segmentName, int flushPeriod, Func<T> code);

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

        void TimeMethodBody(Action code);

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

        void TimeMethodBody(int flushPeriod, Action code);

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

        T TimeMethodBody<T>(Func<T> code);

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

        T TimeMethodBody<T>(int flushPeriod, Func<T> code);

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

        void TimeMethodBodyWithGrouping(string groupName, Action code);
 
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

        void TimeMethodBodyWithGrouping(string groupName, int flushPeriod, Action code);

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

        T TimeMethodBodyWithGrouping<T>(string groupName, Func<T> code);

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

        T TimeMethodBodyWithGrouping<T>(string groupName, int flushPeriod, Func<T> code);

        /// <summary>
        /// Get call rate collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <remarks>Flush period is 1min</remarks>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return call rate collector.</returns>

        IStatsRollingRateCollector GetCallRateCollector(string suffix);

        /// <summary>
        /// Get call rate collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <param name="flushPeriod">Flush period of the histogram in ms (must be greater than 1ms).</param>
        /// <returns>Return call rate collector.</returns>

        IStatsRollingRateCollector GetCallRateCollector(string suffix, int flushPeriod);

        /// <summary>
        /// Get rolling average collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return the rolling average collector.</returns>

        IStatsRollingAverageCollector GetAverageCollector(string suffix = "");
        
        /// <summary>
        /// Get rolling average collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the <c>namePrefix</c> + caller method name.
        /// </summary>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <param name="suffix">Optional suffix to identify the calling point. </param>
        /// <returns>Return the rolling average collector.</returns>

        IStatsRollingAverageCollector GetAverageCollector(int flushPeriod, string suffix = "");

        /// <summary>
        /// Get execution time collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the underlying class name + caller method name (e.g. 'UserServiceImpl.AddUser').
        /// </summary>
        /// <remarks>Flush period is 1 min</remarks>
        /// <param name="suffix">Optional postfix to identify the calling point. </param>
        /// <returns>Return execution time collector.</returns>

        IStatsExecutionTimeHistogram GetExecutionTimeCollector(string suffix = "");
        
        /// <summary>
        /// Get execution time collector by caller name. Create a new if it doesn't exist.
        /// The caller name is the underlying class name + caller method name (e.g. 'UserServiceImpl.AddUser').
        /// </summary>
        /// <param name="suffix">Optional postfix to identify the calling point. </param>
        /// <param name="flushPeriod">Flush period of the histogram in ms.</param>
        /// <returns>Return execution time collector.</returns>

        IStatsExecutionTimeHistogram GetExecutionTimeCollector(int flushPeriod, string suffix = "");

        /// <summary>
        /// Get file extension if the path has a file name at the end.
        /// </summary>
        /// <param name="path">Path to get the file extension name of</param>
        /// <returns>Return file extension. Return empty string if no path or file name exists.</returns>

        string GetFileExtensionFromPath(string path);

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

        double[] GenerateBins(int start, int end, int interval);
    }
}
