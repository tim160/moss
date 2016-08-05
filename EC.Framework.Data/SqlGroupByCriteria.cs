using System;
//using EC.Framework.Data.Interfaces;

namespace EC.Framework.Data
{
    /// <summary>
    /// SqlGroupByCriteria mutates the GroupCriteria to a SQL Group.
    /// </summary>
    public class SqlGroupByCriteria : GroupCriteria
    {
        #region Property(s)
        private string m_GroupBy = string.Empty;

        /// <summary>
        /// Gets the group by clause.
        /// </summary>
        /// <value>The group by clause.</value>
        public string GroupByClause
        {
            get
            {
                GroupBy = String.Empty;
                SetGroupByClause();
                return GroupBy;
            }
        }

        /// <summary>
        /// Gets or sets the group by.
        /// </summary>
        /// <value>The group by.</value>
        private string GroupBy
        {
            get { return m_GroupBy; }
            set { m_GroupBy = value; }
        }

        private DataObjectBroker m_DataObjectBroker = null;
        public DataObjectBroker DataObjectBroker
        {
            set { m_DataObjectBroker = value; }
        }
        #endregion

        #region Constructor
        public SqlGroupByCriteria(GroupCriteria groupCriteria, IObjectBroker dataObjectBroker)
            : base(groupCriteria)
        {
            m_DataObjectBroker = (DataObjectBroker)dataObjectBroker;
        }

        public SqlGroupByCriteria(SqlGroupByCriteria oc, IObjectBroker dataObjectBroker)
            : base(oc)
        {
            m_DataObjectBroker = (DataObjectBroker)dataObjectBroker;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the group by clause.
        /// </summary>
        private void SetGroupByClause()
        {
            int iIndex = -1;
            string nameFormat = m_DataObjectBroker.ConnectionInfo.NameFormat();

            GroupBy = string.Empty;

            foreach (GroupCriterion groupCriterion in base.GroupCriteriaList)
            {
                string persistentTypeName = AppInfo.GetTableName(groupCriterion.Type);
                //string persistentTypeName = ((TableAttribute[])groupCriterion.Type.GetCustomAttributes(typeof(TableAttribute), false))[0].TableName;
                iIndex++;

                if (iIndex > 0)
                {
                    GroupBy += ", ";
                }

                GroupBy += string.Format(nameFormat, persistentTypeName) + ".";

                if (groupCriterion.OriginalPropertyName != "ObjectKey")
                {
                    GroupBy += string.Format(nameFormat, groupCriterion.OriginalPropertyName);
                }
                else
                {
                    GroupBy += string.Format(nameFormat, persistentTypeName + "Key");
                }
            }

            if (GroupBy.Length > 0)
            {
                GroupBy = " GROUP BY " + GroupBy;
            }
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
            foreach (GroupCriterion groupCriterion in base.GroupCriteriaList)
            {
                string persistentTypeName = AppInfo.GetTableName(groupCriterion.Type);
                //string persistentTypeName = ((TableAttribute[])groupCriterion.Type.GetCustomAttributes(typeof(TableAttribute), false))[0].TableName;

                if (groupCriterion.Type != objectType)
                {
                    index++;
                    if (index > 0)
                    {
                        returnColumnList += ", ";
                    }

                    returnColumnList += persistentTypeName + ".";

                    if (groupCriterion.OriginalPropertyName != "ObjectKey")
                    {
                        returnColumnList += groupCriterion.OriginalPropertyName;
                    }
                    else
                    {
                        returnColumnList += persistentTypeName + "Key";
                    }
                }
            }
            return returnColumnList;
        }
        #endregion
    }
}