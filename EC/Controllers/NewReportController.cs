using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EC.Models;
using EC.Models.Database;
using EC.Localization;
using EC.Constants;
using EC.Model.Impl;

namespace EC.Controllers.ViewModel
{
    public class NewReportController : BaseController
    {

        // GET: ReporterDashboard

        public ActionResult Index(int? id)
        {

            int report_id = 0;

            if (!id.HasValue)
                return RedirectToAction("Login", "Service");
            else
                report_id = id.Value;// 167-171??

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;
            ReportModel rm = new ReportModel(report_id);

            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Login", "Service");

            readStatusModel.UpdateReportRead(user_id, report_id);

            if ((rm._investigation_status != 1) && (rm._investigation_status != 2) && (rm._investigation_status != 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewCase", new { report_id = id });
            }

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            UserModel um = new UserModel(user_id);

            Dictionary<int, string> month = m_DateTimeHelper.ShortMonth();

            ViewBag.user_id = user_id;
            ViewBag.um = um;

            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            ViewBag.getNonMediatorsInvolved = getNonMediatorsInvolved(id.Value);
            ViewBag.AllScopes = db.scope.OrderBy(x => x.scope_en).ToList();
            ViewBag.AllSeverities = db.severity.OrderBy(x => x.id).ToList();
            ViewBag.AllDepartments = db.company_department.Where(x => x.company_id == um._user.company_id && x.status_id == 2).OrderBy(x => x.department_en).ToList();
            ViewBag.AllIncidets = db.company_secondary_type.Where(x => x.company_id == um._user.company_id && x.status_id == 2).OrderBy(x => x.secondary_type_en).ToList();
            CompanyModel cm = new CompanyModel(um._user.company_id);
            List<user> available_users = cm.AllMediators(um._user.company_id, true, null).OrderBy(x => x.first_nm).ToList();
            List<UserItems> _users = new List<UserItems>();

            foreach (user _user in available_users)
            {
                UserItems _new_user = new UserItems();
                _new_user.FirstName = _user.first_nm;
                _new_user.LastName = _user.last_nm;
                _new_user.Id = _user.id;
                _users.Add(_new_user);
            }

            ViewBag.AllOwners = _users;

            bool has_access = rm.HasAccessToReport(user_id);

            if ((!has_access) || (user.role_id == 8))
            {
                return RedirectToAction("Login", "Service");
            }
            else
                ViewBag.report_id = report_id;


            if ((rm._investigation_status == 1) && (user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7))
            {

        ///if (!db.report_log.Any(item => ((item.action_id == 28) && (item.report_id == report_id) && (item.string_to_add == LocalizationGetter.GetString("_Completed", is_cc)))))
        ////// if (!db.report_investigation_status.Any(item => (item.report_id == report_id)))
        {
          // need to update investigation status from pending to review after first mediator accessed the report
          report_investigation_status _review_status = new report_investigation_status();
                    _review_status.created_date = DateTime.Now;
                    _review_status.investigation_status_id = (int)CaseStatusConstants.CaseStatusValues.Review;
                    _review_status.report_id = report_id;
                    _review_status.user_id = user_id;
                    _review_status.description = "";

                    db.report_investigation_status.Add(_review_status);

                    var report = db.report.FirstOrDefault(x => x.id == report_id);
                    report.status_id = (int)CaseStatusConstants.CaseStatusValues.Review;
                    report.last_update_dt = DateTime.Now;
                    report.user_id = user_id;

                    db.SaveChanges();

                  logModel.UpdateReportLog(user_id, 20, report_id, LocalizationGetter.GetString("_Started", is_cc), null, "");
                }
            }


            ViewBag.company_location = db.company_location.Where(x => x.company_id == rm._report.company_id).ToList();
            ViewBag.report_mediator_assigned = db.report_mediator_assigned.Where(x => x.report_id == report_id).ToList();
            ViewBag.report_mediator_involved = db.report_mediator_involved.Where(x => x.report_id == report_id).ToList();
            var report_secondary_type = db.report_secondary_type.Where(x => x.report_id == report_id).ToList();
            ViewBag.report_secondary_type = report_secondary_type;
            var ids = report_secondary_type.Select(x => x.secondary_type_id).ToList();
            var company_case_routing = db.company_case_routing.Where(x => x.company_id == rm._report.company_id & ids.Contains(x.company_secondary_type_id)).ToList();
            ViewBag.company_case_routing = company_case_routing;

            ids = company_case_routing.Select(x => x.id).ToList();
            ViewBag.AllPolicies = db.company_case_routing_attachments
                .Where(x => ids.Contains(x.company_case_routing_id) & x.status_id == 2)
                .OrderBy(x => x.company_case_routing_id)
                .ThenBy(x => x.file_nm)
                .ToList();

            var midiators_roles = new int[] { 4, 5, 6 };
            ViewBag.AllMediators = db.user
                .Where(x => x.company_id == rm._report.company_id & midiators_roles.Contains(x.role_id))
                .ToList();

            return View();
        }

        [HttpPost]
    ////cors for sso- remove for live site       [ValidateAntiForgeryToken]
    public ActionResult UpdateStatus([Bind(Include = "report_id,report_investigation_status, description, user_id")] report_investigation_status newStatus)
        {
            newStatus.created_date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.report_investigation_status.Add(newStatus);

                var report = db.report.FirstOrDefault(x => x.id == newStatus.report_id);
                report.status_id = newStatus.investigation_status_id;
                report.last_update_dt = DateTime.Now;
                report.user_id = newStatus.user_id;

                try
                {
          //     db.SaveChanges();
          // need to save new row to report_log about 
          //glb.UpdateReportLog(newStatus.user_id, 20, newStatus.report_id, LocalizationGetter.GetString("_Completed", is_cc) , null, newStatus.description);
        }
        catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return View(newStatus);
                }
                return RedirectToAction("NewCase/Index/" + newStatus.report_id.ToString());
                //  return RedirectToAction("Messages/208");
            }

            return View(newStatus);
        }

        public bool SendSpamMessage()
        {
            int report_id = Convert.ToInt16(Request["report_id"]);
            int user_id = Convert.ToInt16(Request["user_id"]);
            string spam_Message = Request["spamMessage"];


            report_investigation_status addStatus =
                new report_investigation_status()
                {
                    report_id = report_id,
                    investigation_status_id = (int)CaseStatusConstants.CaseStatusValues.Spam,
                    created_date = DateTime.Now,
                    user_id = user_id,
                    description = spam_Message
                };

            db.report_investigation_status.Add(addStatus);

            var report = db.report.FirstOrDefault(x => x.id == report_id);
            report.status_id = (int)CaseStatusConstants.CaseStatusValues.Spam;
            report.last_update_dt = DateTime.Now;
            report.user_id = user_id;


            db.SaveChanges();

            logModel.UpdateReportLog(user_id, 18, report_id, spam_Message, null, "");

            return true;
        }

        public bool AcceptOrReopenCase()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];

            int report_id = Convert.ToInt16(Request["report_id"]);
            int user_id = Convert.ToInt16(Request["user_id"]);
            string description = "";// Request["description"];
            int scopeId = Convert.ToInt32(Request["scopeId"]);
            int severityId = Convert.ToInt32(Request["severityId"]);
            //int departmentId = Convert.ToInt32(Request["departmentId"]);
            //int incidentId = Convert.ToInt32(Request["incidentId"]);
            int ownerId = Convert.ToInt32(Request["ownerId"]);
            bool lifeThreat = Convert.ToBoolean(Request["isLifeThreat"]);

            ReportModel rm = new ReportModel(report_id);

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Request.Url.AbsoluteUri.ToLower());

            using (ECEntities adv = new ECEntities())
            {

                report_investigation_status addStatus =
                new report_investigation_status()
                {
                    report_id = report_id,
                    investigation_status_id = (int)CaseStatusConstants.CaseStatusValues.Investigation,
                    created_date = DateTime.Now,
                    user_id = user_id,
                    description = description
                };

                adv.report_investigation_status.Add(addStatus);

                var report = adv.report.FirstOrDefault(x => x.id == report_id);
                report.status_id = (int)CaseStatusConstants.CaseStatusValues.Investigation;
                report.last_update_dt = DateTime.Now;
                report.user_id = user_id;

                //Remove owner
                foreach (var rem_owner in db.report_owner.Where(x => x.report_id == report_id).ToList())
                {
                    rem_owner.status_id = 1;
                }

                //New owner
                var owner = new report_owner
                {
                    report_id = report_id,
                    status_id = 2,
                    user_id = ownerId,
                    created_on = DateTime.Now,
                };
                db.report_owner.Add(owner);
                db.SaveChanges();

                var _um = new UserModel(ownerId);
                eb.SetCaseOwner(_um._user.first_nm, _um._user.last_nm, user.first_nm, user.last_nm, rm._report.display_name);
                emailNotificationModel.SaveEmailBeforeSend(user.id, _um._user.id, _um._user.company_id, _um._user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                    LocalizationGetter.GetString("Email_Title_SetCaseOwner", is_cc), eb.Body, false, 65);

                var item = db.report_mediator_assigned.FirstOrDefault(x => x.report_id == report_id & x.mediator_id == ownerId);
                if (item == null)
                {
                    //Add to mediators
                    var report_mediator_assigned = new report_mediator_assigned
                    {
                        mediator_id = ownerId,
                        case_owner_id = owner.id,
                        assigned_dt = DateTime.Now,
                        last_update_dt = DateTime.Now,
                        report_id = report_id,
                        status_id = 2,
                        user_id = user_id,
                    };
                    db.report_mediator_assigned.Add(report_mediator_assigned);
                    db.SaveChanges();
                }

                adv.SaveChanges();
            }
            using (ECEntities adv = new ECEntities())
            {
                var report = adv.report.FirstOrDefault(x => x.id == report_id);
                report.scope_id = scopeId;
                report.severity_id = severityId;
                report.severity_user_id = user.id;
                report.scope_user_id = user.id;
                report.cc_is_life_threating = lifeThreat;
                report.cc_is_life_threating_created_date = DateTime.Now;
                report.cc_is_life_threating_user_id = user.id;
                adv.SaveChanges();

            }
              #region commited code
      /*   if (scopeId == 2)
         {
             glb.UpdateReportLog(user_id, 41, report_id, "Case Scope: Internal", null, "");
         }
         if (scopeId == 1)
         {
             glb.UpdateReportLog(user_id, 42, report_id, "Case Scope: Regulatory", null, "");
         }
         if (severityId == 2)
         {
             glb.UpdateReportLog(user_id, 43, report_id, "Case Severity: Low", null, "");
         }
         if (severityId == 3)
         {
             glb.UpdateReportLog(user_id, 44, report_id, "Case Severity: Medium", null, "");
         }
         if (severityId == 4)
         {
             glb.UpdateReportLog(user_id, 45, report_id, "Case Severity: High", null, "");
         }
         if (severityId == 5)
         {
             glb.UpdateReportLog(user_id, 46, report_id, "Case Severity: Critical", null, "");
         }*/

      // Case accepted
      #endregion
              logModel.UpdateReportLog(user_id, 17, report_id, description, null, "");
      ///// to return        glb.UpdateReportLog(user_id, 20, report_id, LocalizationGetter.GetString("_Completed", is_cc), null, "");


      report_log _log = new report_log();

            CompanyModel cm = new CompanyModel(rm._report.company_id);
            UserModel um = new UserModel(user_id);
            #region Email Ready
            List<string> bcc = new List<string>();

            em = new EC.Business.Actions.Email.EmailManagement(is_cc);
            eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            #endregion

            if (EC.Common.Util.DomainUtil.IsCC(Request))
            {
                if (lifeThreat)
                {
                  logModel.UpdateReportLog(user_id, 16, report_id, "", null, "");


                    string platform_manager_email = "";
                    var platformManager = cm.AllMediators(cm.ID, true, null).FirstOrDefault(x => x.role_id == 5);
                    if ((platformManager != null) && (!String.IsNullOrEmpty(platformManager.email)))
                        platform_manager_email = platformManager.email;
                    bool sent_email = false;

                    if (!String.IsNullOrEmpty(cm._company.cc_campus_alert_manager_email))
                    {
                        //glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, cm._company.cc_campus_alert_manager_email, cm._company.cc_campus_alert_manager_first_name, cm._company.cc_campus_alert_manager_last_name);

                        emailNotificationModel.CampusSecurityAlertEmail(user.id, rm._report, Request.Url, db, cm._company.cc_campus_alert_manager_email);
                        logModel.UpdateReportLog(user_id, 24, report_id, "", null, "");
                    }
                    else if (platform_manager_email.Length > 0)
                    {
                        //glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, platformManager.email, platformManager.first_nm, platformManager.last_nm);

                        emailNotificationModel.CampusSecurityAlertEmail(user.id, rm._report, Request.Url, db, platformManager.email);
                        sent_email = true;
                        logModel.UpdateReportLog(user_id, 24, report_id, "", null, "");
                    }

                    /*         if (!String.IsNullOrEmpty(cm._company.cc_daily_crime_log_manager_email))
                               {
                                   glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, cm._company.cc_daily_crime_log_manager_email);
                               }
                               else if (platform_manager_email.Length > 0 && !sent_email)
                               {
                                   glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, platform_manager_email);
                                   glb.UpdateReportLog(user_id, 24, report_id, "", null, "");
                               }
                               */
                }
            }

              logModel.UpdateReportLog(user_id, 21, report_id, LocalizationGetter.GetString("_Started", is_cc), null, "");
            if (!db.report_log.Any(item => ((item.action_id == 19) && (item.report_id == report_id))))
            {
                //case opened
                logModel.UpdateReportLog(user_id, 19, report_id, description, null, "");

                #region Email To Mediators About Case Approved
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        //to = new List<string>();
                        //cc = new List<string>();
                        //bcc = new List<string>();

                        //to.Add(_user.email.Trim());

                        eb.NextStep(_user.first_nm, _user.last_nm, rm._report.display_name);
            //body = eb.Body;

              ///   em.Send(to, cc, LocalizationGetter.GetString("Email_Title_NextStep", is_cc)  , body, true);
            emailNotificationModel.SaveEmailBeforeSend(user.id, _user.id, _user.company_id, _user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                            LocalizationGetter.GetString("Email_Title_NextStep", is_cc), eb.Body, false, 7);
                    }
                }
                #endregion

                #region Email to Reporter About case been Approved

                if ((rm._reporter_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(rm._reporter_user.email.Trim()))
                {
                    eb.NextStep(um._user.first_nm, um._user.last_nm, rm._report.display_name);
                    emailNotificationModel.SaveEmailBeforeSend(user.id, rm._reporter_user.id, rm._reporter_user.company_id, rm._reporter_user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                        LocalizationGetter.GetString("Email_Title_NextStep", is_cc), eb.Body, false, 7);
                }

                #endregion
            }
            else
            {
                // case re-opened
                logModel.UpdateReportLog(user_id, 29, report_id, description, null, "");


                #region Email To Mediators About Case re-opening
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        eb.CaseReopened(_user.first_nm, _user.last_nm, rm._report.display_name, um._user.first_nm, um._user.last_nm);
                        emailNotificationModel.SaveEmailBeforeSend(user.id, _user.id, _user.company_id, _user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                         LocalizationGetter.GetString("Email_Title_CaseReopened", is_cc), eb.Body, false, 8);
                    }
                }
                #endregion

                #region Email to Reporter About case been reopened
                if ((rm._reporter_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(rm._reporter_user.email.Trim()))
                {
                    eb.ReporterCaseReopened(rm._report.display_name);
                    emailNotificationModel.SaveEmailBeforeSend(user.id, rm._reporter_user.id, rm._reporter_user.company_id, rm._reporter_user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                            LocalizationGetter.GetString("Email_Title_CaseReopened", is_cc), eb.Body, false, 33);
                }

                #endregion
            }
 
            switch (severityId)
            {
                case 2:
                    logModel.UpdateReportLog(user_id, 43, report_id, "", null, "");
                    break;
                case 3:
                    logModel.UpdateReportLog(user_id, 44, report_id, "", null, "");
                    break;
                case 4:
                    logModel.UpdateReportLog(user_id, 45, report_id, "", null, "");
                    break;
                case 5:
                  logModel.UpdateReportLog(user_id, 46, report_id, "", null, "");
                    break;
            }
            switch (scopeId)
            {
                case 1:
                  logModel.UpdateReportLog(user_id, 41, report_id, "", null, "");
                    break;
                case 2:
                  logModel.UpdateReportLog(user_id, 42, report_id, "", null, "");
                    break;
            }
            return true;
        }

        public List<attachment> getAttachmentFiles(int report_id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == report_id && !item.visible_mediators_only.HasValue && !item.visible_reporter.HasValue)).ToList();
            return attachmentFiles;
        }

        public List<report_non_mediator_involved> getNonMediatorsInvolved(int report_id)
        {
            List<report_non_mediator_involved> nonMediatorsInvolved = db.report_non_mediator_involved.Where(item => (item.report_id == report_id)).ToList();
            return nonMediatorsInvolved;
        }
    }
}