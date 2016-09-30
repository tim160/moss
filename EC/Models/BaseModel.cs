using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Common.Interfaces;
using EC.Core.Common;

namespace EC.Models
{
    public class BaseModel
    {
        public ECEntities db = new ECEntities();
        protected IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
        

    }
}