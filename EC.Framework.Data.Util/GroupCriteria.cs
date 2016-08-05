using System;
using System.Collections;

namespace EC.Framework.Data
{
    /// <summary>
    /// GroupCriteria provides a generic grouping mechanism for
    /// business object collections. 
    /// </summary>
    public class GroupCriteria
    {
        #region Properties
        private ArrayList m_GroupCriteriaList = new ArrayList();

        /// <summary>
        /// Gets the group criteria list.
        /// </summary>
        /// <value>The group criteria list.</value>
        public ArrayList GroupCriteriaList
        {
            get { return m_GroupCriteriaList; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriteria"/> class.
        /// </summary>
        public GroupCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriteria"/> class.
        /// </summary>
        /// <param name="groupCriterionOriginal">The group criterion original.</param>
        public GroupCriteria(GroupCriteria groupCriterionOriginal)
        {
            foreach (GroupCriterion groupCriterion in groupCriterionOriginal.GroupCriteriaList)
                m_GroupCriteriaList.Add(groupCriterion.Clone());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        public void AddGroup(Type type, string propertyName)
        {
            GroupCriterion groupCriterion = new GroupCriterion(type, propertyName);
            m_GroupCriteriaList.Add(groupCriterion);
        }

        /// <summary>
        /// Clears the groups.
        /// </summary>
        public void ClearGroups()
        {
            m_GroupCriteriaList.Clear();
        }
        #endregion
    }
}