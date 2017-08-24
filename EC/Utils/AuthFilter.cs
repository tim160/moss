using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Constants;

namespace EC.Utils
{
    public class AuthFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null)
            {
                filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }

    public class AuthFilterApi : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            base.OnActionExecuting(actionContext);
        }
    }
}