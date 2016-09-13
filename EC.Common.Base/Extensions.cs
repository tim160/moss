using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EC.Common.Base
{
    public static class Extensions
    {
        /// <summary>
        /// Remove list of items from a HashSet
        /// </summary>
        /// <typeparam name="T">Type of HashSet</typeparam>
        /// <param name="source">HashSet</param>
        /// <param name="toBeRemoved">List of items to be removed from the HashSet</param>

        public static void Remove<T>(this HashSet<T> source, IList<T> toBeRemoved)
        {
            toBeRemoved.ForEach(r => source.Remove(r));
        }

        /// <summary>
        /// Randomize the order of a set
        /// </summary>
        /// <param name="source">The list to randomize</param>
        /// <returns>Return the randomized set</returns>

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Random rng = new Random();
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        /// <summary>
        /// Call <paramref name="action"/> an pass in every item in <paramref name="source"/> as a parameter.
        /// </summary>
        /// <typeparam name="T">Type of source</typeparam>
        /// <param name="source">Enumerable of T objects</param>
        /// <param name="action">Action to execute and takes one parameter of type T</param>

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// Check if the string is in the collection. Compare every string with the specified <c>StringComparison</c>
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <param name="source">A string collection</param>
        /// <param name="toCheck">string to check</param>
        /// <param name="compare">Comparer</param>
        /// <returns>Return <c>true</c> if the string exists in the collection. Return <c>false</c> otherwise.</returns>

        public static bool Contains(this ICollection<string> source, string toCheck, StringComparison compare)
        {
            bool doesContain = false;
            source.ForEach<string>(u =>
            {
                if (u.Equals(toCheck, compare))
                {
                    doesContain = true;
                    return;
                }
            });

            return doesContain;
        }

        /// <summary>
        /// Check whether <paramref name="toCheck"/> is present in <paramref name="source"/> using a <paramref name="comparer"/> (i.e. case-insensitive).
        /// </summary>
        /// <param name="source">String to check against</param>
        /// <param name="toCheck">String to check</param>
        /// <param name="comparer">String comparison type (i.e. case-insensitive)</param>
        /// <returns>
        /// Return <c>true</c> if <paramref name="source"/> == <paramref name="toCheck"/> or <paramref name="toCheck"/> is in <paramref name="source"/>
        /// or both are <c>null</c>.
        /// Return <c>false</c> if <paramref name="toCheck"/> is <c>null</c> and <paramref name="source"/> is not <c>null</c>.
        /// Return <c>false</c> if <paramref name="source"/> doesn't contain <paramref name="toCheck"/>.
        /// </returns>

        public static bool Contains(this string source, string toCheck, StringComparison comparer)
        {
            if (source == null && toCheck == null) { return true; }
            if (source == null) { return false; }
            return source.IndexOf(toCheck, comparer) >= 0;
        }

        /// <summary>
        /// Compare a <c>string</c> if it is equal ignoring case sensitivity.
        /// </summary>
        /// <param name="toCheck"><c>string</c> to compare to this instance.</param>
        /// <returns>Return <c>true</c> if <c>source</c> is equal (case ignored). Return <c>false</c> if the <c>source</c> is not equal.</returns>
        /// <exception cref="ArgumentNullException">If parameter is <c>null</c>.</exception>

        public static bool EqualsCaseInsensitive(this string source, string toCheck)
        {
            if (toCheck == null)
            {
                throw new ArgumentNullException("toCheck", "Parameter must not be 'null'.");
            }
            return source.Equals(toCheck, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Replace a <paramref name="oldValue"/> with a <paramref name="newValue"/> within a <paramref name="str"/>. 
        /// The <paramref name="comparison"/> is used to find the <paramref name="oldValue"/> - e.g. if the capitalization 
        /// should be ignored.
        /// </summary>
        /// <param name="str">String to work with.</param>
        /// <param name="oldValue">String to replace</param>
        /// <param name="newValue">String to use for replacement</param>
        /// <param name="comparison">Comparison type to look for <paramref name="oldValue"/></param>
        /// <returns>Return the new string with <paramref name="newValue"/>.</returns>

        public static string ReplaceString(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// Replace all back slashes '\' with forward slashes '/' in a string.
        /// </summary>
        /// <param name="str">string to replace back slashes in</param>
        /// <returns>Return string with all forward slashes ('/') instead of back slashes ('\'). Return <c>null</c> if <paramref name="str"/> is <c>null</c></returns>

        public static string ReplaceBackWithForwardSlashes(this string str)
        {
            if (str == null) { return null; }
            return str.Replace('\\', '/');
        }
        
        /// <summary>
        /// Parse a string into an int.
        /// </summary>
        /// <param name="value">String value to parse into an int.</param>
        /// <returns>Returns the parsed int. Return <c>null</c> if the value couldn't be parsed.</returns>

        public static int? ParseToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            int parsedValue = 0;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue)) { return null; }
            return parsedValue;
        }

        /// <summary>
        /// Parse a string into a long.
        /// </summary>
        /// <param name="value">String value to parse into a long</param>
        /// <returns>Return parsed long. Return <c>null</c> if the value couldn't be parsed.</returns>

        public static long? ParseToLong(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            long parsedValue = 0;
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue)) { return null; }
            return parsedValue;
        }

        /// <summary>
        /// Parse a string into a Guid.
        /// </summary>
        /// <param name="value">String value to parse</param>
        /// <returns>Returns parsed Guid. Return <c>null</c> if value could not be parsed</returns>

        public static Guid? ParseToGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var parsedValue = Guid.Empty;

            if (!Guid.TryParse(value, out parsedValue))
            {
                return null;
            }

            return parsedValue;
        }

        /// <summary>
        /// Convert an enumeration into another type of enumeration (based on the name, not value of the enum).
        /// </summary>
        /// <typeparam name="T">Target enum type</typeparam>
        /// <param name="value"></param>
        /// <returns>Returns the converted enumeration of type <c>T</c>.</returns>
        /// <exception cref="ArgumentException">If the destination type is not an enum.</exception>

        public static T ConvertEnumTo<T>(this Enum value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("Destination type is not enum"); }
            return (T)Enum.Parse(typeof(T), value.ToString());
        }

        /// <summary>
        /// Parse a string into an enumeration of type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">enum type to parse the value into.</typeparam>
        /// <param name="value">String value to parse into an enum.</param>
        /// <returns>Returns the parsed enumeration of type <c>T</c>.</returns>
        /// <exception cref="ArgumentException">If the destination type is not an enum.</exception>
        /// <exception cref="Exception">If any other error.</exception>

        public static T ParseToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("Destination type must be of type enum."); }
            if (string.IsNullOrWhiteSpace(value)) { throw new Exception("The value must not be empty, null or only white spaces."); }

            // Note: Enum parses the string into an enum without throwing an exception even if the value doesn't exist.
            //       Therefore Enum.IsDefined must be used to check the value!
            var result = (T)Enum.Parse(typeof(T), value, true);
            if (Enum.IsDefined(typeof(T), result))
            {
                return result;
            }
            else
            {
                throw new Exception(string.Format("String value '{0}' is not part of the enumeration '{1}'.", value, typeof(T).FullName));
            }
        }

        /// <summary>
        /// Parse a string into an enumeration of type <c>T</c>. If not pars able (wrong string value), return <c>null</c>.
        /// </summary>
        /// <typeparam name="T">enum type to parse the value into.</typeparam>
        /// <param name="value">String value to parse into an enum.</param>
        /// <returns>Returns the parsed enumeration of type <c>T</c>. Return <c>null</c> if the <paramref name="value"/> is not parse able</returns>
        /// <exception cref="ArgumentException">If the destination type is not an enum.</exception>

        public static T? ParseToEnumOrNull<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("Destination type must be of type enum."); }
            if (string.IsNullOrWhiteSpace(value)) { return null; }

            // Note: Enum parses the string into an enum without throwing an exception even if the value doesn't exist.
            //       Therefore Enum.IsDefined must be used to check the value!
            var result = (T)Enum.Parse(typeof(T), value, true);
            if (Enum.IsDefined(typeof(T), result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the description attribute from enum value. we use this attribute to store property column that we sorted by
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="enumerationValue">Enum value to get the description from</param>
        /// <returns>Return the enum description. Return the <paramref name="enumerationValue"/> if no description has been found</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>

        public static string GetEnumDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum) { throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue"); }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }



        /// <summary>
        /// Removes all leading or trailing white spaces of a string.
        /// If the string is already empty, only consists of white spaces or <c>null</c> - return <c>null</c>.
        /// <remarks>
        /// This extends the normal Trim() function to not throw a null-exception.
        /// </remarks>
        /// </summary>
        /// <param name="str">String to trim</param>
        /// <returns>Return trimmed string or <c>null</c> if already <c>null</c>, empty or only white spaces.</returns>

        public static string TrimOrDefault(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            else
            {
                return str.Trim();
            }
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

        public static IList<string> SplitStringBy(this string valueToSplit, char[] splitBy, bool removeEmptyEntries = true)
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
        /// Split string by <paramref name="splitBy"/> and trim each element.
        /// </summary>
        /// <param name="valueToSplit">string value to be split</param>
        /// <param name="splitBy">Characters to split the string by.</param>
        /// <param name="removeEmptyEntries">Flag whether to remove empty entries after the split.</param>
        /// <returns>
        /// Return list with split values.
        /// Return empty list if <paramref name="valueToSplit"/> is <c>null</c> or empty. 
        /// Return list with <paramref name="valueToSplit"/> as entry if <paramref name="splitBy"/> is <c>null</c> or empty.
        /// </returns>

        public static IList<string> SplitStringBy(this string valueToSplit, string splitBy, bool removeEmptyEntries = true)
        {
            if (string.IsNullOrEmpty(valueToSplit)) { return new List<string>(); }
            if ((splitBy == null) || (splitBy.Length == 0)) { return new List<string>() { valueToSplit }; }

            var splitEntries = Regex.Split(valueToSplit, splitBy).Select(s => s.Trim());

            if (removeEmptyEntries)
            {
                splitEntries = splitEntries.Where(s => !string.IsNullOrEmpty(s));
            }
            return splitEntries.ToList();
        }

        /// <summary>
        /// Create a memory stream from <paramref name="str"/>.
        /// </summary>
        /// <param name="str">String to convert into a stream</param>
        /// <returns>Return memory stream.</returns>

        public static Stream ToStream(this string str)
        {
            MemoryStream mStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(mStream);
            writer.Write(str);
            writer.Flush();
            mStream.Position = 0;
            return mStream;
        }

        /// <summary>
        /// Get value from a dictionary by <paramref name="key"/>. The value is nullable.
        /// </summary>
        /// <remarks>
        /// The Value of the dictionary must be nullable. 
        /// </remarks>
        /// <typeparam name="K">Type of the dictionary key</typeparam>
        /// <typeparam name="V">Type (nullable) of the dictionary value</typeparam>
        /// <param name="dict">Dictionary to get the value from</param>
        /// <param name="key">Dictionary key at which to get the value from</param>
        /// <returns>
        /// Return the value if it exists. 
        /// Return <c>null</c> if the <paramref name="key"/> doesn't exist or the value itself is <c>null</c>.
        /// </returns>

        public static V? GetValueOrNull<K, V>(this IDictionary<K, V> dict, K key) where V : struct
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return new Nullable<V>();
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
    }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        } 
    }
}

