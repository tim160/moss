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
        /// Create content URL for a specific path.
        /// </summary>
        /// <param name="dnsName">DNS name for the URL</param>
        /// <param name="isSSL">Flag whether to use HTTPS or HTTP</param>
        /// <param name="path">content path</param>
        /// <returns>Return full URL (http(s)://.....) for the content <paramref name="path"/>.</returns>

        public static string CreateContentUrl(string dnsName, bool isSSL, string path) 
        {
            path = PathHelper.FixPath(path, true);
            string url = string.Format("http{0}://{1}/Cnt{2}", (isSSL) ? "s" : string.Empty, dnsName, path);
            return url;
        }

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

            var splitUrl = url.SplitStringBy(new char[] { '/' }, true);

            return splitUrl.First();
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
