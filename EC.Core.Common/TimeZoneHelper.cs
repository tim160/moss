using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.MicroKernel;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;
using EC.Constants;

namespace EC.Core.Common
{
    [TransientType]
    public class TimeZoneHelper : ITimeZoneHelper
    {
        /// <summary>
        /// Convert the UTC date time into the specified <paramref name="targetTimeZone"/>.
        /// </summary>
        /// <param name="utcDateTime">Date time in UTC</param>
        /// <param name="targetTimeZone">Time zone to convert to.</param>
        /// <returns>Converted DateTime object.</returns>

        public DateTime ConvertUtcToSpecificDateTime(DateTime utcDateTime, TimeZoneInfo targetTimeZone)
        {
            if (targetTimeZone == null) { return utcDateTime; }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, targetTimeZone);
        }
        
        /// <summary>
        /// Convert the UTC date time into the specified <paramref name="targetTimeZone"/>.
        /// </summary>
        /// <param name="utcDateTime">Date time in UTC</param>
        /// <param name="targetTimeZone">Time zone to convert to.</param>
        /// <returns>Converted DateTime object.</returns>

        public DateTime? ConvertUtcToSpecificDateTime(DateTime? utcDateTime, TimeZoneInfo targetTimeZone)
        {
            if (!utcDateTime.HasValue) { return null; }
            if (targetTimeZone == null) { return utcDateTime; }
            return this.ConvertUtcToSpecificDateTime(utcDateTime.Value, targetTimeZone);
        }

        /// <summary>
        /// Convert date time from UTC to user context time zone (use time zone from <c>IRequestContext</c>).
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> mark <paramref name="userDateTime"/> as UTC.
        /// </remarks>
        /// <param name="utcDateTime">Time as UTC</param>
        /// <returns>Return <paramref name="utcDateTime"/> if it is already in UTC. Return the converted DateTime.</returns>

        public DateTime ConvertUtcToUserDateTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Local) { return utcDateTime; }
            var requestContextMgr = this.Kernel.Resolve<IManageRequestContexts>();
            if (!requestContextMgr.IsContextInitialized()) { return utcDateTime; }
            IRequestContext rContext = requestContextMgr.GetContext();
            string destinationTzString = rContext.UserTimeZone;
                
            if (string.IsNullOrWhiteSpace(destinationTzString))
            {
                Logger.WarnFormat("ConvertUtcToUserDateTime - No time zone specified for [{0}]. Take UTC as user DateTime.", rContext.DisplayName);
                return utcDateTime;
            }

            var destinationTz = TimeZoneInfo.FindSystemTimeZoneById(destinationTzString);
            var result = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, destinationTz);
            result = DateTime.SpecifyKind(result, DateTimeKind.Local);
            return result;
            
        }
        
        /// <summary>
        /// Convert date time from user to UTC time zone (use time zone from <c>IRequestContext</c>)
        /// </summary>
        /// <remarks>
        /// If the user time zone is <c>null</c> mark <paramref name="userDateTime"/> as UTC.
        /// </remarks>
        /// <param name="userDateTime">DateTime other than Utc.</param>
        /// <returns>Return UTC DateTime.</returns>

        public DateTime ConvertUserDateTimeToUtc(DateTime userDateTime)
        {
            if (userDateTime.Kind == DateTimeKind.Utc)
            {
                // If already UTC...
                return userDateTime;
            }

            var getRequestContext = this.Kernel.Resolve<IManageRequestContexts>();
            var rContext = getRequestContext.GetContext();
            string sourceTzString = rContext.UserTimeZone;

            if (string.IsNullOrWhiteSpace(sourceTzString))
            {
                Logger.WarnFormat("ConvertUserDateTimeToUtc - No time zone specified for [{0}]. Take user DateTime as UTC.", rContext.DisplayName);
                return DateTime.SpecifyKind(userDateTime, DateTimeKind.Utc);
            }

            var sourceTtz = TimeZoneInfo.FindSystemTimeZoneById(sourceTzString);
            var result = TimeZoneInfo.ConvertTimeToUtc(userDateTime, sourceTtz);
            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);

            return result;
        }

        /// <summary>
        /// Get time zone info, and if there is a problem getting it, return the time zone
        /// info for UTC (Greenwich Standard Time).
        /// </summary>
        /// <param name="timeZoneId">Windows time zone Id string</param>
        /// <returns>time zone info. Return <c>null</c> if <paramref name="timeZoneId"/> is <c>null</c> or empty</returns>

        public TimeZoneInfo GetTimeZone(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId)) { return null; }
            try
            {    
                var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return tzInfo;
            }
            catch (Exception ex)
            {
                Logger.Warn("GetTimeZone - unexpected exception", ex);
                return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneConstants.UTC_TIME_ZONE_ID);
            }
        }

        /// <summary>
        /// Get the time zone display name.
        /// If the <paramref name="timeZone"/> is <c>null</c> return the default time zone "GMT Standard Time".
        /// </summary>
        /// <param name="timeZone">Time zone to get display name from. Can be <c>null</c> to get the default value</param>
        /// <returns>Return the display name of the time zone. Return default time zone if <paramref name="timeZone"/> is <c>null</c></returns>

        public string GetTimeZoneDisplayNameOrDefault(TimeZoneInfo timeZone)
        {
            string timeZoneName = null;
            if (timeZone != null)
            {
                timeZoneName = timeZone.DisplayName;
            }
            else
            {
                timeZoneName =  TimeZoneConstants.DEFAULT_TIME_ZONE_DISPLAY_NAME;
            }
            return timeZoneName;
        }

         /// <summary>
        /// Convert date time from UTC to user context time zone (use time zone from <c>IRequestContext</c>).
        /// </summary>
        /// <param name="utcDateTime">Time as UTC</param>
        /// <returns>Return <paramref name="utcDateTime"/> if it is already in UTC. Return the converted DateTime.</returns>
        /// <exception cref="Exception">If the user time zone has not been set for the <c>IRequestContext</c></exception>

        public DateTime? ConvertUtcToUserDateTime(DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue) { return null; }
            return this.ConvertUtcToUserDateTime(utcDateTime.Value);
        }
        
        /// <summary>
        /// Convert date time from user context time zone to UTC date time (use user context time zone from <c>IRequestContext</c>)
        /// </summary>
        /// <param name="userDateTime">DateTime other than Utc.</param>
        /// <returns>Return UTC DateTime. Return <c>null</c> if parameter doesn't have any value.</returns>
        /// <exception cref="Exception">If the user time zone has not been set for the <c>IRequestContext</c></exception>

        public DateTime? ConvertUserDateTimeToUtc(DateTime? userDateTime)
        {
            if (!userDateTime.HasValue) { return null; }
            return this.ConvertUserDateTimeToUtc(userDateTime.Value);
        }

        /// <summary>
        /// Validate whether the <paramref name="timeZone"/> is a valid .NET time zone Id string.
        /// </summary>
        /// <param name="timeZone">.NET time zone Id string to check</param>
        /// <param name="throwExceptionOnError">Optional (default <c>false</c>): Flag whether to throw an exception or just return <c>false</c> instead.</param>
        /// <returns>Return <c>true</c> if the <paramref name="timeZone"/> is valid. Return <c>false</c> if unknown time zone AND 
        /// <paramref name="throwExceptionOnError"/> is set <c>false</c>).</returns>

        public bool ValidateTimeZone(string timeZone, bool throwExceptionOnError = false) 
        {
            try
            {
                var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return true;
            }
            catch (Exception ex)
            {
                if (throwExceptionOnError) 
                { 
                    throw ex; 
                }
                else { return false; }
            }
        }

        /// <summary>
        /// IoC entry point.
        /// </summary>

        public TimeZoneHelper(IKernel kernel, ILogger logger)
        {
            this.Kernel = kernel;
            this.Logger = logger;
        }

        private IKernel Kernel { get; set; }
        private ILogger Logger { get; set; }


    }
}
