using System;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    public class DateTimePair
    {
        public DateTimePair()
        {
            DateTime = SysTime = DateTime.UtcNow;
        }

        public DateTimePair(DateTime dateTime, DateTime sysTime)
        {
            DateTime = dateTime;
            SysTime = sysTime;
        }

        public DateTime DateTime { get; private set; }
        public DateTime SysTime { get; private set; }
    }
}
