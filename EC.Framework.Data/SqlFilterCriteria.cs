using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EC.Framework.Logger;

namespace EC.Framework.Data
{
    /// <summary>
    /// SqlFilterCriteria mutates the FilterCriteria to a SQL Filter.
    /// </summary>
    public class SqlFilterCriteria : FilterCriteria
    {
        #region Property(s)
        private static readonly ICustomLog m_Log = CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string m_Where = string.Empty;
        private string m_WhereForParameters = string.Empty;
        private List<object> m_Parameters = new List<object>();
        private List<Type> m_Types = new List<Type>(); 

        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <value>The where clause.</value>
        public string WhereClause
        {
            get
            {
                m_Where = String.Empty;
                SetWhereClause(Criteria, ref m_Where, false);
                return m_Where;
            }
        }

        public string WhereClauseForParameters
        {
            get
            {
                m_WhereForParameters = String.Empty;
                SetWhereClause(Criteria, ref m_WhereForParameters, true);
                return m_WhereForParameters;
            }
        }

        private SelectionCriterion m_PreviousSelectionCriterion = null;
        private DataObjectBroker m_DataObjectBroker = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFilterCriteria"/> class.
        /// </summary>
        /// <param name="dataObjectBroker">The data object broker.</param>
        public SqlFilterCriteria(IObjectBroker dataObjectBroker)
        {
            m_DataObjectBroker = (DataObjectBroker)dataObjectBroker;
        }
        #endregion

        #region Methods
        public void AddParameters(IDbCommand command)
        {
//#if (DEBUG)
//            string values = string.Empty;
//#endif
            for (int i = 0; i < m_Parameters.Count; i++)
            {
                if (m_Parameters[i].GetType() != typeof(bool))
                {
                    command.Parameters.Add(m_DataObjectBroker.ConnectionInfo.GetParameter("p" + i.ToString(), m_Parameters[i]));
                }
                else
                {
                    if (m_Parameters[i].ToString().ToUpper() == "TRUE")
                    {
                        command.Parameters.Add(m_DataObjectBroker.ConnectionInfo.GetParameter("p" + i.ToString(), "1"));
                    }
                    else
                    {
                        command.Parameters.Add(m_DataObjectBroker.ConnectionInfo.GetParameter("p" + i.ToString(), "0"));
                    }
                }
//#if (DEBUG)
//                if (m_Parameters[i].GetType() != typeof(DateTime))
//                {
//                    values += string.Format("'{0}',", m_Parameters[i].ToString());
//                }
//                else
//                {
//                    values += string.Format("'{0}',", ((DateTime)m_Parameters[i]).ToString("s"));
//                }
//#endif
            }
//#if (DEBUG)
//            m_Log.Debug(values.Substring(0, values.Length - 1));
//#endif
        }

        public List<Type> GetFilterTypes()
        {
            return m_Types;
        }

        /// <summary>
        /// Sets the where clause.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void SetWhereClause(PersistentCriteria criteria, ref string where, bool forParameters)
        {
            SelectionCriterion selectionCriterion;
            PersistentCriteria persistentCriteria;

            where += " " + OperandText(criteria.OperandType) + "(";

            foreach (object o in criteria.Criteria)
            {
                selectionCriterion = o as SelectionCriterion;
                if (selectionCriterion != null)
                {
                    where += SelectionClause(selectionCriterion, forParameters);
                    continue;
                }
                persistentCriteria = o as PersistentCriteria;
                if (persistentCriteria != null)
                {
                    SetWhereClause(persistentCriteria, ref where, forParameters);
                    continue;
                }
            }

            // Close paren for this criteria
            where += ") ";
            if (!where.StartsWith("WHERE"))
            {
                if (where == "  ()")
                {
                    where = "";
                }
                else
                {
                    where = "WHERE" + where;
                }
            }
        }

        /// <summary>
        /// Selections the clause.
        /// </summary>
        /// <param name="selectionCriterion">The selectionCriterion.</param>
        /// <returns></returns>
        private string SelectionClause(SelectionCriterion selectionCriterion, bool forParameters)
        {
            string result = string.Empty;

            if (selectionCriterion.Expression == Expression.None)
            {
                if (selectionCriterion == null)
                {
                    throw new NullReferenceException("SelectionCriterion cannot be null!");
                }
                if (!m_Types.Contains(selectionCriterion.Type))
                {
                    m_Types.Add(selectionCriterion.Type);
                }
                selectionCriterion.PropertyAlias = AppInfo.GetTableName(selectionCriterion.Type) + ".";
                if (selectionCriterion.OriginalPropertyName != "ObjectKey")
                {
                    selectionCriterion.PropertyAlias += selectionCriterion.OriginalPropertyName;
                }
                else
                {
                    selectionCriterion.PropertyAlias += AppInfo.GetTableName(selectionCriterion.Type) + "Key";
                }
                string expression = string.Empty;
                if (m_PreviousSelectionCriterion != null && m_PreviousSelectionCriterion.Expression == Expression.Begin)
                {
                    expression = "(";
                }
                if (forParameters)
                {
                    object value = selectionCriterion.Value;
                    if (selectionCriterion.ComparisonMethod == ComparisonMethod.In || selectionCriterion.ComparisonMethod == ComparisonMethod.NotIn)
                    {
                        if (value != null)
                        {
                            object obj = value;
                            Array array = obj as Array;
                            object[] objs = new object[array.Length];
                            int i = 0;
                            foreach (var item in array)
                            {
                                objs[i++] = item;
                            }
                            value = objs;
                        }
                        else
                        {
                            throw new InvalidOperationException("ComparisonMethod.In/ComparisonMethod.NotIn cannot have a null object value");
                        }
                    }
                    string text = ComparisonTextForParameters(m_DataObjectBroker, selectionCriterion.ComparisonMethod, ref value, m_Parameters.Count);
                    if (value != null)
                    {
                        if (selectionCriterion.ComparisonMethod != ComparisonMethod.In && selectionCriterion.ComparisonMethod != ComparisonMethod.NotIn)
                        {
                            m_Parameters.Add(value);
                        }
                        else
                        {
                            for (int i = 0; i < ((object[])value).Length; i++)
                            {
                                m_Parameters.Add(((object[])value)[i]);
                            }
                        }
                    }
                    result = OperandText(selectionCriterion.OperandType) + " " + expression + ColumnAlias(selectionCriterion) + " " + text + " ";
                }
                else
                {
                    result = OperandText(selectionCriterion.OperandType) + " " + expression +
                        ColumnAlias(selectionCriterion) + " " +
                        ComparisonText(m_DataObjectBroker, selectionCriterion.ComparisonMethod, selectionCriterion.Value, selectionCriterion.Property.PropertyType) + " ";
                }
            }
            else if (selectionCriterion.Expression == Expression.Begin)
            {
                result = string.Empty;
            }
            else if (selectionCriterion.Expression == Expression.End)
            {
                result = ") ";
            }
            else
            {
                throw new ApplicationException("Could not generate where clause!");
            }
            m_PreviousSelectionCriterion = selectionCriterion;
            return result;
        }

        /// <summary>
        /// Columns the alias.
        /// </summary>
        /// <param name="selectionCriterion">The selectionCriterion.</param>
        /// <returns></returns>
        private string ColumnAlias(SelectionCriterion selectionCriterion)
        {
            string str = string.Empty;
            if ((selectionCriterion == null) || (selectionCriterion.Property == null))
            {
                throw new NullReferenceException("SelectionCriterion cannot be null!");
            }
            if (selectionCriterion.DataType == null)
            {
                str = selectionCriterion.PropertyAlias;
            }
            else
            {
                char[] delimiterChars = { '.' };
                string[] xmlColumn = selectionCriterion.OriginalPropertyName.Split(delimiterChars);
                string persistentTypeName = AppInfo.GetTableName(selectionCriterion.Type);
                str = m_DataObjectBroker.ConnectionInfo.GetColumnAlias(persistentTypeName, xmlColumn, selectionCriterion.XmlPath, GetDataType(selectionCriterion), selectionCriterion);
            }
            return str;
        }

        /// <summary>
        /// Operands the text.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private string OperandText(OperandType operand)
        {
            string operandString = String.Empty;

            switch (operand)
            {
                case OperandType.And:
                    {
                        operandString = "AND";
                        break;
                    }
                case OperandType.Or:
                    {
                        operandString = "OR";
                        break;
                    }
                case OperandType.None:
                    {
                        operandString = "";
                        break;
                    }
            }

            return operandString;
        }

        /// <summary>
        /// Gets the value string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetValueString(DataObjectBroker dob, object value)
        {
            string valueString = String.Empty;
            if (value == null)
            {
                valueString = "NULL";
            }
            else if (value.GetType() == typeof(bool))
            {
                if (value.ToString() == "True")
                {
                    valueString = "1";
                }
                else
                {
                    valueString = "0";
                }
            }
            else if (value.GetType() == typeof(decimal))
            {
                valueString = ((decimal)value).ToString(new System.Globalization.CultureInfo("en-US"));
            }
            else if (value.GetType() == typeof(double))
            {
                valueString = ((double)value).ToString(new System.Globalization.CultureInfo("en-US"));
            }
            else if (value.GetType() == typeof(DateTime))
            {
                valueString = dob.ConnectionInfo.FormatDate(value);
            }
            else
            {
                valueString = value.ToString();
            }
            return valueString;
        }

        /// <summary>
        /// Comparisons the text.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        //private string ComparisonText(ComparisonMethod method, object value)
        public static string ComparisonText(DataObjectBroker dob, ComparisonMethod method, object value, Type type)
        {
            return dob.ConnectionInfo.ComparisonText(method, GetValueString(dob, value), type); 
        }

        public static string ComparisonTextForParameters(DataObjectBroker dob, ComparisonMethod comparisonMethod, ref object value, int counter)
        {
            return dob.ConnectionInfo.ComparisonTextForParameters(comparisonMethod, ref value, counter);
        }

        private string GetDataType(SelectionCriterion selectionCriterion)
        {
            return m_DataObjectBroker.ConnectionInfo.GetDataType(selectionCriterion);
        }
        #endregion
    }
}