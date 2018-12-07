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

            string company_left_attempts = GetOnboardingsRemaining(user.company_id);




            return View();
        }

        public string GetOnboardingsRemaining(int company_id)
        {
            string remaining_onboardings = "";
            CompanyModel cm = new CompanyModel(company_id);

            int paid_oboarding = cm._company.onboard_sessions_paid;
            if (cm != null && cm._company != null &&  paid_oboarding > 0)
            {
                
                // at least customer paid for them
                remaining_onboardings = $"You've paid for {paid_oboarding} sessions. ";
                if (cm._company.onboard_sessions_expiry_dt != null && cm._company.onboard_sessions_expiry_dt < DateTime.Today)
                {
                    remaining_onboardings = "Your onboarding sessions expired.";
                }
                else
                {
                    int booked_sessions =  db.TrainerTimes.Where(t => t.Hour >= cm._company.onboard_sessions_expiry_dt.Value.AddYears(-1)).Count();
                    switch (booked_sessions)
                    {
                        case 0:
                            remaining_onboardings += "None taken. ";
                            break;
                        case 1:
                            remaining_onboardings += "Used one. ";
                            break;
                        case 2:
                            remaining_onboardings += "Used two. ";
                            break;
                        case 3:
                            remaining_onboardings += "Used tree. ";
                            break;

                    }

                    int remaining_sessions = paid_oboarding - booked_sessions;
                    switch (remaining_sessions)
                    {
                        case 0:
                            remaining_onboardings += "You have no onboarding and follow-up sessions remaining. ";
                            break;
                        case 1:
                            remaining_onboardings += "You have one session remaining. ";
                            break;
                        case 2:
                            remaining_onboardings +=  "You have two sessions remaining. ";
                            break;
                        case 3:
                            remaining_onboardings += "You have an onboarding and two follow-up sessions remaining. ";
                            break;

                    }

                }
            }


            return remaining_onboardings;

        }
    }
}