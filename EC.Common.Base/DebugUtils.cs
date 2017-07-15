using System;
using System.Diagnostics;
using System.Linq;

namespace EC.Common.Base
{
    public class DebugUtils
    {
        public static string GetLocationFromStack(int upFrames)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames.Length <= upFrames) return "Unknown Location";
            var caller = stackFrames[upFrames];
            return String.Format("Assembly({0}) Class({1}) Method({2})", 
                caller.GetMethod().Module.FullyQualifiedName, 
                caller.GetMethod().DeclaringType.Name, 
                caller.GetMethod().Name);
        }  
     
        public static string GetStackTrace()
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            return stackFrames.Select(f => String.Format("{0} {1}", ClassFromFrame(f), MethodFromFrame(f))).Aggregate((a, n) => a + "\n" + n);
        }

        private static string ClassFromFrame(StackFrame f)
        {
            if (f == null) { return "NullStackFrame"; }
            if (f.GetMethod() == null) { return "UknownMethod"; }
            if (f.GetMethod().DeclaringType == null) { return "NoDeclaringType"; }
            return f.GetMethod().DeclaringType.Name;
        }

        private static string MethodFromFrame(StackFrame f)
        {
            if (f == null) { return "NullStackFrame"; }
            if (f.GetMethod() == null) { return "UknownMethod"; }
            return f.GetMethod().Name;
        }

        public static void PerfomanceEstimate(string name, Action action)
        {
            var now = DateTime.Now;

            action();

            System.Diagnostics.Debug.WriteLine("Name: {0} - {1} msec\r\nStarted: {2} Stoped {3}", name, (DateTime.Now - now).TotalMilliseconds, now, DateTime.Now);
        }

        public static T PerfomanceEstimate<T>(string name, Func<T> action)
        {
            PerfomanceEstimateCounter++;
            var now = DateTime.Now;

            var r = action();

            System.Diagnostics.Debug.WriteLine("Name: {0} - {1} msec\r\nStarted: {2} Stoped {3}\r\nPerfomanceEstimateCounter: {4}", name, (DateTime.Now - now).TotalMilliseconds, now, DateTime.Now, PerfomanceEstimateCounter);

            return r;
        }

        private static int PerfomanceEstimateCounter = 0;
        public static void PerfomanceEstimateCounterReset()
        {
            PerfomanceEstimateCounter = 0;
        }
    }
}
