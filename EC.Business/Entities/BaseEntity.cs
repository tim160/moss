using System.Linq;
using EC.Common.Util;
using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Xml.Serialization; 
using EC.Framework.Data;
using System.Reflection; 
using EC.Common.Base;
using System.Collections.Generic;
using EC.Business.Actions;


namespace EC.Business.Entities
{
    [Serializable]
    public abstract class BaseEntity : IBaseEntity
    {
        protected Guid m_ObjectKey = GuidTools.SequentialGuid();
        protected DateTime m_Timestamp = new DateTime();
        protected Int64 m_Id = 0;
        protected DataAction m_ObjectState = DataAction.Insert;
        IObjectBroker m_ObjectBroker;


        public Int64 Id
        {
            get { return m_Id; }
            set
            {
                m_Id = value;
                //   if (IdUpdated != null)
                //    {
                //      IdUpdated(new IdEventArgs { Id = value });
                // }
            }
        }

        public virtual Guid ObjectKey
        {
            get { return m_ObjectKey; }
            set { m_ObjectKey = value; }
        }

        public DateTime Timestamp
        {
            get { return m_Timestamp; }
            set { m_Timestamp = value; }
        }
        
        public DataAction ObjectState
        {
            get { return m_ObjectState; }
            set { m_ObjectState = value; }
        }

        public BaseEntity()
        {
        }

        public BaseEntity(Guid objectKey, Int64 id, DateTime ts)
        {
            m_ObjectKey = objectKey;
            m_Id = id;
            m_Timestamp = ts;
        }

        public void SetObjectState()
        {
            if (m_ObjectKey == Guid.Empty)
            {
                m_ObjectKey = GuidTools.SequentialGuid();
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
        public virtual void Remove()
        {
            m_ObjectState = DataAction.Delete;
        }

        public event EventHandler<ObjectKeyUpdatedArgs> ObjectKeyUpdated;

        public void InitSequentialKey()
        {
            ObjectKey = GuidTools.SequentialGuid();
            ObjectKeyUpdated(this, new ObjectKeyUpdatedArgs { ObjectKey = ObjectKey });
        }

        public virtual void ObjectKeyUpdatedHandler(object sender, ObjectKeyUpdatedArgs e)
        {
            PropertyInfo pi = this.GetType().GetProperty("EntityKey");
            if (pi != null)
                pi.SetValue(this, e.ObjectKey, null);
        }
    }
}