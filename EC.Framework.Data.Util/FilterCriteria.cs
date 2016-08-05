using System;
using System.Collections;

namespace EC.Framework.Data
{
    /// <summary>
    /// FilterCriteria provides a generic filtering mechanism for
    /// business object collections. 
    /// </summary>
    public class FilterCriteria
    {
        #region Property(s)
        #region Criteria Property
        /// <summary>
        /// Gets or sets the criteria.
        /// </summary>
        /// <value>The criteria.</value>
        public PersistentCriteria Criteria
        {
            get { return m_Criteria; }
            set { m_Criteria = value; }
        }
        private PersistentCriteria m_Criteria;
        #endregion Criteria Property

        public bool FilterValueListOnDisplay
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Parameterless constructor initialises the criteia.
        /// </summary>
        public FilterCriteria()
        {
            m_Criteria = new PersistentCriteria();
        }

        public FilterCriteria(FilterCriteria fc)
        {
            m_Criteria = fc.Criteria.Clone();
        }
        #endregion

        #region Methods

        #region ClearFilters Method

        /// <summary>
        /// Provides base implementation of ClearFilters functionality;
        /// this just re-creates a blank PersistentCriteria, just 
        /// like the constructor.
        /// </summary>
        public void ClearFilters()
        {
            this.Criteria = new PersistentCriteria();
        }
        #endregion ClearFilters Method

        public void BeginExpression()
        {
            SelectionCriterion selectionCriterion = new SelectionCriterion(Expression.Begin);
            m_Criteria.AddCriteria(selectionCriterion);
        }

        public void EndExpression()
        {
            SelectionCriterion selectionCriterion = new SelectionCriterion(Expression.End);
            m_Criteria.AddCriteria(selectionCriterion);
        }

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(Type type, string propertyName, object value)
        {
            return AddXmlFilter(type, null, 0, string.Empty, OperandType.None, propertyName, ComparisonMethod.Equals, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, string xmlPath, string propertyName, object value)
        {
            return AddXmlFilter(type, dataType, 0, xmlPath, OperandType.None, propertyName, ComparisonMethod.Equals, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, object value)
        {
            return AddXmlFilter(type, dataType, length, xmlPath, OperandType.None, propertyName, ComparisonMethod.Equals, value);
        }

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="combineInstruction">The combine instruction.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(Type type, OperandType combineInstruction, string propertyName, object value)
        {
            return AddXmlFilter(type, null, 0, string.Empty, combineInstruction, propertyName, ComparisonMethod.Equals, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, string xmlPath, OperandType combineInstruction, string propertyName, object value)
        {
            return AddXmlFilter(type, dataType, 0, xmlPath, combineInstruction, propertyName, ComparisonMethod.Equals, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, Int32 length, string xmlPath, OperandType combineInstruction, string propertyName, object value)
        {
            return AddXmlFilter(type, dataType, length, xmlPath, combineInstruction, propertyName, ComparisonMethod.Equals, value);
        }

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="compareMethod">The compare method.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(Type type, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, null, 0, string.Empty, OperandType.None, propertyName, compareMethod, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, string xmlPath, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, dataType, 0, xmlPath, OperandType.None, propertyName, compareMethod, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, dataType, length, xmlPath, OperandType.None, propertyName, compareMethod, value);
        }

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="combineInstruction">The combine instruction.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="compareMethod">The compare method.</param>
        /// <param name="value">The value.</param>
        /// 

        public FilterCriteria AddFilter(Type type, OperandType combineInstruction, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, null, 0, string.Empty, combineInstruction, propertyName, compareMethod, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, string xmlPath, OperandType combineInstruction, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, dataType, 0, xmlPath, combineInstruction, propertyName, compareMethod, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, Int32 length, string xmlPath, OperandType combineInstruction, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(type, dataType, length, xmlPath, combineInstruction, propertyName, null, compareMethod, value);
        }

        public FilterCriteria AddXmlFilter(Type type, Type dataType, Int32 length, string xmlPath, OperandType combineInstruction, string propertyName, string workTypeName, ComparisonMethod compareMethod, object value)
        {
            // Create a new SelectionCriterion object.
            SelectionCriterion selectionCriterion = new SelectionCriterion(combineInstruction, type, dataType, length, xmlPath, propertyName, workTypeName, compareMethod, value);
            m_Criteria.AddCriteria(selectionCriterion);
            ValidateCriteria(m_Criteria);
            return this;
        }

        public FilterCriteria AddSelectionCriterion(SelectionCriterion newSelectionCriterion)
        {
            SelectionCriterion selectionCriterion = new SelectionCriterion(newSelectionCriterion);
            m_Criteria.AddCriteria(selectionCriterion);
            ValidateCriteria(m_Criteria);
            return this;
        }

        public bool ContainsProperty(string propertyName)
        {
            foreach (SelectionCriterion sc in m_Criteria.Criteria)
            { 
                if (sc == null)
                    continue;
                if (sc.OriginalPropertyName == propertyName)
                    return true; 
            }
            return false; 
        }

        /// <summary>
        /// Validates the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void ValidateCriteria(PersistentCriteria criteria)
        {
            for (int i = 0; i < criteria.Criteria.Count; i++)
            {
                if (criteria.Criteria[i] is SelectionCriterion)
                    ValidateSelectionCriterion(criteria.Criteria);
                else
                    ValidateCriteria((PersistentCriteria)criteria.Criteria[i]);
            }
        }

        /// <summary>
        /// Validates the selection criterion.
        /// </summary>
        /// <param name="selections">The selections.</param>
        private void ValidateSelectionCriterion(ArrayList selections)
        {
            int cOperandTypes = 0;
            int cSelectionCriteria = selections.Count;
            for (int i = 0; i < selections.Count; i++)
            {
                if (selections[i] is SelectionCriterion)
                {
                    if (((SelectionCriterion)selections[i]).OperandType == OperandType.None && ((SelectionCriterion)selections[i]).Expression == Expression.None)
                        cOperandTypes++;
                }
                else
                    cSelectionCriteria--;
            }
            if (cSelectionCriteria == 1)
                return;

            if (cSelectionCriteria == cOperandTypes)
                throw new ArgumentException("Cannot have multiple filter values with each having an OperandType of None.");
        }

        /// <summary>
        /// Creates a new parenthetical expression for the WHERE clause.
        /// This expression will be combined with the previous expression's
        /// criteria by the OperandType specified in the next call to
        /// AddFilter.
        /// </summary>
        public void NewExpression()
        {
            if ((Criteria.Criteria != null) && (Criteria.Criteria.Count > 0))
            {
                PersistentCriteria newPC = new PersistentCriteria();
                newPC.AddCriteria(m_Criteria);
                m_Criteria = newPC;
            }
        }

        public override string ToString()
        {
            return string.Format("FilterCriteria {0}{1}", Environment.NewLine, m_Criteria.ToString());
        }
        #endregion
    }

    public enum Expression
    {
        /// <summary>
        /// There is no operation run on the previous criteria and the new criteria.
        /// </summary>
        None,
        /// <summary>
        /// Perform an And operation on the previous criteria and the new criteria.
        /// </summary>
        Begin,
        /// <summary>
        /// Perform an Or operation on the previous criteria and the new criteria.
        /// </summary>
        End
    }


    public class FilterCriteria<T> : FilterCriteria where T : class
    {
        public FilterCriteria()
            : base()
        {
        }



        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(string propertyName, object value)
        {
            return AddXmlFilter(typeof(T), null, 0, string.Empty, OperandType.None, propertyName, ComparisonMethod.Equals, value);
        }

      

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="combineInstruction">The combine instruction.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(OperandType combineInstruction, string propertyName, object value)
        {
            return AddXmlFilter(typeof(T), null, 0, string.Empty, combineInstruction, propertyName, ComparisonMethod.Equals, value);
        }

      

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="compareMethod">The compare method.</param>
        /// <param name="value">The value.</param>
        public FilterCriteria AddFilter(string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(typeof(T), null, 0, string.Empty, OperandType.None, propertyName, compareMethod, value);
        }

       

        /// <summary>
        /// Adds single criterion to the filter expression.
        /// </summary>
        /// <param name="combineInstruction">The combine instruction.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="compareMethod">The compare method.</param>
        /// <param name="value">The value.</param>
        /// 

        public FilterCriteria AddFilter(OperandType combineInstruction, string propertyName, ComparisonMethod compareMethod, object value)
        {
            return AddXmlFilter(typeof(T), null, 0, string.Empty, combineInstruction, propertyName, compareMethod, value);
        }
        
    }



}