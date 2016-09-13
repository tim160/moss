using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace EC.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public static class NotificationChannelConstants
    {
        public const string DefaultUserContactProperty = "ContactEmailOrDefault";
    }
    /// <summary>
    /// Names of all different channels.
    /// </summary>

    public static class NotificationChannelNames
    {
        /// <summary>
        /// 'Email' channel sends an email as soon as possible.
        /// </summary>

        public const string EMAIL = "Email";

        //public const string DAILY_EMAIL = "DailyEmail";

        //public const string WEEKLY_EMAIL = "WeeklyEmail";

        //public const string MONTHLY_EMAIL = "MonthlyEmail";

        //public const string TWITTER = "Twitter";


        /// <summary>
        /// Get all channel names as a list.
        /// </summary>
        /// <returns>All channel names.</returns>

        public static IEnumerable<string> All()
        {
            IList<string> c = new List<string>();

            FieldInfo[] fieldInfos = typeof(NotificationChannelNames).GetFields();

            foreach (FieldInfo fi in fieldInfos)
            {
                string channelName = fi.GetRawConstantValue() as string; // Get value of the field...
                c.Add(channelName);
            }

            // Return the enumerable of channels...
            return c.AsEnumerable<string>();
        }

        /// <summary>
        /// Check if the specified <paramref name="channelName"/> exists.
        /// Compares case-sensitive against all channel names.
        /// </summary>
        /// <param name="channelName">Channel name to check it exists</param>
        /// <returns>Return <c>true</c> if <paramref name="channelName"/> exists. Return <c>false</c> if the <paramref name="channelName"/> doesn't exist.</returns>

        public static bool ChannelExists(string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName)) { return false; }
            return All().Contains(channelName);
        }
    }
}
