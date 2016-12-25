using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Controllers.Utils;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Constants;

namespace EC.Controllers
{
    public class EmployeeAwarenessController : BaseController
    {
        // GET: EmployeeAwareness
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            ViewBag.user_id = user_id;

            return View();
        }

        public ActionResult Poster()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            ViewBag.user_id = user_id;

            return View();
        }
    }
}