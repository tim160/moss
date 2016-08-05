using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Describes specialized SELECT behavior when performing a 
    /// data retrieval operation of business objects from the data
    /// store, such as TOP(n) or DISTINCT.
    /// </summary>
    public enum SelectPredicate
    {
        /// <summary>
        /// Indicates a SELECT TOP (n) select predicate statement.
        /// </summary>
        Top,
        /// <summary>
        /// Indicates a SELECT TOP (n) PERCENT select predicate statement.
        /// </summary>
        //TopPercent,
        /// <summary>
        /// Indicates a SELECT DISTINCT select predicate statement.
        /// </summary>
        Distinct
    }
}