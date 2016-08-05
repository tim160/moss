using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EC.Framework.Data;
using EC.Common.Util;
using EC.Business.Actions;

namespace EC.Business.Entities
{
    [Serializable]
    [TableAttribute("SortQueryCriteria")]
    public class SortQueryCriteria : BaseEntity
    {
        XmlDocument m_DataXml = new XmlDocument();
        string m_Name = string.Empty;
        string m_QueryCriteriaType = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public XmlDocument DataXml
        {
            get { return m_DataXml; }
            set { m_DataXml = value; }
        }

        public long? UserId
        {
            get;
            set; 
        }

        public long? CreatorId
        {
            get;
            set; 
        }

        public string QueryCriteriaType
        {
            get { return m_QueryCriteriaType; }
            set { m_QueryCriteriaType = value; }
        }

        public SortQueryCriteria()
        {
        }

        public SortQueryCriteria(SortCriteria qc, long creatorId)
        {
            m_ObjectKey = qc.ObjectKey;
            m_Name = qc.Name;
            UserId = qc.UserId;
            CreatorId = creatorId;
            m_QueryCriteriaType = qc.SortCriteriaType.ToString();
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(SortCriteria));
            serializer.Serialize(writer, qc);
            m_DataXml.LoadXml(writer.ToString());
            writer.Close();
        }

        public SortCriteria AsQueryCriteria()
        {
            StringReader reader = new StringReader(m_DataXml.InnerXml);
            XmlSerializer serializer = new XmlSerializer(typeof(SortCriteria));
            SortCriteria qc = (SortCriteria)serializer.Deserialize(reader);
            reader.Close();
            return qc;
        }

        #region IBusinessGroupMember Members

       
        public string GetName()
        {
            return Name;
        }

        public List<Common.Util.ReturnProblem> Validate(long? actorId)
        {
            List<ReturnProblem> rp = new List<ReturnProblem>();
            return rp;
        }

        public Common.Util.ReturnProblem ValidateAgainstParents()
        {
            return null; 
        }

        #endregion
    }
}
