using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Constants;

namespace EC.Controllers
{
    public class TrainerController : BaseController
    {
        // GET: Trainer
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            if (user.role_id != ECLevelConstants.level_trainer)
                return RedirectToAction("Index", "Account");

            ViewBag.user_id = user.id;

            return View();
        }

        public ActionResult Calendar()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            if (user.role_id == ECLevelConstants.level_trainer)
                return RedirectToAction("Index", "Account");

            ViewBag.user_id = user.id;

            return View();
        }
    }
}