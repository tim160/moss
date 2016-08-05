using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Framework.Data;
using EC.Business.Actions;
using EC.Common;
using EC.Common.Util;

namespace EC.Business.Entities
{
    /// <summary>
    /// LoginLog class
    /// </summary>
    [TableAttribute("LoginLog")]
    public class LoginLog
    {
        #region Base
        private DateTime m_Timestamp = DateTime.UtcNow;
        private Int64 m_Id = 0;
        private DataAction m_ObjectState = DataAction.Insert;

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

        #region Property(s)
        private int m_EventTypeId;
        private Int64? m_PermissionId;
        private bool m_Success;
        private string m_SessionId;
        
        private string m_Ip;
        private string m_MacAddress;
        private int m_ClientTypeId;
        private string m_ClientVersion;
        private string m_Username;
        private string m_Password;
        private DateTime m_CreationDatetime;
        private string m_FailedReason;

        public string FailedReason
        {
            get { return m_FailedReason; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 255)
                {
                    m_FailedReason = value;
                }
                else
                {
                    m_FailedReason = value.Substring(0, 255);
                }
            }
        }

        public int EventTypeId
        {
            get { return m_EventTypeId; }
            set { m_EventTypeId = value; }
        }

        public Int64? PermissionId
        {
            get { return m_PermissionId; }
            set { m_PermissionId = value; }
        }

        public bool Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }	

        public string SessionId
        {
            get { return m_SessionId; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 50)
                {
                    m_SessionId = value;
                }
                else
                {
                    m_SessionId = value.Substring(0, 50);
                }
            }
        }

        public long? UserId
        {
            get; 
            set; 
        }

        public string Ip
        {
            get { return m_Ip; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 50)
                {
                    m_Ip = value;
                }
                else
                {
                    m_Ip = value.Substring(0, 50);
                }
            }
        }

        public string MacAddress
        {
            get { return m_MacAddress; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 50)
                {
                    m_MacAddress = value;
                }
                else
                {
                    m_MacAddress = value.Substring(0, 50);
                }
            }
        }

        public int ClientTypeId
        {
            get { return m_ClientTypeId; }
            set { m_ClientTypeId = value; }
        }

        public string ClientVersion
        {
            get { return m_ClientVersion; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 50)
                {
                    m_ClientVersion = value;
                }
                else
                {
                    m_ClientVersion = value.Substring(0, 50);
                }
            }
        }

        public string Username
        {
            get { return m_Username; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 255)
                {
                    m_Username = value;
                }
                else
                {
                    m_Username = value.Substring(0, 255);
                }
            }
        }

        public string Password
        {
            get { return m_Password; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length <= 255)
                {
                    m_Password = value;
                }
                else
                {
                    m_Password = value.Substring(0, 255);
                }
            }
        }	

        public DateTime CreationDatetime
        {
            get { return m_CreationDatetime; }
            set { m_CreationDatetime = value; }
        }
        #endregion

        public LoginLog()
        {
        }

        public LoginLog(int eventTypeId, long? permissionId, bool success, string sessionId, long? userId, string ip, string username, string password, DateTime timestamp)
        {
            EventTypeId = eventTypeId;
            PermissionId = permissionId;
            Success = success;
            SessionId = sessionId;
            UserId = userId;
            Ip = ip;
            Username = username;
            Password = password;

     //       Password = EC.Framework.Encryption2.Cryptics.Encrypt(password);
            CreationDatetime = timestamp;
        }

        public LoginLog(int eventTypeId, long? permissionId, bool success, string sessionId, long? userId, string ip, string mac, ClientType clientType, string clientVersion, string username, string password, DateTime timestamp)
            : this(eventTypeId, permissionId, success, sessionId, userId, ip, username, password, timestamp)
        {
            MacAddress = mac;
            ClientTypeId = (int)clientType;
            ClientVersion = clientVersion;
        }

        public LoginLog(int eventTypeId, long? permissionId, bool success, string sessionId, long? userId, string ip, string mac, ClientType clientType, string clientVersion, string username, string password, DateTime timestamp, string failedReason)
            : this(eventTypeId, permissionId, success, sessionId, userId, ip, mac, clientType, clientVersion, username, password, timestamp)
        {
            FailedReason = failedReason;
        }


        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            m_ObjectState = DataAction.Delete;
        }
    }
}
