
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using EC.Constants;


namespace EC.Controllers
{
    public class ApplicationController : Controller
    {

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.ApplicationInstance.Context);
            user user = (user)httpContext.Session[ECGlobalConstants.CurrentUserMarcker];
        }
    }
}