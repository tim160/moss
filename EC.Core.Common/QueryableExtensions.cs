using System;
using System.Linq;
using System.Linq.Expressions;
using EC.Constants;
using EC.Errors.CommonExceptions;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Common extensions for EF and Linq queryables.
    /// </summary>

    public static class QueryableExtensions
    {
        /*
        public static IQueryable<T> PageBy<T>(this IQueryable<T> source, IPageInfo pageInfo = null)
        {
            return source.AsEnumerable().PageBy(pageInfo).AsQueryable();
        }

        /// <summary>
        /// Generic sort method that takes sort parameter name
        /// </summary>
        /// <returns>Sorted query nominally; original query if propertyName is null or sortDirection is None</returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, SortDirectionEnum sortDirection, string propertyName)
        {
            if (propertyName == null) { return source; }

            // Select name for sort function (which we will call by name below)
            string sortFunctionName;
            switch (sortDirection)
            {
                case SortDirectionEnum.Ascending: sortFunctionName = "OrderBy"; break;
                case SortDirectionEnum.Descending: sortFunctionName = "OrderByDescending"; break;
                default: return source;
            }

            // Get all the property information to construct expression
            var propertyAndParameterExpressions = GetPropertyAndParameterExpressions<T>(propertyName);
            var typeArgs = new[] {typeof (T), propertyAndParameterExpressions.PropertyExpression.Type};

            // Because OrderBy and OrderByDescending require a Expression<Func<TEntity, TKey>> parameter, and we only know TEntity at compile time,
            // we need to create a dynamic Expression<Func<TEntity, TKey>> after we have found the sort parameter and its type
            var delegateType = typeof (Func<,>).MakeGenericType(typeArgs);
            var lambda = Expression.Lambda(delegateType, propertyAndParameterExpressions.PropertyExpression, propertyAndParameterExpressions.ParameterExpression);

            // Create sort expression
            var sortExpression = Expression.Call(
                typeof(Queryable),
                sortFunctionName,
                typeArgs,
                source.Expression,
                lambda);

            // Return provider specific query for sort
            return source.Provider.CreateQuery<T>(sortExpression);
        }


        /// <summary>
        /// Gets property and parameter expressions for property of class/interface T
        /// </summary>
        /// <typeparam name="T">Class or interface that property belongs to</typeparam>
        /// <returns>PropertyAndParameterExpressions instance that contains property and parameter expressions for property if successful, null otherwise</returns>
        private static PropertyAndParameterExpressions GetPropertyAndParameterExpressions<T>(string propertyName)
        {
            if (propertyName == null) { return null; }
            var type = typeof(T);
            try
            {
                // Check for property in current type (and inherited classes if a class)
                var propertyAndParameterExpressions = GetPropertyAndParameterExpressionsExpression(type, propertyName);
                if (propertyAndParameterExpressions == null)
                {
                    // Check for property in interfaces
                    var enumerator = type.GetInterfaces().GetEnumerator();
                    for (enumerator.MoveNext(); (propertyAndParameterExpressions == null) && (enumerator.Current != null); enumerator.MoveNext())
                    {
                        if (enumerator.Current is Type)
                        {
                            propertyAndParameterExpressions = GetPropertyAndParameterExpressionsExpression(enumerator.Current as Type, propertyName);
                        }
                    }
                }
                if (propertyAndParameterExpressions == null) { throw new NotFoundException($"Property name '{propertyName}' does not exist for class '{type.Name}'", propertyName, type); }
                return propertyAndParameterExpressions;
            }
            catch (Exception exception)
            {
                throw new NotFoundException($"Property name '{propertyName}' does not exist for class '{type.Name}': {exception.Message}", propertyName, type);
            }
        }

        /// <summary>
        /// Helper function for iterating through class/interfaces
        /// </summary>
        /// <remarks>
        /// Since PropertyOrField (and Property) does not search all interfaces for a property, we must iterate through them
        /// </remarks>
        /// <param name="type">Specific class or interface to search for property</param>
        /// <returns>PropertyAndParameterExpressions instance that contains property and parameter expressions for property if successful, null otherwise</returns>
        private static PropertyAndParameterExpressions GetPropertyAndParameterExpressionsExpression(Type type, string propertyName)
        {
            var parameterExpression = Expression.Parameter(type);
            MemberExpression propertyExpression;
            try
            {
                // PropertyOrField method will except with ArgumentException if the property is not in the class (nor base classes) nor interface of type
                propertyExpression = Expression.PropertyOrField(parameterExpression, propertyName);
            }
            catch (ArgumentException)
            {
                return null;
            }
            return new PropertyAndParameterExpressions { PropertyExpression = propertyExpression, ParameterExpression = parameterExpression };
        }

        private class PropertyAndParameterExpressions
        {
            public MemberExpression PropertyExpression { get; set; }
            public ParameterExpression ParameterExpression { get; set; }
        }
*/
    }
}