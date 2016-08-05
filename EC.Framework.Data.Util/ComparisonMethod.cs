using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Indicates the method in which two values should be compared. 
    /// </summary>
    public enum ComparisonMethod
    {
        /// <summary>
        /// Represents the SQL '=' operator.
        /// </summary>
        Equals,
        /// <summary>
        /// Represents the SQL not equals operator.
        /// </summary>
        NotEquals,
        /// <summary>
        /// Represents the SQL greater than operator.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Represents the SQL less than operator.
        /// </summary>
        LessThan,
        /// <summary>
        /// Represents the SQL greater than or equal to operator.
        /// </summary>
        GreaterThanOrEqualTo,
        /// <summary>
        /// Represents the SQL less than or equal to operator.
        /// </summary>
        LessThanOrEqualTo,
        /// <summary>
        /// Represents the SQL 'LIKE*%' operator.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Represents the SQL 'LIKE %*%' operator.
        /// </summary>
        Contains,
        /// <summary>
        /// Represents the SQL 'NOT LIKE %*%' operator.
        /// </summary>
        NotContains,
        /// <summary>
        /// Represents the SQL 'LIKE %*' operator.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Represents the SQL 'IS NULL' operator.
        /// </summary>
        IsNull,
        /// <summary>
        /// Represents the SQL 'IS NOT NULL' operator.
        /// </summary>
        IsNotNull,
        /// <summary>
        /// Represents the SQL 'In' operator.
        /// </summary>
        In,
        /// <summary>
        /// Represents the SQL 'In' operator.
        /// </summary>
        NotIn
    }
}