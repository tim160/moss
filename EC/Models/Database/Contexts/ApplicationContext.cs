using System.Diagnostics;

namespace EC.Models.Database.Contexts
{
	public class ApplicationContext : ECEntities
	{
		public ApplicationContext()
			: base()
		{
			Database.Log = sql => Debug.Write(sql);
		}
	}
}