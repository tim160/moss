using System.Collections.Generic;

namespace EC.Common.Base
{
	public class PagedList<T>
	{
		public int Total { get; set; }
		public IReadOnlyCollection<T> Items { get; set; }
	}
}
