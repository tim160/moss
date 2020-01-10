using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using EC.Common.Base;

namespace EC.Services.API.v1
{
	internal static class QueriesBase
	{
		public static async Task<PagedList<TModel>> GetPagedAsync<T, TOrderKey, TModel>(
			this DbSet<T> set,
			int page,
			int pageSize,
			Expression<Func<T, bool>> filter = null,
			Expression<Func<T, TOrderKey>> order = null,
			params Expression<Func<T, object>>[] includes)
			where T : class
		{
			IQueryable<T> querySource = set;

			if (includes.Length > 0)
			{
				foreach (Expression<Func<T, object>> include in includes)
				{
					querySource = querySource.Include(include);
				}
			}

			if (order != null)
			{
				querySource = querySource.OrderBy(order);
			}
			if (filter != null)
			{
				querySource = querySource.Where(filter);
			}

			int totalItems = await querySource
				.CountAsync()
				.ConfigureAwait(false);

			// Только при наличии указанного порядка возможно разбиение на страницы.
			if (page > 1 && order != null)
			{
				querySource = querySource.Skip((page - 1) * pageSize);
			}
			if (pageSize > 0)
			{
				querySource = querySource.Take(pageSize);
			}

			return new PagedList<TModel>
			{
				Total = totalItems,
				Items = await querySource
					.ProjectTo<TModel>()
					.ToListAsync()
					.ConfigureAwait(false)
			};
		}
	}
}