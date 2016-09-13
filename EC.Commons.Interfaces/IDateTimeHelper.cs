using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Help class for different date time functions
    /// </summary>
    public interface IDateTimeHelper
    {
        /// <summary>
        /// Used to get the date time for week number
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        
        DateTime FirstDateOfWeek(int year, int weekOfYear);

        /// <summary>
        /// Get week number according to the <paramref name="date"/>.
        /// </summary>
        /// <param name="date">Date time to get the week number from.</param>
        /// <returns>Return week number</returns>

        int GetWeekOfYear(DateTime date);

        /// <summary>
        /// Will return the suffix for days of the week :IE st, nd, rd, th
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
       
        string SuffixForDay(DateTime date);

        /// <summary>
        /// Determines whether [is valid time] [the specified time string].
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <returns></returns>
        
        bool IsValidTime(string timeString);
        
        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <returns></returns>
       
        TimeSpan ParseTime(string timeString);
    }
}
