using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Common.Interfaces;
using EC.Core.Common;
using log4net;

namespace EC.Models.Database
{
  public class BaseDB
  {
    public ECEntities db = new ECEntities();
    protected IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
    public ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();
  }
}