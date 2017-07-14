using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EC.Controllers.utils;
using EC.Controllers.ViewModel;
using EC.Models;
using EC.Models.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using EC.Constants;

namespace EC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserModel userModel = UserModel.inst;
        //
        // GET: /Account/
        public ActionResult Index()
        {
         /*   ReportModel rm = new ReportModel(208);
            DateTime dt = rm.promotion_toactive_status_date();
            DateTime dt1 = rm.last_event_date();
            */
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            SingOut();
            return View("~/Views/Login/Index.cshtml");
        }


        public ActionResult Login()
        {
            SingOut();
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            return View("~/Views/Login/Index.cshtml");
            //return Redirect("/");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var user = userModel.Login(model.login, model.password);
            if (user != null)
            {
                SignIn(user);
                if (user.role_id == 8)
                {
                    return Redirect("~/ReporterDashboard");
                }
                return Redirect("~/Cases");
                //Session["userName"] = "";
                //Session["userId"] = user.id;
            }
            else
            {
                ModelState.AddModelError("Login", "Login failed"); 
                ModelState.AddModelError("Login", "Login failed");
                return View("~/Views/Login/Index.cshtml", model);
            }   
        }

        private void SignIn(user user)
        {
            AuthHelper.SetCookies(user, HttpContext);
            Session[ECGlobalConstants.CurrentUserMarcker] = user;
        }

        private void SingOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
        }
	}
}