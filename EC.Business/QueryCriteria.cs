using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;

using EC.Business.Entities;

using System.Xml.Serialization;
using EC.Framework.Data;

namespace EC.Business
{
    /// <summary>
    /// Used to deliver sort information 
    /// </summary>
    public class QueryCriteria 
    {
        Guid m_ObjectKey = Guid.Empty;
        string m_Name = string.Empty;

        public int Version { get; set; }
        /// <summary>
        /// The unique object key of this QueryCriteria if it is to be persisted
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
        /// The name of this QueryCriteria if it is persisted
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
        /// Used for WorkSpace Named Filter management.  Ids of business groups the filter is shared with.
        /// </summary>
        public string[] BGIds { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Ids of users the filter is shared with.
        /// </summary>
        public string[] UserIds { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Filter is shared with business group(s).
        /// </summary>
        public bool IsSharedBG { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Filter is shared with one or more user(s).
        /// </summary>
        public bool IsSharedUser { get; set; }

        /// <summary>
        /// The user that created this QueryCriteria
        /// </summary>
        public long? UserId
        {
            get;
            set; 
        }

      
        public bool FilterValueListOnDisplay
        {
            get;
            set;
        }

        /// <summary>
        /// Empty QueryCriteria constructor
        /// </summary>
        public QueryCriteria()
        {
        }

        /// <summary>
        /// Does not copy BusinessGroupAssociations
        /// </summary>
        /// <returns></returns>
        public QueryCriteria DeepCopy()
        {
            QueryCriteria other = (QueryCriteria)MemberwiseClone();
          
            return other;
        }

    }

    

    
}
