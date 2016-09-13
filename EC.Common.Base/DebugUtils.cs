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
    }
}
