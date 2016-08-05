using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Framework.Data;

namespace EC.Business.Entities
{
    /// <summary>
    /// File class.
    /// </summary>
    [TableAttribute("Attachment")]
    public class Attachment
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
        public Guid ForeignKey
        {
            get { return m_ForeignKey; }
            set
            {
                m_ForeignKey = value;
                SetObjectState();
            }
        }

        private string m_Filename = string.Empty;
        public string Filename
        {
            get { return m_Filename; }
            set
            {
                m_Filename = value;
                SetObjectState();
            }
        }

        private string m_Type = string.Empty;
        public string Type
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                SetObjectState();
            }
        }	

        private Byte[] m_Data = new Byte[0];
        public Byte[] Data
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
                SetObjectState();
            }
        }
		
		private string m_Meta = string.Empty;
		public string Meta
        {
            get { return m_Meta; }
            set
            {
                m_Meta = value;
                SetObjectState();
            }
        }
        #endregion

        #region Methods
        public Attachment()
        {
            Initialize();
        }

        public Attachment(Guid foreignKey, String fileName, String mimeType, Byte[] data, String meta)
        {
            Initialize();

            ForeignKey = foreignKey;
            Filename = fileName;
            Type = mimeType;
            Data = data;
            Meta = meta ?? String.Empty;
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
