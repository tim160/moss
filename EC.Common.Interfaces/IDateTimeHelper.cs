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

        /// <summary>
        /// Will return the just Date MM/dd/yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string ConvertDateToString(DateTime? dt);

        /// <summary>
        /// YYYYMMDD
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string ConvertDateToDataBaseString(DateTime dt);

        /// <summary>
        /// YYYY/MM/DD
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string ConvertDateToDataBaseStringWithDashes(DateTime dt);

        /// <summary>
        /// Feb dd, yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string ConvertDateToShortString(DateTime date);

        /// <summary>
        /// February 2, 2015
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string ConvertDateToLongMonthString(DateTime? date);

        /// <summary>
        /// All Month names
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        Dictionary<int, string> FullMonth();

        /// <summary>
        /// Jan,Feb, Mar
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        Dictionary<int, string> ShortMonth();


        /// <summary>
        /// returns February
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string GetFullMonth(int month);

        /// <summary>
        /// Feb
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        string GetShortMonth(int month);
    }
}
