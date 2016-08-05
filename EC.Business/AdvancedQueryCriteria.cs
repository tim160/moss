using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using EC.Business.Actions;
using EC.Business.Entities;
using EC.Framework.Data;

namespace EC.Business
{
    public class AdvancedQueryCriteria
    {
              /// <summary>
        /// Used for WorkSpace Named Filter management.  Ids of business groups the filter is shared with.
        /// </summary>
        public string[] BGIds { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Ids of users the filter is shared with.
        /// </summary>
        public string[] UserIds { get; set; }

       public AdvancedQuery[] Queries { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Filter is shared with business group(s).
        /// </summary>
        public bool IsSharedBG { get; set; }

        /// <summary>
        /// Used for WorkSpace Named Filter management.  Filter is shared with one or more user(s).
        /// </summary>
        public bool IsSharedUser { get; set; }


        public string Name { get; set; }
        public Guid ObjectKey { get; set; }
        public long? UserId { get; set; }
        public string QueryType { get; set; }

      

        public AdvancedQueryCriteria DeepCopy()
        {
            AdvancedQueryCriteria other = (AdvancedQueryCriteria)MemberwiseClone();
            if (Queries != null)
            {
                List<AdvancedQuery> otherQueries = new List<AdvancedQuery>();
                foreach (AdvancedQuery q in Queries)
                {
                    otherQueries.Add(q.DeepCopy());
                }
                other.Queries = otherQueries.ToArray();
            }
            return other;
        }

        public XmlDocument ToXml()
        {
            XmlDocument advancedFilterXml = new XmlDocument();
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AdvancedQueryCriteria));
                serializer.Serialize(writer, this);
                advancedFilterXml.LoadXml(writer.ToString());
            }
            return advancedFilterXml;
        }

           
    }
}