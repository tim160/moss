using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using EC.Controllers;
using EC.Controllers.utils;
using EC.Models.Database;
using WebGrease.Css.Extensions;
using EC.Models;
using EC.Constants;


namespace EC.Utils.Auth
{
    public class EcAuthorizedAttribute : AuthorizeAttribute
    {
  

        public EcAuthorizedAttribute()
        {

        }

 

 

    }
}