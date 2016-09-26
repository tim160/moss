using System.Collections.Generic;
using System.Linq;

namespace EC.Core.Common
{
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Breaks an enumerable into a list of sub-list chunks where each chunk has at most <paramref name="chunkSize"/> items.
        /// </summary>
        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}