using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EC.Common.Base
{
    /// <summary>
    /// An implementation of IEqualityComparer accepting a lambda expression.
    /// </summary>
    /// <typeparam name="T">Lambda expression to compare against</typeparam>

    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> m_EqualityLambda;
        private readonly Func<T, int> m_HashCodeLambda;

        /// <summary>
        /// Equality comparer accepting a lambda to specify the equals expression.
        /// </summary>
        /// <remarks>
        /// How to specify <paramref name="hashCodeLambda"/> according to <param name="equalityLambda"></param>: <br/>
        /// A <paramref name="hashCodeLambda"/> expression is necessary because you have 2 lists of different objects, and you want to check whether those 2 objects are equal
        /// for some properties (used in <paramref name="equalityLambda"/>, but other properties don't matter otherwise. 
        /// The <paramref name="hashCodeLambda"/> should return a combined int value of all properties used in <paramref name="equalityLambda"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// public class Address { public string StreetName; public int StreetNumber; }
        /// 
        /// var firstList = new List<Address>();
        /// var secondList = new List<Address>();
        /// var noSeconds1 = firstList.Except(secondList, new LambdaEqualityComparer<Address>((a,b)=> a.StreetName == b.StreetName, x => x.StreetName.GetHashCode()));
        /// var noSeconds2 = firstList.Except(secondList, new LambdaEqualityComparer<Address>((a,b)=> a.StreetName == b.StreetName && a.StreetNumber == b.StreetNumber, x => x.StreetName.GetHashCode() + x.StreetNumber));
        /// </code>
        /// </example>
        /// <param name="equalityLambda">This is the lambda expression as equals</param>
        /// <param name="hashCodeLambda">
        /// Lambda to get the int value of the object. 
        /// Please refer to the remarks how to specify this lambda according to <paramref name="equalityLambda"/>
        /// </param>
        /// <exception cref="ArgumentNullException">If one of the parameters are <c>null</c></exception>

        public LambdaEqualityComparer(Func<T, T, bool> equalityLambda, Func<T, int> hashCodeLambda)
        {
            if (equalityLambda == null) { throw new ArgumentNullException("equalityLambda", "Parameter must not be null."); }
            if (hashCodeLambda == null) { throw new ArgumentNullException("hashCodeLambda", "Parameter must not be null."); }
            m_EqualityLambda = equalityLambda;
            m_HashCodeLambda = hashCodeLambda;
        }

        public bool Equals(T first, T second)
        {
            return m_EqualityLambda(first, second);
        }

        public int GetHashCode(T value)
        {
            return m_HashCodeLambda(value);
        }
    }
}
