using System;
using System.Diagnostics;

namespace EC.Common.Base
{
    /// <summary>
    /// Utility class for writing to Windows Event Log.
    /// </summary>
    /// <remarks>
    /// NOTE: This class should only be used in those circumstances in which logging via
    /// the normal logging infrastructure is not possible. In most cases, this will 
    /// only be in the EC.Common.Base namespace, since that module is at the 
    /// lowest level in the system.
    /// </remarks>
    
    public class WindowsEventLog
    {
        /// <summary>
        /// Add an entry to the Windows Event Log in the Application category.
        /// </summary>
        /// <param name="message">Message to displayed</param>
        /// <param name="code">Error code to be displayed</param>
        
        public static void AddEvent(string message, int code)
        {
            string source = "EC";
            string log = "Application";

            try
            {
                if (!EventLog.SourceExists(source)) { EventLog.CreateEventSource(source, log); }
                EventLog.WriteEntry(source, message, ConvertEventCodeToEventLogType(code), code);
            }
            catch (Exception)
            {
                // We have no place to log errors here, so we're just going to swallow them.
            }
        }

        /// <summary>
        /// Convert the error code passed into AddEvent() to a windows event log entry type.
        /// </summary>
        /// <param name="code">the error code</param>
        /// <returns>the corresponding type for the windows event log entry</returns>
        
        private static EventLogEntryType ConvertEventCodeToEventLogType(int code)
        {
            switch (code)
            {
                case 1: return EventLogEntryType.Error;
                case 2: return EventLogEntryType.Error;
                case 3: return EventLogEntryType.Error;
                case 4: return EventLogEntryType.Warning;
                case 5: return EventLogEntryType.Error;
                case 6: return EventLogEntryType.Error;
                case 7: return EventLogEntryType.Information;
                case 8: return EventLogEntryType.Error;
                case 9999: return EventLogEntryType.Information;
                default: return EventLogEntryType.Information;
            }
        }

        /// <summary>
        /// Different error codes that can be passed to AddEvent().
        /// </summary>
        
        public const int DBCAssertion      = 1;
        public const int DBCNonNull        = 2;
        public const int LockingError      = 3;
        public const int TickCountError    = 4;
        public const int ServiceError      = 5;
        public const int FatalError        = 6;
        public const int Info              = 7;
        public const int RTAInstallerError = 8;
        public const int Debug             = 9999;
    }


}
