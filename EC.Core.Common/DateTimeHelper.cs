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


        #region String Dates
        /// <summary>
        /// MM/dd/yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateToString(DateTime? dt)
        {
            if (dt.HasValue)
            {
                string sDate = "";
                string Month = dt.Value.Month.ToString();
                string Day = dt.Value.Day.ToString();
                if (Month.Length == 1)
                    Month = "0" + Month;
                if (Day.Length == 1)
                    Day = "0" + Day;
                sDate = Month + "/" + Day + "/" + dt.Value.Year.ToString();
                return sDate;
            }
            return "N/A";
        }

        /// <summary>
        /// YYYYMMDD
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateToDataBaseString(DateTime dt)
        {
            string sDate = "";
            string Month = dt.Month.ToString();
            string Day = dt.Day.ToString();
            if (Month.Length == 1)
                Month = "0" + Month;
            if (Day.Length == 1)
                Day = "0" + Day;
            sDate = dt.Year.ToString() + Month + Day;
            return sDate;
        }

        /// <summary>
        /// YYYY/MM/DD
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateToDataBaseStringWithDashes(DateTime dt)
        {
            string sDate = "";
            string Month = dt.Month.ToString();
            string Day = dt.Day.ToString();
            if (Month.Length == 1)
                Month = "0" + Month;
            if (Day.Length == 1)
                Day = "0" + Day;
            sDate = dt.Year.ToString() + "/" + Month + "/" + Day;
            return sDate;
        }

        /// <summary>
        /// Feb dd, yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateToShortString(DateTime dt)
        {
            string sDate = "";
            string Month = GetShortMonth(dt.Month);
            string Day = dt.Day.ToString();
            if (Month.Length == 1)
                Month = "0" + Month;
            if (Day.Length == 1)
                Day = "0" + Day;
            //sDate = Month + " " + Day + ", " + dt.Year.ToString();
            sDate = Month + " " + Day + ", " + dt.Year.ToString();
            return sDate;
        }


        /// <summary>
        /// February 2, 2015
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateToLongMonthString(DateTime? dt)
        {
            if (dt.HasValue)
            {
                string sDate = "";
                string Month = GetFullMonth(dt.Value.Month);
                string Day = dt.Value.Day.ToString();
                if (Month.Length == 1)
                    Month = "0" + Month;
                if (Day.Length == 1)
                    Day = "0" + Day;
                //sDate = Month + " " + Day + ", " + dt.Year.ToString();
                sDate = Month + " " + Day + ", " + dt.Value.Year.ToString();
                return sDate;
            }
            return "N/A";
        }

        #endregion

        #region Months

        public Dictionary<int, string> FullMonth()
        {
            Dictionary<int, string> month = new Dictionary<int, string>();

            month.Add(1, "January");
            month.Add(2, "February");
            month.Add(3, "March");
            month.Add(4, "April");
            month.Add(5, "May");
            month.Add(6, "June");
            month.Add(7, "July");
            month.Add(8, "August");
            month.Add(9, "September");
            month.Add(10, "October");
            month.Add(11, "November");
            month.Add(12, "December");

            return month;
        }
        public Dictionary<int, string> ShortMonth()
        {
            Dictionary<int, string> month = new Dictionary<int, string>();

            month.Add(1, "Jan");
            month.Add(2, "Feb");
            month.Add(3, "Mar");
            month.Add(4, "Apr");
            month.Add(5, "May");
            month.Add(6, "Jun");
            month.Add(7, "Jul");
            month.Add(8, "Aug");
            month.Add(9, "Sep");
            month.Add(10, "Oct");
            month.Add(11, "Nov");
            month.Add(12, "Dec");

            return month;
        }

        /// <summary>
        /// returns February
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetFullMonth(int month)
        {
            string month_s = "";
            Dictionary<int, string> Monthes = FullMonth();
            if ((month > 0) && (month < 13))
                Monthes.TryGetValue(month, out month_s);

            return month_s;

        }
        public string GetShortMonth(int month)
        {
            string month_s = "";
            Dictionary<int, string> Monthes = ShortMonth();
            if ((month > 0) && (month < 13))
                Monthes.TryGetValue(month, out month_s);

            return month_s;

        }
        #endregion

    }
}
