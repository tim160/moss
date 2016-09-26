using System;
using EC.Common.Base;
using System.Threading;

namespace EC.Core.Common
{
    /// <summary>
    /// Provides the implementation for Design By Contract capabilities.
    /// </summary>
    
    public class DBC
    {
        /// <summary>
        /// If the condition is false, log an error in the Windows Event Log. In a
        /// DEBUG build, an assertion failure will also cause the server to shut
        /// down.
        /// </summary>
        /// <param name="condition">Boolean to check</param>
        /// <param name="message">Message for event log</param>
        /// <exception cref="AssertException">Throw exception if assert fails (condition == <c>false</c>)</exception>
        
        public static void Assert(bool condition, string message)
        {
            if (condition) return;
            string msg = String.Format("{0} - Assertion Failure: {1}", DebugUtils.GetLocationFromStack(2), message);
            WindowsEventLog.AddEvent(msg, WindowsEventLog.DBCAssertion);
            ShutdownIfNeededAndThrow(msg);
        }

        /// <summary>
        /// Check if the parameter <c>obj</c> is <c>null</c>. If <c>obj</c> is <c>null</c>, 
        /// write an error to the Windows Event Log. In a DEBUG build, an assertion failure 
        /// will also cause the server to shut down.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <param name="message">Message for the log if the parameter <c>obj</c> is <c>null</c>.</param>
        /// <exception cref="AssertException">Throw exception if object is <c>null</c></exception>
        
        public static void NonNull<T>(T obj, string message)
        {
            if (obj != null) return;
            string msg = String.Format("{0} - Null check failure: {1}", DebugUtils.GetLocationFromStack(2), message);
            WindowsEventLog.AddEvent(msg, WindowsEventLog.DBCNonNull);
            ShutdownIfNeededAndThrow(msg);
        }

        /// <summary>
        /// If in debug mode, shut the application down before throwing an assertion exception.
        /// Otherwise just throw an assertion exception.
        /// </summary>
        /// <remarks>
        /// IoCSetup.Shutdown() is asynchronous. That is, control may return from the shutdown
        /// call which in turn measns control will return from assert. Previously, we just 
        /// allowed a normal return in this situation with the result that code *after* the
        /// assert would continue to execute for a small time window (i.e. until the async
        /// shutdown completed). Allowing code to execute after an assert is bad, so now
        /// we always thrown an exception.
        /// </remarks>

        private static void ShutdownIfNeededAndThrow(string msg)
        {
            #if DEBUG
            IoCSetup.ShutdownAndExit("Shutting down - " + msg, 1);
            Thread.Sleep(1000);
            #endif

            throw new AssertException(msg);
        }
    }

    /// <summary>
    /// This exception is thrown if DBC.Assert() or DBC.NonNull() fails.
    /// </summary>

    public class AssertException : Exception
    {
        public AssertException(string msg) :base(msg)
        {
        }
    }
}