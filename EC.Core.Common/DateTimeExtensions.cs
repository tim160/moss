using System;

namespace EC.Core.Common
{
    /// <summary>
    /// Contains extensions to the built-in .Net DateTime struct.
    /// </summary>
    static public class DateTimeExtensions
    {
        /// <summary>
        /// Returns null if the DateTime is DateTime.MinValue.
        /// </summary>
        /// <param name="dateTime">The date time or null.</param>
        /// <returns></returns>

        public static DateTime? ToNullIfMinValue(this DateTime dateTime)
        {
            return dateTime == DateTime.MinValue ? (DateTime?)null : dateTime;
        }
    }
}