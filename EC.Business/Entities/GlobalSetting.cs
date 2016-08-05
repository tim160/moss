using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Framework.Data;

namespace EC.Business.Entities
{
    /// <summary>
    /// Area class
    /// </summary>
    [TableAttribute("GlobalSetting")]
    public class GlobalSetting
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
        private string m_Name = string.Empty;
        private string m_Value = string.Empty;

        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                SetObjectState();
            }
        }

        public string Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                SetObjectState();
            }
        }
        #endregion

        #region Methods
        public GlobalSetting()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
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
