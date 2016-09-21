using System.Collections.Generic;
using System.Linq;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    public static class EnumerableExtensions
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

        public static IEnumerable<T> PageBy<T>(this IEnumerable<T> source, IPageInfo pageInfo = null)
        {
            if (pageInfo == null) { return source; }
            source = source.Skip(pageInfo.StartIndex);
            if (pageInfo.PageSize.HasValue)
            {
                source = source.Take(pageInfo.PageSize.Value);
            }
            return source;
        }
    }
}
