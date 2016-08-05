using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Framework.Data
{
    /// <summary>
    /// SqlOrderByCriteria mutates the OrderCriteria to a SQL Order.
    /// </summary>
    public class SqlOrderByCriteria : OrderCriteria
    {
        #region Property(s)
        private string m_OrderBy = string.Empty;

        /// <summary>
        /// Gets the order by clause.
        /// </summary>
        /// <value>The order by clause.</value>
        public string OrderByClause
        {
            get
            {
                OrderBy = String.Empty;
                SetOrderByClause();
                return OrderBy;
            }
        }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        private string OrderBy
        {
            get { return m_OrderBy; }
            set { m_OrderBy = value; }
        }

        private bool m_Reverse = false;
        public bool Reverse
        {
            get { return m_Reverse; }
            set { m_Reverse = value; }
        }

        private string m_PersistentTypeName = null;
        public string PersistentTypeName
        {
            get { return m_PersistentTypeName; }
            set { m_PersistentTypeName = value; }
        }

        private DataObjectBroker m_DataObjectBroker = null;
        public DataObjectBroker DataObjectBroker
        {
            set { m_DataObjectBroker = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOrderByCriteria"/> class.
        /// </summary>
        /// <param name="dataObjectBroker">The data object broker.</param>
        /// <param name="orderCriteria">The order criteria.</param>
        public SqlOrderByCriteria(OrderCriteria orderCriteria, IObjectBroker dataObjectBroker)
            : base(orderCriteria)
        {
            m_DataObjectBroker = (DataObjectBroker)dataObjectBroker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOrderByCriteria"/> class.
        /// </summary>
        /// <param name="oc">The oc.</param>
        public SqlOrderByCriteria(SqlOrderByCriteria oc, IObjectBroker dataObjectBroker)
            : base(oc)
        {
            m_DataObjectBroker = (DataObjectBroker)dataObjectBroker;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the order by clause.
        /// </summary>
        private void SetOrderByClause()
        {
            int index = -1;
            OrderBy = string.Empty;

            foreach (OrderCriterion orderCriterion in base.OrderCriteriaList)
            {
                string persistentTypeName = AppInfo.GetTableName(orderCriterion.Type);

                index++;

                if (index > 0)
                    OrderBy += ", ";

                if (orderCriterion.OriginalPropertyName != "ObjectKey")
                {
                    if (orderCriterion.XmlPath == string.Empty)
                    {
                        string pTypeName = PersistentTypeName != null && !String.Empty.Equals(PersistentTypeName) ? PersistentTypeName : persistentTypeName;
                        string field = string.Format("{0}." + "{1}", pTypeName, orderCriterion.OriginalPropertyName);

                        if (orderCriterion.AggregateFunction == AggregateFunction.None)
                            OrderBy += field;
                        else
                            OrderBy += string.Format("{0}({1})", orderCriterion.AggregateFunction, field);
                    }
                    else
                    {
                        char[] delimiterChars = { '.' };
                        string[] xmlColumn = orderCriterion.OriginalPropertyName.Split(delimiterChars);
                        OrderBy += m_DataObjectBroker.ConnectionInfo.GetXmlOrder(persistentTypeName, xmlColumn, orderCriterion.XmlPath, GetDataType(orderCriterion));
                    }
                }
                else
                {
                    string field = persistentTypeName + "." + persistentTypeName + "Key";

                    if (orderCriterion.AggregateFunction == AggregateFunction.None)
                        OrderBy += field;
                    else
                        OrderBy += string.Format("{0}({1})", orderCriterion.AggregateFunction, field);
                }

                OrderBy += " ";

                if (m_Reverse)
                {
                    if (orderCriterion.SortOrder.ToUpper().Equals("ASC"))
                        orderCriterion.SortOrder = "DESC";
                    else
                        orderCriterion.SortOrder = "ASC";
                }

                OrderBy += orderCriterion.SortOrder;
            }

            if (OrderBy.Length > 0)
                OrderBy = "ORDER BY " + OrderBy;
        }


        private string HandleAggregateFunction(string name, AggregateFunction aggregateFunction)
        {
            return string.Format("{0}({1})", aggregateFunction, name);
        }

        /// <summary>
        /// Gets the column list.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public string GetColumnList(System.Type objectType)
        {
            int index = -1;
            string returnColumnList = String.Empty;

            foreach (OrderCriterion orderCriterion in base.OrderCriteriaList)
            {
                string persistentTypeName = AppInfo.GetTableName(orderCriterion.Type);
                //string persistentTypeName = ((TableAttribute[])orderCriterion.Type.GetCustomAttributes(typeof(TableAttribute), false))[0].TableName;

                if (orderCriterion.Type != objectType)
                {
                    index++;
                    if (index > 0)
                        returnColumnList += ", ";

                    returnColumnList += persistentTypeName + ".";

                    if (orderCriterion.OriginalPropertyName != "ObjectKey")
                        returnColumnList += orderCriterion.OriginalPropertyName;
                    else
                        returnColumnList += persistentTypeName + "Key";
                }
            }

            return returnColumnList;
        }

        public string GetXmlColumnList()
        {
            int index = -1;
            StringBuilder str = new StringBuilder();

            foreach (OrderCriterion orderCriterion in base.OrderCriteriaList)
            {
                string persistentTypeName = AppInfo.GetTableName(orderCriterion.Type);
                //string persistentTypeName = ((TableAttribute[])orderCriterion.Type.GetCustomAttributes(typeof(TableAttribute), false))[0].TableName;

                if (orderCriterion.DataType != null)
                {
                    index++;
                    if (index > 0)
                        str.Append(",");

                    char[] delimiterChars = { '.' };
                    string[] xmlColumn = orderCriterion.OriginalPropertyName.Split(delimiterChars);

                    str.AppendFormat("{0}.{1}.value('(/{2}{3})[1]', '{4}') {5}",
                        persistentTypeName,
                        xmlColumn[0],
                        orderCriterion.XmlPath,
                        xmlColumn[1],
                        GetDataType(orderCriterion),
                        orderCriterion.OriginalPropertyName);
                }
            }

            return str.ToString();
        }

        public Dictionary<string, string> GetCreateXmlColumns(Dictionary<string, string> createColumns)
        {
            foreach (OrderCriterion orderCriterion in base.OrderCriteriaList)
            {
                if (orderCriterion.DataType != null)
                {
                    char[] delimiterChars = { '.' };
                    string[] xmlColumn = orderCriterion.OriginalPropertyName.Split(delimiterChars);
                    createColumns.Add(xmlColumn[1], GetDataType(orderCriterion));
                }
            }

            return createColumns;
        }

        private string GetDataType(OrderCriterion orderCriterion)
        {
            if (orderCriterion.DataType == typeof(string))
            {
                if (orderCriterion.Length > 0 && orderCriterion.Length < 4000)
                    return string.Format("NVARCHAR({0})", orderCriterion.Length);
                else
                    return "NVARCHAR(max)";
            }
            else if (orderCriterion.DataType == typeof(DateTime))
                return "DATETIME";
            else if (orderCriterion.DataType == typeof(Guid))
                return "UNIQUEIDENTIFIER";
            else if (orderCriterion.DataType == typeof(Int64))
                return "BIGINT";
            else if (orderCriterion.DataType == typeof(Int32))
                return "INTEGER";
            else if (orderCriterion.DataType == typeof(Int16))
                return "SMALLINT";
            else if (orderCriterion.DataType == typeof(Decimal))
                return "FLOAT";
            else if (orderCriterion.DataType == typeof(Double))
                return "FLOAT";
            else if (orderCriterion.DataType == typeof(Boolean))
                return "BIT";

            return "NVARCHAR(max)";
        }
        #endregion
    }
}