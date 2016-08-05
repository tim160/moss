using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// SelectPredicateAttribute decorates a property of a
    /// SelectPredicate that represents a simple way to load a
    /// collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SelectPredicateAttribute : Attribute, ICloneable
    {
        #region Property(s)
        /// <summary>
        /// Gets or sets the predicate.
        /// </summary>
        /// <value>The predicate.</value>
        private SelectPredicate m_Predicate;
        public SelectPredicate Predicate
        {
            get { return m_Predicate; }
            set { m_Predicate = value; }
        }

        /// <summary>
        /// Gets or sets the predicate parameter.
        /// </summary>
        /// <value>The predicate parameter.</value>
        private int m_PredicateParameter;
        public int PredicateParameter
        {
            get { return m_PredicateParameter; }
            set { m_PredicateParameter = value; }
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPredicateAttribute"/> class.
        /// </summary>
        /// <param name="selectPredicate">The select predicate.</param>
        public SelectPredicateAttribute(SelectPredicate selectPredicate)
        {
            this.Predicate = selectPredicate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPredicateAttribute"/> class.
        /// </summary>
        /// <param name="selectPredicate">The select predicate.</param>
        /// <param name="parameter">The parameter.</param>
        public SelectPredicateAttribute(SelectPredicate selectPredicate, int parameter)
        {
            if (selectPredicate == SelectPredicate.Distinct)
            {
                throw new ArgumentException("Invalid SelectPredicateAttribute initialization; Optional parameter only to be specified with a TOP or TOP_PERCENT select predicate!");
            }
            this.Predicate = selectPredicate;
            this.PredicateParameter = parameter;
        }
        #endregion

        #region Method(s)
        /// <summary>
        /// Gets the SQL predicate statement.
        /// </summary>
        /// <returns></returns>
        public string GetSqlPredicateStatement(IConnectionInfo connectionInfo)
        {
            if ((this.Predicate != SelectPredicate.Distinct) && (this.PredicateParameter == 0))
            {
                throw new InvalidOperationException("Cannot generate TOP or TOP_PERCENT predicate without specifying an integer parameter!");
            }
            if (Predicate == SelectPredicate.Top)
            {
                return connectionInfo.GetPredicateTop(PredicateParameter.ToString());
            }
            else
            {
                return "DISTINCT";
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}