using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
using EC.Business.Entities;
using EC.Common;
using EC.Common.Util;
using EC.Framework.Data;
using EC.Framework.Logger;
using EC.Framework.TimeZone;
using System.Web;
using EC.Business.Abstract;


namespace EC.Business.Actions
{
    public class WorkOrderManagement
    {
        #region Property(s)
 //       private static readonly ICustomLog m_Log = CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CultureInfo m_CultureInfo = null;
        private static readonly char[] m_TrimChars = { '/' };
        #endregion

    
      
        public WorkOrderManagement()
        {
            m_CultureInfo = Culture.GetCulture();
        }

        public WorkOrderManagement(CultureInfo ci)
        {
            m_CultureInfo = ci;
        }

    
     }

    public class Reason
    {
        private ReasonCode m_Code = ReasonCode.Unknown;
        public ReasonCode Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        private string m_Description = string.Empty;
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private string m_Exception = string.Empty;
        public string Exception
        {
            get { return m_Exception; }
            set { m_Exception = value; }
        }

        public Reason()
        {
        }

        public override string ToString()
        {
            return string.Format("ReasonCode: {0}, Description: {1}, Exception: {2}",
                m_Code, m_Description, m_Exception);
        }
    }


}