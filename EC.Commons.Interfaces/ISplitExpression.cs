using System.Collections.Generic;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Expression split into its pieces.
    /// </summary>

    public interface ISplitExpression
    {
        /// <summary>
        /// Expression name.
        /// </summary>

        string Name { get; set; }

        /// <summary>
        /// Flag whether the expression is used inverse.
        /// </summary>

        bool IsInverse { get; set; }

        /// <summary>
        /// List of trimmed arguments for the expression.
        /// </summary>

        IList<string> Arguments { get; set; }

        /// <summary>
        /// Create string from the expression values.
        /// <remarks>
        /// An expression format looks like [!][Name]([Arguments separated with ','])
        /// </remarks>
        /// </summary>
        /// <returns>Return the string expression.</returns>

        string CreateExpressionString();
    }
}
