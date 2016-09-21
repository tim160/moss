using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel;
using EC.Common.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Basic implementation of common functions for expression handling.
    /// </summary>

    public abstract class BaseExpressionHelper : IBaseExpressionHelper
    {
        /// <summary>
        /// Split the predicate string into its pieces (name, isInverse, args).
        /// Also trim every value.
        /// <note>
        /// No check is done whether the predicate has the correct argument count or whether the expression exists.
        /// </note>
        /// </summary>
        /// <param name="expressionString">String of a format [!][expressionName]([arguments]) where the arguments are separated by ','</param>
        /// <returns>Return the split predicate. Return <c>null</c> if the <paramref name="expressionString"/> has the wrong format.</returns>

        public ISplitExpression SplitExpressionValue(string expressionString)
        {
            var argumentList = expressionString.Split(new char[] { '(', ',', ')' }).Where(s => s.Trim() != string.Empty).ToList<string>();
            if (argumentList.Count == 0)
            {
                return null;
            }

            string predicateName = argumentList.FirstOrDefault();  // Get predicate name.
            argumentList.RemoveAt(0);  // Get rid of expression name to get the actual argument list.

            var pred = Kernel.Resolve<ISplitExpression>();
            pred.IsInverse = IsInverseExpression(predicateName);  // Is it an inverse predicate?
            pred.Name = GetPositiveExpression(predicateName);  // Remove ! from the predicate.
            pred.Arguments = argumentList;

            return pred;
        }

        /// <summary>
        /// Split expressions into a list of single expressions.
        /// <remarks>
        /// The expressions are separated with ';'.
        /// No check whether the predicate exists or has the correct count of arguments is done.
        /// </remarks>
        /// </summary>
        /// <param name="expression">Expression as string</param>
        /// <returns>Return list of expression definitions</returns>
        /// <exception cref="FormatException">If an expression has a wrong format (there is not check whether the expression exists).</exception>

        public IList<ISplitExpression> DecomposeExpression(string expression)
        {
            var expressionList = expression.SplitStringBy(new char[] { ';' });

            IList<ISplitExpression> result = new List<ISplitExpression>();
            foreach (string p in expressionList)
            {
                var sExpr = SplitExpressionValue(p);
                if (sExpr == null) { throw new FormatException(string.Format("Expression '{0}' has a wrong format.", p)); }
                result.Add(sExpr);
            }

            return result;
        }

        /// <summary>
        /// Create string representation from a list of <paramref name="splitExpressions"/>.
        /// </summary>
        /// <remarks>
        /// Every expression is separated by ';'.
        /// </remarks>
        /// <param name="splitExpressions">List of expressions</param>
        /// <returns>Return the string representation of <paramref name="splitExpressions"/></returns>

        public string ComposeExpression(IList<ISplitExpression> splitExpressions)
        {
            StringBuilder result = new StringBuilder();
            string separator = string.Empty;
            foreach (var e in splitExpressions)
            {
                result.Append(separator);
                separator = ";";
                if (e.IsInverse) { result.Append("!"); }
                result.Append(e.Name);
                result.Append("(");
                if (e.Arguments.Count > 0) { result.Append(e.Arguments.Aggregate((a, b) => a + "," + b)); }
                result.Append(")");
            }

            return result.ToString();
        }

        /// <summary>
        /// Check if the expression is inversed (indicated by a leading '!').
        /// </summary>
        /// <param name="expression">Expression string.</param>
        /// <returns>Return <c>true</c> if the expression is inversed. Return <c>false</c> if the expression is not inversed.</returns>

        public bool IsInverseExpression(string expression)
        {
            return expression.StartsWith("!");
        }

        /// <summary>
        /// If an expression is inversed with a leading '!' - remove it and return the expression name without the leading '!'.
        /// This operation is idempotent (=can be executed multiple times with the same result).
        /// </summary>
        /// <param name="inverseExpression">Expression name with or without leading '!'</param>
        /// <returns>Return the expression name without a leading '!'.</returns>

        public string GetPositiveExpression(string inverseExpression)
        {
            return inverseExpression.TrimStart(new char[] { '!' });
        }

        /// <summary>
        /// Hide constructor with 0 parameters.
        /// </summary>

        protected BaseExpressionHelper ()
        {
        }

        public BaseExpressionHelper(IKernel k, ILogger l)
        {
            Kernel = k;
            Logger = l;
        }

        protected IKernel Kernel { get; set; }
        protected ILogger Logger { get; set; }
    }
}
