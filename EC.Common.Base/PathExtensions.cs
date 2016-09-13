using System.Collections.Generic;
using System.Linq;
using EC.Constants;

namespace EC.Common.Base
{
    /// <summary>
    /// Use this extensions if the elements implement the interface IPathElement.
    /// </summary>

    public static class PathExtensions
    {
        /// <summary>
        /// Return the string representation of the list.
        /// </summary>
        /// <param name="source">List of path elements to convert into a path</param>
        /// <returns>Return string representation (path) of the <paramref name="source"/>. Return <c>null</c> if the <paramref name="source"/> is <c>null</c> or empty</returns>

        public static string ToPath(this IEnumerable<IPathElement> source)
        {
            if (source == null || source.Count() == 0) { return null; }
            return "/" + source.Select(l => l.PathElementName).Aggregate((a, b) => a + PathConstants.ElementSeparator + b);
        }
    }
}
