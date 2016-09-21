using System.Collections.Generic;
using System.Text;
using EC.Core.Common;
using EC.Common.Interfaces;

namespace MarineLMS.SharedModel.Impl
{
    /// <summary>
    /// When a expression (e.g. predicate) is processed, this is the 'typed' representation
    /// of the expression.
    /// </summary>

    [TransientType]
    [RegisterAsType(typeof(ISplitExpression))]

    public class SplitExpression : ISplitExpression
    {
        /// <summary>
        /// Predicate name.
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// Flag whether the predicate is used inverse.
        /// </summary>

        public bool IsInverse { get; set; }

        /// <summary>
        /// List of arguments for the predicate.
        /// </summary>

        public IList<string> Arguments { get; set; }

        /// <summary>
        /// Create string from the expression values.
        /// <remarks>
        /// An expression format looks like [!][Name]([Arguments separated with ','])
        /// </remarks>
        /// </summary>
        /// <returns>Return the string expression.</returns>

        public string CreateExpressionString()
        {
            StringBuilder strBuilder = new StringBuilder();
            if (this.IsInverse)
            {
                strBuilder.Append("!");
            }

            strBuilder.Append(this.Name);
            strBuilder.Append("(");
            string separator = string.Empty;
            strBuilder.Append(string.Join(",", this.Arguments));
            strBuilder.Append(")");

            return strBuilder.ToString();
        }

        public SplitExpression()
        {
            this.Arguments = new List<string>();
        }
    }
}
