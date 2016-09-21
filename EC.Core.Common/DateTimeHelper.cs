using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Common.Interfaces;
using System.Text.RegularExpressions;

namespace EC.Core.Common
{
    [TransientType]
    [RegisterAsType(typeof(IDateTimeHelper))]
    
    public class DateTimeHelper: IDateTimeHelper
    {
        public DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public int GetWeekOfYear(DateTime date)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public string SuffixForDay(DateTime date)
        {
            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        /// <summary>
        /// Determines whether [is valid time] [the specified time string].
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <returns></returns>
        public bool IsValidTime(string timeString)
        {
            Regex checktime = new Regex(@"^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            return checktime.IsMatch(timeString);
        }

        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <returns></returns>
        public TimeSpan ParseTime(string timeString)
        {
            string[] timeAry = timeString.Split(':');
            int hour = Convert.ToInt32(timeAry[0]);
            int min = Convert.ToInt32(timeAry[1]);            
            TimeSpan newTime = new TimeSpan(hour, min, 0);
            return newTime;
        }
    }
}
