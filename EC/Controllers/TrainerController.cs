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
    public ActionResult AddOnboarding()
    {

      user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
      if (user == null || user.id == 0)
        return RedirectToAction("Login", "Service");
      company cm = new CompanyModel(user.company_id)._company;

      var booked_sessions = db.TrainerTimes.Where(t => t.CompanyId == cm.id).Count();
      int paid_oboarding = cm.onboard_sessions_paid;

      if ((paid_oboarding == 0) || (cm.onboard_sessions_expiry_dt.HasValue && cm.onboard_sessions_expiry_dt < DateTime.Today)
        || (paid_oboarding - booked_sessions < 1))
      {
        // either never bought onboarding, onboarding sessions were expired or booked all his sessions already ---- Redirect to buy more onboarding

        string redirect = "https://www.employeeconfidential.com/book/onboarding/";
        if (is_cc)
          redirect = "https://www.campusconfidential.com/book/onboarding/";

        return Redirect(redirect + cm.guid.ToString());
      }


      return RedirectToAction("Calendar", "Trainer");

    }
  }
}