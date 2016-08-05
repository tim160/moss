using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using EC.Framework.Data;

namespace EC.Business.Entities
{
    /// <summary>
    /// Setting class
    /// </summary>
    [TableAttribute("Setting")]
    public class Setting
    {
        #region Base
        private Guid m_ObjectKey = Guid.NewGuid();
        private DateTime m_Timestamp = new DateTime();
        private Int64 m_Id = 0;
        private DataAction m_ObjectState = DataAction.Insert;

        /// <summary>
        /// Gets or sets the object key.
        /// </summary>
        /// <value>The object key.</value>
        public virtual Guid ObjectKey
        {
            get { return m_ObjectKey; }
            set { m_ObjectKey = value; }
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp
        {
            get { return m_Timestamp; }
            set { m_Timestamp = value; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Int64 Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        /// <summary>
        /// Gets or sets the state of the object.
        /// </summary>
        /// <value>The state of the object.</value>
        public DataAction ObjectState
        {
            get { return m_ObjectState; }
            set { m_ObjectState = value; }
        }
        #endregion Base

        #region Properties
        private Guid m_ForeignKey = Guid.Empty;
        private XmlDocument m_SettingData = new XmlDocument();

        public Guid ForeignKey
        {
            get { return m_ForeignKey; }
            set
            {
                m_ForeignKey = value;
                SetObjectState();
            }
        }

        public XmlDocument SettingData
        {
            get { return m_SettingData; }
            set
            {
                m_SettingData = value;
                SetObjectState();
            }
        }
        #endregion

        #region Methods
        public Setting()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            m_SettingData.LoadXml("<Setting></Setting>");
        }

        /// <summary>
        /// Sets the state of the object.
        /// </summary>
        private void SetObjectState()
        {
            if (m_ObjectKey == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                m_ObjectKey = Guid.NewGuid();
                m_ObjectState = DataAction.Insert;
            }
            if (m_ObjectState == DataAction.None)
            {
                m_ObjectState = DataAction.Update;
            }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            m_ObjectState = DataAction.Delete;
        }
        #endregion
    }
}
