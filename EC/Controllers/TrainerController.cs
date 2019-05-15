using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Constants;
using EC.Models;

namespace EC.Controllers
{
    public class TrainerController : BaseController
    {
        // GET: Trainer
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

      if (user.role_id != ECLevelConstants.level_trainer)
                return RedirectToAction("Login", "Service");

            ViewBag.user_id = user.id;

            return View();
        }

        public ActionResult Calendar()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
              return RedirectToAction("Login", "Service", new { returnUrl = Url.Action("Calendar", "Trainer") });
            if (user.role_id == ECLevelConstants.level_trainer)
              return RedirectToAction("Login", "Service");

              ViewBag.user_id = user.id;

      //string company_left_attempts = GetOnboardingsRemaining(user.company_id);
            CompanyModel model = new CompanyModel(user.company_id);
            ViewBag.company_guid = model._company.guid.Value.ToString();
            return View();
        }
    }
}