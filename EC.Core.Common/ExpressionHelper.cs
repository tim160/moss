using System;
using System.Linq.Expressions;
using EC.Errors.CommonExceptions;

namespace EC.Core.Common
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Gets name of property passed in by lambda expression
        /// </summary>
        /// <typeparam name="T">Class or interface that property belongs to</typeparam>
        /// <param name="propertyRefExpr">Lambda expression that provides the property</param>
        /// <returns>Name</returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyRefExpr)
        {
            try
            {
                MemberExpression memberExpression;
                // When a value type is boxed, we need to use the operand of the convert expression
                if ((propertyRefExpr.Body.NodeType == ExpressionType.Convert) || (propertyRefExpr.Body.NodeType == ExpressionType.ConvertChecked))
                {
                    var unaryExpression = propertyRefExpr.Body as UnaryExpression;
               //////tim     memberExpression = (unaryExpression?.Operand) as MemberExpression;
                }
                else
                {
                    memberExpression = propertyRefExpr.Body as MemberExpression;
                }
           ////     if (memberExpression == null)
                {
             /////       throw new ParameterValidationException("propertyRefExpr", "You must pass a lambda of the form: 'c => c.Property'");
                }
                return "";//////// memberExpression.Member.Name;
            }
            catch (Exception)
            {
                throw new ParameterValidationException("propertyRefExpr", "You must pass a lambda of the form: 'c => c.Property'");
            }
        }
    }
}
