using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Util
{
    public class StringUtil
    {
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Performs a case insensitive (invariant culture) compare of 2 strings, with front and back whitespace trimmed.
        /// </summary>
        /// <param name="str">The string to compare.</param>
        /// <param name="other">The string to compare against str.</param>
        /// <returns>True if they are equal.</returns>
        public static bool CaseInsensitiveTrimmedCompare(string str, string other)
        {
            return string.Compare(str.Trim(), other.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Replaces "smart quotes" with real quotes. Handles single and double quotes.
        /// </summary>

        public static string ReplaceSmartQuotes(string str)
        {
            if (string.IsNullOrEmpty(str)) { return str; }

            return str
                .Replace('\u2018', '\'')
                .Replace('\u2019', '\'')
                .Replace('\u201c', '\"')
                .Replace('\u201d', '\"');
        }

        /// <summary>
        /// 21.03
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDecimalToStringAmount(decimal _amount)
        {
            string sAmount = String.Format("{0:C2}", Convert.ToInt32(_amount));
            sAmount = sAmount.Replace("$", "");
            return sAmount;

        }

        /// <summary>
        /// takes a credit card and converts to XXXXXXXXX1234
        /// </summary>
        /// <param name="cc_number"></param>
        /// <returns></returns>
        public static string ConvertCCInfoToLast4DigitsInfo(string cc_number)
        {
            string saved_cc_number = "";
            for (int i = 0; i < cc_number.Length; i++)
            {
                if (i + 4 < cc_number.Length)
                    saved_cc_number = saved_cc_number + "X";
                else
                    saved_cc_number = saved_cc_number + cc_number[i];

            }
            return saved_cc_number.ToString();

        }

        /// <summary>
        /// Gets Random Letter
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomLetter(int length)
        {
            const string chars = "ABCDEFGHKLMNPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// generates random string with length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// takes several first words from description to return 'preview' version
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numberWords"></param>
        /// <returns></returns>
        public static string FirstWords(string input, int numberWords)
        {
            try
            {
                // Number of words we still want to display.
                int words = numberWords;
                // Loop through entire summary.
                for (int i = 0; i < input.Length; i++)
                {
                    // Increment words on a space.
                    if (input[i] == ' ')
                    {
                        words--;
                    }
                    // If we have no more words to display, return the substring.
                    if (words == 0)
                    {
                        return input.Substring(0, i);
                    }
                }
            }
            catch (Exception)
            {
                // Log the error.
            }
            return input;
        }

        /// <summary>
        /// Replaces letters in password, so they will not look like numbers - Like 0 vs o, 1 vs I - etc. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ReplaceForUI(string text)
        {
            string better_text = text.Trim();

            better_text = better_text.Replace('O', 'K');
            better_text = better_text.Replace('o', 'K');


            better_text = better_text.Replace('1', '8');
            better_text = better_text.Replace('0', '5');

            better_text = better_text.Replace('I', 'A');
            better_text = better_text.Replace('i', 'a');

            better_text = better_text.Replace('J', 'W');
            better_text = better_text.Replace('j', 'w');

            better_text = better_text.Replace('L', 'S');
            better_text = better_text.Replace('l', 's');

            return better_text;
        }
    }
}
