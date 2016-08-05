using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Business.Entities;
using EC.Framework.Data;

namespace EC.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class SortCriteria
    {
        List<Sort> m_Sorts = new List<Sort>();


        Guid m_ObjectKey = Guid.Empty;
        string m_Name = string.Empty;
        SortCriteriaType m_QueryCriteriaType = SortCriteriaType.Undefined;

        /// <summary>
        /// 
        /// </summary>
        public Guid ObjectKey
        {
            get
            {
                return m_ObjectKey;
            }
            set
            {
                m_ObjectKey = value;
            }
        }

        public long Id
        {
            get
            {
                return default(long); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Used for WorkSpace Named Sort management.  Ids of business groups the sort is shared with.
        /// </summary>
        public string[] BGIds { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Sort management.  Ids of users the sort is shared with.
        /// </summary>
        public string[] UserIds { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Sort management.  Sort is shared with business group(s).
        /// </summary>
        public bool IsSharedBG { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Sort management.  Sort is shared with one or more user(s).
        /// </summary>
        public bool IsSharedUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? UserId
        {
            get;
            set; 
        }

        /// <summary>
        /// 
        /// </summary>
        public SortCriteriaType SortCriteriaType
        {
            get
            {
                return m_QueryCriteriaType;
            }
            set
            {
                m_QueryCriteriaType = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<Sort> Sorts
        {
            get
            {
                return m_Sorts;
            }
            set
            {
                m_Sorts = value;
            }
        }

        public bool SortValueListOnDisplay
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public SortCriteria()
        {
            m_Sorts = new List<Sort>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSort(string field, SortDirection direction)
        {
            m_Sorts.Add(new Sort(field, direction)); 
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int i = 0; 
            foreach (Sort sort in Sorts)
            {
                builder.Append(sort.ToString());

                if (i++ < m_Sorts.Count - 1)
                    builder.Append(", "); 
                
            }
            return builder.ToString(); 
        }

        /// <summary>
        /// Does not copy BusinessGroupAssociations
        /// </summary>
        /// <returns></returns>
        public SortCriteria DeepCopy()
        {
            SortCriteria other = (SortCriteria)MemberwiseClone();
            List<Sort> sorts = new List<Sort>();
            foreach (Sort s in Sorts)
            {
                sorts.Add(s.DeepCopy());
            }
            other.Sorts = sorts;
            return other;
        }

        #region IBusinessGroupMember Members

        EC.Framework.Data.DataAction m_ObjectState = DataAction.None;
        public EC.Framework.Data.DataAction ObjectState
        {
            get 
            {
                return m_ObjectState;
            }
        }


        public string GetName()
        {
            return Name;
        }

        public List<Common.Util.ReturnProblem> Validate(long? actorId)
        {
            return new List<Common.Util.ReturnProblem>(); 
        }

        public Common.Util.ReturnProblem ValidateAgainstParents()
        {
            return null; 
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class Sort
    {
        string m_Field = string.Empty;
        SortDirection m_Direction = SortDirection.ASC;
        SortCriteriaType m_SortCriteriaType = SortCriteriaType.Undefined;

        /// <summary>
        /// 
        /// </summary>
        public string Field
        {
            get
            {
                return m_Field;
            }
            set
            {
                m_Field = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SortDirection Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Sort()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Sort(string field, SortDirection direction)
        {
            m_Field = field;
            m_Direction = direction; 
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} {1}", m_Field, m_Direction); 
        }

        /// <summary>
        /// 
        /// </summary>
        public SortCriteriaType SortCriteriaType
        {
            get
            {
                return m_SortCriteriaType;
            }
            set
            {
                m_SortCriteriaType = value;
            }
        }


        /// <summary>
        /// Creates a deep copy of this Sort
        /// </summary>
        /// <returns>Sort</returns>
        public Sort DeepCopy()
        {
            Sort other = (Sort)MemberwiseClone();            
            return other;
        }
    }

    /// <summary>
    /// Sort direction for sort criteria
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// No sort applied
        /// </summary>
        NONE,
        /// <summary>
        /// Ascending sort applied
        /// </summary>
        ASC, 
        /// <summary>
        /// Descending sort applied
        /// </summary>
        DESC
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SortCriteriaType
    {
        /// <summary>
        /// 
        /// </summary>
        Undefined,
        /// <summary>
        /// 
        /// </summary>
        Order
    }
}
