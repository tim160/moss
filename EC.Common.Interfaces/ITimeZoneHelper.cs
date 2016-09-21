using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Helper to convert between Utc and user date time zones.
    /// </summary>

    public interface ITimeZoneHelper
    {
        /// <summary>
        /// Convert the UTC date time into the specified <paramref name="targetTimeZone"/>.
        /// </summary>
        /// <param name="utcDateTime">Date time in UTC</param>
        /// <param name="targetTimeZone">Time zone to convert to. Set to <c>null</c> to return the original time).</param>
        /// <returns>Converted DateTime object. Return <paramref name="utcDateTime"/> if <paramref name="targetTimeZone"/> is <c>null</c></returns>

        DateTime ConvertUtcToSpecificDateTime(DateTime utcDateTime, TimeZoneInfo targetTimeZone);

        /// <summary>
        /// Convert the UTC date time into the specified <paramref name="targetTimeZone"/>.
        /// </summary>
        /// <param name="utcDateTime">Date time in UTC</param>
        /// <param name="targetTimeZone">Time zone to convert to. Set to <c>null</c> to return the original time).</param>
        /// <returns>Converted DateTime object. Return <paramref name="utcDateTime"/> if <paramref name="targetTimeZone"/> is <c>null</c></returns>

        DateTime? ConvertUtcToSpecificDateTime(DateTime? utcDateTime, TimeZoneInfo targetTimeZone);

        /// <summary>
        /// Convert date time from UTC to user context time zone (use user context time zone from <c>IRequestContext</c>).
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> return the <paramref name="utcDateTime"/>.
        /// </remarks>
        /// <param name="utcDateTime">Time as UTC</param>
        /// <returns>Return <paramref name="utcDateTime"/> if it is already in UTC. Return the converted DateTime.</returns>
        
        DateTime ConvertUtcToUserDateTime(DateTime utcDateTime);

        /// <summary>
        /// Convert date time from user context time zone to UTC date time (use user context time zone from <c>IRequestContext</c>)
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> mark <paramref name="userDateTime"/> as UTC.
        /// </remarks>
        /// <param name="userDateTime">DateTime other than Utc.</param>
        /// <returns>Return UTC DateTime.</returns>
        
        DateTime ConvertUserDateTimeToUtc(DateTime userDateTime);

        /// <summary>
        /// Convert date time from UTC to user context time zone (use user context time zone from <c>IRequestContext</c>).
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> just return the <paramref name="utcDateTime"/>.
        /// </remarks>
        /// <param name="utcDateTime">Time as UTC</param>
        /// <returns>Return <paramref name="utcDateTime"/> if it is already in UTC. Return the converted DateTime.  Return <c>null</c> if parameter doesn't have any value.</returns>

        DateTime? ConvertUtcToUserDateTime(DateTime? utcDateTime);
 
        /// <summary>
        /// Convert date time from user context time zone to UTC date time (use user context time zone from <c>IRequestContext</c>)
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> mark <paramref name="userDateTime"/> as UTC.
        /// </remarks>
        /// <param name="userDateTime">DateTime other than Utc.</param>
        /// <returns>Return UTC DateTime. Return <c>null</c> if parameter doesn't have any value.</returns>
        
        DateTime? ConvertUserDateTimeToUtc(DateTime? userDateTime);

        /// <summary>
        /// Get time zone info, and if there is a problem getting it, return the time zone
        /// info for UTC (Greenwich Standard Time).
        /// </summary>
        /// <param name="timeZoneId">Windows time zone Id string</param>
        /// <returns>time zone info</returns>

        TimeZoneInfo GetTimeZone(string timeZoneId);

        /// <summary>
        /// Get the time zone display name.
        /// If the <paramref name="timeZone"/> is <c>null</c> return the default time zone "GMT Standard Time".
        /// </summary>
        /// <param name="timeZone">Time zone to get display name from. Can be <c>null</c> to get the default value</param>
        /// <returns>Return the display name of the time zone. Return default time zone if <paramref name="timeZone"/> is <c>null</c></returns>

        string GetTimeZoneDisplayNameOrDefault(TimeZoneInfo timeZone);

        /// <summary>
        /// Validate whether the <paramref name="timeZone"/> is a valid .NET time zone Id string.
        /// </summary>
        /// <param name="timeZone">.NET time zone Id string to check</param>
        /// <param name="throwExceptionOnError">Optional (default <c>false</c>): Flag whether to throw an exception or just return <c>false</c> instead.</param>
        /// <returns>Return <c>true</c> if the <paramref name="timeZone"/> is valid. Return <c>false</c> if unknown time zone AND 
        /// <paramref name="throwExceptionOnError"/> is set <c>false</c>).</returns>

        bool ValidateTimeZone(string timeZone, bool throwExceptionOnError = false);
    }
}
