using EC.Common.Util;
using EC.Constants;
using EC.Controllers.utils;
using EC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.Controllers
{
    public class ServiceController : Controller
    {
        public class LoginViewModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        private readonly UserModel userModel = UserModel.inst;

        // GET: Service
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            Session.Clear();
            GlobalFunctions glb = new GlobalFunctions();

            if (DomainUtil.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {
                if (!String.IsNullOrEmpty(model.Login))
                {
                    var user = userModel.Login(model.Login, model.Password);
                    if (user == null)
                    {
                        return View(model);
                    }

                    AuthHelper.SetCookies(user, HttpContext);
                    Session[ECGlobalConstants.CurrentUserMarcker] = user;

                    Session["userName"] = "";
                    Session["userId"] = user.id;

                    if (user.role_id == 8)
                    {
                        return RedirectToAction("Index", "ReporterDashboard");
                    }
                    return RedirectToAction("Index", "Cases");
                }
            }

            return View(model);
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult CheckStatus()
        {
            return View();
        }
    }
}