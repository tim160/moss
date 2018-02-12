using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Base
{
    /// <summary>
    /// Helper class to create URLs.
    /// </summary>

    public static class UrlPathHelper
    {
        /// <summary>
        /// Get domain name from a url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        public static string GetDomainFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return null; }

            if (url.StartsWith("http://"))
            {
                url = url.Substring("http://".Count());
            }
            if (url.StartsWith("https://"))
            {
                url = url.Substring("https://".Count());
            }

            var splitUrl = SplitStringBy(url, new char[] { '/' }, true);

            return splitUrl.First();
        }

        /// <summary>
        /// Split string by <paramref name="splityBy"/> and trim each element.
        /// </summary>
        /// <param name="valueToSplit">string value to be split</param>
        /// <param name="splitBy">Characters to split the string by.</param>
        /// <param name="removeEmptyEntries">Flag whether to remove empty entries after the split.</param>
        /// <returns>
        /// Return list with split values.
        /// Return empty list if <paramref name="valueToSplit"/> is <c>null</c> or empty. 
        /// Return list with <paramref name="valueToSplit"/> as entry if <paramref name="splitBy"/> is <c>null</c> or empty.
        /// </returns>

        private static IList<string> SplitStringBy(string valueToSplit, char[] splitBy, bool removeEmptyEntries = true)
        {
            if (string.IsNullOrEmpty(valueToSplit)) { return new List<string>(); }
            if ((splitBy == null) || (splitBy.Length == 0)) { return new List<string>() { valueToSplit }; }

            if (removeEmptyEntries)
            {
                return valueToSplit.Split(splitBy, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList<string>();
            }
            else
            {
                return valueToSplit.Split(splitBy).Select(s => s.Trim()).ToList<string>();
            }
        }



        /// <summary>
        /// Check whether the URL is secure (https) or not (http).
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// Return <c>true</c> if url starts with 'https'. 
        /// Return <c>false</c> if prefix 'http' exists, no url (null or empty) or no prefix exists.
        /// </returns>

        public static bool IsSecureUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return false; }

            if (url.StartsWith("http://"))
            {
                return false;
            }
            if (url.StartsWith("https://"))
            {
                return true;
            }
            return false;
        }
    }
}
