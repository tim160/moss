using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Indicates the manner in which the criteria is concatenated to any previous criteria.  
    /// </summary>
    public enum OperandType
    {
        /// <summary>
        /// There is no operation run on the previous criteria and the new criteria.
        /// </summary>
        None,
        /// <summary>
        /// Perform an And operation on the previous criteria and the new criteria.
        /// </summary>
        And,
        /// <summary>
        /// Perform an Or operation on the previous criteria and the new criteria.
        /// </summary>
        Or
    }
}