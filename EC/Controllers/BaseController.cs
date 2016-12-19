using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EC.Controllers.Utils;
using EC.Utils.Auth;
using EC.Models.Database;
using EC.Controllers.utils;
using EC.Models;
using EC.Common.Interfaces;
using EC.Core.Common;
using log4net;

namespace EC.Controllers
{
    public class BaseController : Controller
    {
       /// var culture = Culture.GetCulture();
  ////      ILog logger = LogManager.GetLogger(typeof((_Default));


        public ECEntities db = new ECEntities();
        public CultureInfo m_CultureInfo = null;
        public GlobalFunctions glb = new GlobalFunctions();
        public string CurrentLangCode { get; protected set; }
        private SessionManager sessionManager = SessionManager.inst;
        internal IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
        internal bool is_cc
        {
            get { return glb.IsCC(Request.Url.AbsoluteUri.ToLower()); }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.ApplicationInstance.Context);
            user user = (user)Session[Constants.CurrentUserMarcker]; // (user)httpContext.Session[Constants.CurrentUserMarcker];
            if (user == null)
            {
                //sessionManager.User = AuthHelper.GetCookies(httpContext); расскоментировать когда будет использоваться авторизация! и в IndexController

                //NEED TO BE FIXED
                //Session["user"] = UserModel.inst.GetById(2);
            }

            sessionManager.Lang = CurrentLangCode;
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //CurrentLangCode = "ru";
            CurrentLangCode = "en";
            if (requestContext.RouteData.Values["lang"] != null && requestContext.RouteData.Values["lang"] as string != "null")
            {
                CurrentLangCode = requestContext.RouteData.Values["lang"] as string;
                var ci = new CultureInfo(CurrentLangCode);
                sessionManager.Lang = CurrentLangCode;
                Session["lang"] = CurrentLangCode;
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
            //sessionManager.Lang = (string)Session["lang"];
            base.Initialize(requestContext);
        }
        public ActionResult Logout()
        {
            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.ApplicationInstance.Context);
            httpContext.Response.Cookies.Remove(Constants.AuthUserCookies);
            return RedirectToAction("Start", "Index");
        }
	}
}