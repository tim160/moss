using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Common.Util;

namespace EC.Business.Entities
{
    [Serializable]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.mobilefieldforce.com/")]
    public class LoginResult
    {

        private UpdateType m_UpdateType;
        public UpdateType UpdateType
        {
            get { return m_UpdateType; }
            set { m_UpdateType = value; }
        }

        private AccountInformation m_AccountInformation;
        public AccountInformation AccountInformation
        {
            get { return m_AccountInformation; }
            set { m_AccountInformation = value; }
        }

        private OpFailureCode m_FailureCode;
        public OpFailureCode FailureCode
        {
            get { return m_FailureCode; }
            set { m_FailureCode = value; }
        }

        public bool IsCrewChief { get; set; }


        public LoginResult() { }
    }

    public enum UpdateType
    {
        Mandatory,
        Optional,
        None
    }

    [Serializable]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.mobilefieldforce.com/")]
    public class AccountInformation
    {
        private bool m_IsAccountLocked;
        private int m_PasswordExpiredInDays;
        private bool m_AllowToChangePassword;
        private bool m_AuthenticationServiceUsed;

        public bool IsAccountLocked
        {
            get { return m_IsAccountLocked; }
            set { m_IsAccountLocked = value; }
        }

        public int PasswordExpiredInDays
        {
            get { return m_PasswordExpiredInDays; }
            set { m_PasswordExpiredInDays = value; }
        }

        public bool AllowToChangePassword
        {
            get { return m_AllowToChangePassword; }
            set { m_AllowToChangePassword = value; }
        }

        public bool AuthenticationServiceUsed
        {
            get { return m_AuthenticationServiceUsed; }
            set { m_AuthenticationServiceUsed = value; }
        }
    }
}
