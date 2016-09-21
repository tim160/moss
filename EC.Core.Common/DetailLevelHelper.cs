using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(DetailLevelHelper))]

    public class DetailLevelHelper
    {

        /// <summary>
        /// Strip off <paramref name="prefix"/> (including leading '.') from all detail levels if possible.
        /// Remove detail levels which don't have the <paramref name="prefix"/>.
        /// </summary>
        /// <param name="detailLevel">Detail level string</param>
        /// <param name="prefix">Prefix to strip off.</param>
        /// <returns>
        /// Return detail level string without <paramref name="prefix"/> also remove leading '.'. 
        /// Return unchanged detail level if <paramref name="detailLevel"/> or <paramref name="prefix"/> is <c>null</c>, empty or only contains white spaces 
        /// Return an empty <c>string</c> if no detail level starts with the <paramref name="prefix"/>.
        /// </returns>

        public string TruncateDetailLevelsByPrefix(string detailLevel, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(detailLevel) || !detailLevel.ToLower().Contains(prefix.ToLower())) { return detailLevel; }

            StringBuilder newDetailLevel = new StringBuilder();
            char space = ' ';
            char[] trimDot = new char[] { '.' };

            var split = this.SplitDetailLevels(detailLevel);
            prefix = prefix.ToLower();
            foreach (string currentLevel in split)
            {
                if (currentLevel.StartsWith(prefix) && (currentLevel != prefix))
                {
                    // If current level starts with the prefix and doesn't only consist of the prefix.
                    var pattern = string.Format("^{0}\\.", prefix);
                    var regex = new Regex(pattern);
                    var tempDLevel = regex.Replace(currentLevel, "");

                    if (tempDLevel.Length > 0)
                    {
                        // Add changed detail level if still something there...
                        newDetailLevel.Append(tempDLevel);
                        newDetailLevel.Append(space);
                    }
                }
            }
            return newDetailLevel.ToString();
        }

        /// <summary>
        /// Split detail level string by ' ' and return its levels as enumerable (all lower cased).
        /// </summary>
        /// <param name="detailLevel">Detail level string</param>
        /// <returns>Return enumerable with all lower cased detail levels. Return an empty enumerable if no detail level exists.</returns>

        public IEnumerable<string> SplitDetailLevels(string detailLevel)
        {
            if (string.IsNullOrWhiteSpace(detailLevel)) { return Enumerable.Empty<string>(); }
            return Regex.Split(detailLevel, @"\s").Where(s => s != "").Select(s => s.ToLower());
        }
    }
}
