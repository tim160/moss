using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Models.Database.Contexts;
using log4net;

namespace EC.Services.API.v1
{
	public class ServiceBase<T>
		where T : class
	{
		protected readonly ApplicationContext _appContext;
		protected readonly DbSet<T> _set;
		protected readonly ILog _logger;

		public ServiceBase()
		{
			_logger = LogManager.GetLogger(GetType());

			_appContext = new ApplicationContext();
			_appContext.Configuration.ProxyCreationEnabled = false;
			_appContext.Configuration.LazyLoadingEnabled = false;

			_set = _appContext.Set<T>();
		}

		public Task<PagedList<TModel>> GetPagedAsync<TOrderKey, TModel>(
			int page,
			int pageSize,
			Expression<Func<T, bool>> filter = null,
			Expression<Func<T, TOrderKey>> order = null)
		{
			return _set.GetPagedAsync<T, TOrderKey, TModel>(page, pageSize, filter, order);
		}
	}
}