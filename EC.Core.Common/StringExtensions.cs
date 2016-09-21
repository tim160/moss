using System;

namespace EC.Core.Common
{
    /// <summary>
    /// Contains extensions to the built-in .Net string class.
    /// </summary>
    static public class StringExtensions
    {
        /// <summary>
        /// Performs a case insensitive (invariant culture) compare of 2 strings, with front and back whitespace trimmed.
        /// </summary>
        /// <param name="str">The string to compare.</param>
        /// <param name="other">The string to compare against str.</param>
        /// <returns>True if they are equal.</returns>
        public static bool CaseInsensitiveTrimmedCompare(this string str, string other)
        {
            return string.Compare(str.Trim(), other.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Replaces "smart quotes" with real quotes. Handles single and double quotes.
        /// </summary>

        public static string ReplaceSmartQuotes(this string str)
        {
            if (string.IsNullOrEmpty(str)) { return str; }

            return str
                .Replace('\u2018', '\'')
                .Replace('\u2019', '\'')
                .Replace('\u201c', '\"')
                .Replace('\u201d', '\"');
        }
    }
}