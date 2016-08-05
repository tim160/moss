using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Indicates the method in which two values should be compared. 
    /// </summary>
    public enum AggregateFunction
    {
        /// <summary>
        /// Represents the SQL Min
        /// </summary>
        Min,
        /// <summary>
        /// Represents the SQL Max
        /// </summary>
        Max,
        /// <summary>
        /// Represents None
        /// </summary>
        None
    }
}