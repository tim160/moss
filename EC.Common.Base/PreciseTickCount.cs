using System.Reflection;
using System.Runtime.InteropServices;

namespace EC.Common.Base
{
    /// <summary>
    /// This class provides methods to retrieve the high-resolution performance counter
    /// from Windows (precision is up to 3 micro seconds).
    /// </summary>
    /// <remarks>
    /// The C# Environment.TickCount resolution is milliseconds. The DateTime.Ticks
    /// resolution is nanoseconds but the smallest interval between the ticks is
    /// actually about 15 ms.
    /// <para>It relies on the Kernel32.dll for the actual query.</para>
    /// </remarks>
   
    public static class PreciseTickCount
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        /// Static constructor. It initializes the static sFrequency data member.
        /// </summary>
      
        static PreciseTickCount()
        {
            // initialize the static sFrequency
            if (!QueryPerformanceFrequency(out s_Frequency))
            {
                string msg = string.Format("PreciseTickCount::PreciseTickCount(): Error in calling QueryPerformanceFrequency");
                WindowsEventLog.AddEvent(msg, WindowsEventLog.TickCountError);
            }
        }

        /// <summary>
        /// Retrieve the current tick count.
        /// </summary>
        /// <param name="tick">Out param represents the current tick count.</param>
        /// <remarks>The tick count is a 64 bit integer, it's big enough not
        /// to worry about loop over.</remarks>
        
        public static void GetPreciseTickCount(out long tick)
        {
            if (!QueryPerformanceCounter(out tick))
            {
                tick = 0;
                string msg = string.Format("PreciseTickCount::GetPreciseTickCount(): Error in calling QueryPerformanceCounter");
                WindowsEventLog.AddEvent(msg, WindowsEventLog.TickCountError);
            }
        }

        public static double ToSeconds(long after, long before)
        {
            return ToSeconds(after - before);
        }

        public static double ToSeconds(long delta)
        {
            return delta/(double) Frequency;
        }

        public static double ToMilliSeconds(long after, long before)
        {
            return ToMilliSeconds(after - before);
        }

        public static double ToMilliSeconds(long delta)
        {
            return ToSeconds(delta)*1000.0d;
        }

        /// <summary>Retrieve the frequency of the high-resolution performance counter.</summary>
        /// <value>Return the static sFrequency data member.</value>
        
        public static long Frequency
        {
            get { return s_Frequency; }
        }

        private static readonly long s_Frequency; // the frequency of the performance counter
    }
}