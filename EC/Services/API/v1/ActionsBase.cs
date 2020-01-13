using System;
using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper;
using EC.Errors.CommonExceptions;

namespace EC.Services.API.v1
{
	internal static class ActionsBase
	{
		public static T Add<T, TModel>(
			this DbSet<T> dbSet,
			TModel model,
			Action<T> additionalActions = null)
			where T : class
			where TModel : class
		{
			if (dbSet == null)
			{
				throw new ArgumentNullException(nameof(dbSet));
			}
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			T item = Mapper.Map<T>(model);
			additionalActions?.Invoke(item);

			return dbSet.Add(item);
		}

		public static async Task<T> UpdateAsync<T, TModel>(
			this DbSet<T> dbSet,
			int id,
			TModel model,
			Action<T> additionalActions = null)
			where T : class
			where TModel : class
		{
			if (dbSet == null)
			{
				throw new ArgumentNullException(nameof(dbSet));
			}
			if (id == 0)
			{
				throw new ArgumentException("The identifier cannot be empty.", nameof(id));
			}
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			T item = await dbSet
				.FindAsync(id)
				.ConfigureAwait(false);
			if (item == null)
			{
				throw new NotFoundException($"Item with ID '{id}' not found.", typeof(T));
			}

			Mapper.Map(model, item);
			additionalActions?.Invoke(item);

			return item;
		}
	}
}