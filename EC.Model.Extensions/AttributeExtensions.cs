using System.Collections.Generic;
using System.Linq;
using EC.Constants;
using EC.Model.Interfaces;

namespace EC.Model.Extensions
{
    /// <summary>
    /// Extensions for handling IAttributes.
    /// </summary>

    public static class AttributeExtensions
    {
        /// <summary>
        ///////////// Filter list to return all attributes with its Key starting with 
        ///////////// <see cref="AttributeConstants.GROUP_DEFINITION_BASE_NAME"/>.
        ///////////// </summary>
        ///////////// <param name="source">List of attributes</param>
        ///////////// <returns>Return list of filtered group attributes</returns>

        //////////public static IList<IAttribute> FilterByGroupDefinitionAttributes(this IList<IAttribute> source)
        //////////{
        //////////    return source.Where(a => a.Key != null && a.Key.StartsWith(AttributeConstants.GROUP_DEFINITION_BASE_NAME)).ToList();
        //////////}

        ///////////// <summary>
        ///////////// Filter queryable to return all attributes with its Key starting with
        ///////////// <see cref="AttributeConstants.GROUP_DEFINITION_BASE_NAME"/>.
        ///////////// </summary>
        ///////////// <param name="source">Queryable of attributes</param>
        ///////////// <returns>Return queryable filtered by group definition attributes</returns>

        //////////public static IQueryable<IAttribute> FilterByGroupDefinitionAttributes(this IQueryable<IAttribute> source)
        //////////{
        //////////    return source.Where(a => a.Key != null && a.Key.StartsWith(AttributeConstants.GROUP_DEFINITION_BASE_NAME));
        //////////}

        ///////////// <summary>
        ///////////// Filter list to return all attributes with its Key starting with 
        ///////////// <see cref="AttributeConstants.CAPABILITY_BASE_NAME"/>.
        ///////////// </summary>
        ///////////// <param name="source">List of attributes</param>
        ///////////// <returns>List of attributes filtered by capability attributes</returns>

        //////////public static IList<IAttribute> FilterByCapabilityAttributes(this IList<IAttribute> source)
        //////////{
        //////////    return source.Where(a => a.Key != null && a.Key.StartsWith(AttributeConstants.CAPABILITY_BASE_NAME)).ToList();
        //////////}  

        ///////////// <summary>
        ///////////// Filter queryable to return all attributes with its Key starting with 
        ///////////// <see cref="AttributeConstants.CAPABILITY_BASE_NAME"/>.
        ///////////// </summary>
        ///////////// <param name="source">Queryable of attributes</param>
        ///////////// <returns>Return queryable filtered by capability attributes</returns>

        //////////public static IQueryable<IAttribute> FilterByCapabilityAttributes(this IQueryable<IAttribute> source)
        //////////{
        //////////    return source.Where(a => a.Key != null && a.Key.StartsWith(AttributeConstants.CAPABILITY_BASE_NAME));
        //////////}  
    }
}