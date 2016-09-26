using System;
using System.Collections.Generic;

namespace EC.Common.Interfaces
{
    public interface IBaseExpressionHelper
    {
        /// <summary>
        /// Check if the expression is inversed (indicated by a leading '!').
        /// </summary>
        /// <param name="expression">Expression string.</param>
        /// <returns>Return <c>true</c> if the expression is inversed. Return <c>false</c> if the expression is not inversed.</returns>

        bool IsInverseExpression(string expression);

        /// <summary>
        /// If an expression is inversed with a leading '!' - remove it and return the expression name without the leading '!'.
        /// This operation is idempotent (=can be executed multiple times with the same result).
        /// </summary>
        /// <param name="inverseExpression">Expression name with or without leading '!'</param>
        /// <returns>Return the expression name without a leading '!'.</returns>

        string GetPositiveExpression(string inverseExpression);

        /// <summary>
        /// Split the expression string into its pieces (name, isInverse, args).
        /// <note>
        /// No check is done whether the expression has the correct argument count or whether the expression exists.
        /// </note>
        /// </summary>
        /// <param name="expressionString">String of a format [!][predName]([arguments]) where the arguments are separated by ','</param>
        /// <returns>Return the split expression. Return <c>null</c> if the <paramref name="expressionString"/> has the wrong format.</returns>

        ISplitExpression SplitExpressionValue(string expressionString);

        /// <summary>
        /// Split expressions into single expression representations.
        /// <remarks>
        /// The expression are separated with ';'.
        /// No check whether the expression exists or has the correct count of arguments is done.
        /// </remarks>
        /// </summary>
        /// <param name="expression">Expression as string</param>
        /// <returns>Return list of expression definitions</returns>
        /// <exception cref="FormatException">If an expression has a wrong format (there is not check whether the expression exists).</exception>

        IList<ISplitExpression> DecomposeExpression(string expression);

        /// <summary>
        /// Create string representation from a list of <paramref name="splitExpressions"/>.
        /// </summary>
        /// <remarks>
        /// Every expression is separated by ';'.
        /// </remarks>
        /// <param name="splitExpressions">List of expressions</param>
        /// <returns>Return the string representation of <paramref name="splitExpressions"/></returns>

        string ComposeExpression(IList<ISplitExpression> splitExpressions);
    }
}
