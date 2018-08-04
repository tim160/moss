using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Controllers.Utils;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Controllers.ViewModel;
using EC.Localization;
using EC.Constants;
using EC.Model.Impl;
using EC.Core.Common;
using EC.Common.Interfaces;


namespace EC.Controllers.ViewModel
{
    public class NewReportController : BaseController
    {

        // GET: ReporterDashboard

        /* public ActionResultExtended Index(int? id)
         {
             if (!id.HasValue)
                 id = 166;
             NewReportModel reporterDashboard = new NewReportModel();
             ViewBag.user_id = id.Value; // 167-171
         }*/
        public ActionResult Index(int? id)
        {

            int report_id = 0;

            if (!id.HasValue)
                return RedirectToAction("Index", "Account");
            else
                report_id = id.Value;// 167-171??

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            ReportModel rm = new ReportModel(report_id);

            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            glb.UpdateReportRead(user_id, report_id);
            NewReportModel reporterDashboard = new NewReportModel();


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
                return RedirectToAction("Index", "Account");
            }
            else
                ViewBag.report_id = report_id;


            if ((rm._investigation_status == 1) && (user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7))
            {

                ///if (!db.report_log.Any(item => ((item.action_id == 28) && (item.report_id == report_id) && (item.string_to_add == App_LocalResources.GlobalRes._Completed))))
               ////// if (!db.report_investigation_status.Any(item => (item.report_id == report_id)))
                {
                        // need to update investigation status from pending to review after first mediator accessed the report
                        report_investigation_status _review_status = new report_investigation_status();
                    _review_status.created_date = DateTime.Now;
                    _review_status.investigation_status_id = 2;
                    _review_status.report_id = report_id;
                    _review_status.user_id = user_id;
                    _review_status.description = "";

                    db.report_investigation_status.Add(_review_status);
                    db.SaveChanges();

 ///////                   glb.UpdateReportLog(user_id, 28, report_id, App_LocalResources.GlobalRes._Completed, null, "");
                    glb.UpdateReportLog(user_id, 20, report_id, App_LocalResources.GlobalRes._Started, null, "");
                }
            }


            /*
            report report = new report();
            report = reporterDashboard.getReport(175);


            ViewBag.id = report.id;

            ViewBag.generatedName = report.report_name_generic;
            ViewBag.companyName = report.submitted_company_nm;

            ViewBag.reported = report.reported_dt.Day + " " + month[report.reported_dt.Month] + " " + report.reported_dt.Year;
            ViewBag.totalTime = (System.DateTime.Now - report.reported_dt).Days + " days";

            ViewBag.happenedIn = report.other_location_name;


            //Parties involved
            if (report.management_know_id == null)
            {
                ViewBag.managmentKnow = "NULL";
            }
            else
            {
                ViewBag.managmentKnow = reporterDashboard.getManagmentKnowInfo((Int16)report.management_know_id);
            }


            report_secondary_type reportSecondaryType = new report_secondary_type();
            reportSecondaryType = reporterDashboard.getReportSecondaryType(report.id);

            //Case nformation
            ViewBag.reportingAbout = reportSecondaryType.secondary_type_nm;
            ViewBag.incidentHappend = report.incident_dt.Day + " " + month[report.incident_dt.Month] + " " + report.incident_dt.Year;
            ViewBag.incidentDescription = report.description;

            //Messages
            ViewBag.messages = reporterDashboard.getMessagesList(report.id);
             */

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
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus([Bind(Include = "report_id,report_investigation_status, description, user_id")] report_investigation_status newStatus)
        {
            newStatus.created_date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.report_investigation_status.Add(newStatus);
                try
                {
                    //     db.SaveChanges();
                    // need to save new row to report_log about 
                    //glb.UpdateReportLog(newStatus.user_id, 20, newStatus.report_id, App_LocalResources.GlobalRes._Completed , null, newStatus.description);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return View(newStatus);
                }
                return RedirectToAction("Case/Index/" + newStatus.report_id.ToString());
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
                    investigation_status_id = 7,
                    created_date = DateTime.Now,
                    user_id = user_id,
                    description = spam_Message
                };

            db.report_investigation_status.Add(addStatus);
            db.SaveChanges();

            glb.UpdateReportLog(user_id, 18, report_id, spam_Message, null, "");

            return true;
        }

        public bool AcceptOrReopenCase()
        {
            int report_id = Convert.ToInt16(Request["report_id"]);
            int user_id = Convert.ToInt16(Request["user_id"]);
            string description = "";// Request["description"];
            int scopeId = Convert.ToInt32(Request["scopeId"]);
            int severityId = Convert.ToInt32(Request["severityId"]);
            //int departmentId = Convert.ToInt32(Request["departmentId"]);
            //int incidentId = Convert.ToInt32(Request["incidentId"]);
            int ownerId = Convert.ToInt32(Request["ownerId"]);
            bool lifeThreat = Convert.ToBoolean(Request["isLifeThreat"]);
            using (ECEntities adv = new ECEntities())
            { 
                report_investigation_status addStatus =
                new report_investigation_status()
                {
                    report_id = report_id,
                    investigation_status_id = 3,
                    created_date = DateTime.Now,
                    user_id = user_id,
                    description = description
                };

                adv.report_investigation_status.Add(addStatus);

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
                user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];

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
            glb.UpdateReportLog(user_id, 17, report_id, description, null, "");
            ///// to return        glb.UpdateReportLog(user_id, 20, report_id, App_LocalResources.GlobalRes._Completed, null, "");

            
            report_log _log = new report_log();

            ReportModel rm = new ReportModel(report_id);
            CompanyModel cm = new CompanyModel(rm._report.company_id);
            UserModel um = new UserModel(user_id);
            #region Email Ready
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            string body = "";
            #endregion

            if (EC.Common.Util.DomainUtil.IsCC(Request))
            {
                if (lifeThreat)
                {
                    glb.UpdateReportLog(user_id, 16, report_id, "", null, "");
  ////////                  glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, cm._company.cc_campus_alert_manager_email);
   ///////                 glb.CampusSecurityAlertEmail(rm._report, Request.Url, db, cm._company.cc_daily_crime_log_manager_email);
                }
            }
            glb.UpdateReportLog(user_id, 21, report_id, App_LocalResources.GlobalRes._Started, null, "");
            if (!db.report_log.Any(item => ((item.action_id == 19) && (item.report_id == report_id))))
            {
                //case opened
                glb.UpdateReportLog(user_id, 19, report_id, description, null, "");

                #region Email To Mediators About Case Approved
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        to = new List<string>();
                        cc = new List<string>();
                        bcc = new List<string>();

                        to.Add(_user.email.Trim());

                        eb.NextStep(_user.first_nm, _user.last_nm, rm._report.display_name);
                        body = eb.Body;

                     ///   em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                    }
                }
                #endregion

                #region Email to Reporter About case been Approved

                if ((rm._reporter_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(rm._reporter_user.email.Trim()))
                {
                    to = new List<string>();
                    cc = new List<string>();
                    bcc = new List<string>();

                    to.Add(rm._reporter_user.email.Trim());

                    eb.NextStep(um._user.first_nm, um._user.last_nm, rm._report.display_name);
                    body = eb.Body;

                    em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                }

                #endregion
            }
            else
            {
                // case re-opened
                glb.UpdateReportLog(user_id, 29, report_id, description, null, "");


                #region Email To Mediators About Case re-opening
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        to = new List<string>();
                        cc = new List<string>();
                        bcc = new List<string>();

                        to.Add(_user.email.Trim());
                        ///     bcc.Add("timur160@hotmail.com");

                        eb.CaseReopened(_user.first_nm, _user.last_nm, rm._report.display_name, um._user.first_nm, um._user.last_nm);
                        body = eb.Body;

                        em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_CaseReopened, body, true);
                    }
                }
                #endregion

                #region Email to Reporter About case been reopened
                if ((rm._reporter_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(rm._reporter_user.email.Trim()))
                {
                    to = new List<string>();
                    cc = new List<string>();
                    bcc = new List<string>();

                    to.Add(rm._reporter_user.email.Trim());
                    ///     bcc.Add("timur160@hotmail.com");

                    eb.ReporterCaseReopened(rm._report.display_name);
                    body = eb.Body;

                    em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_CaseReopened, body, true);
                }

                #endregion
            }

            /////     glb.UpdateReportLog(user_id, 20, report_id, App_LocalResources.GlobalRes._Completed, null, description);
            //////    glb.UpdateReportLog(user_id, 21, report_id, App_LocalResources.GlobalRes._Started, null, "");
            switch (severityId)
            {
                case 2:
                    glb.UpdateReportLog(user_id, 43, report_id, "", null, "");
                    break;
                case 3:
                    glb.UpdateReportLog(user_id, 44, report_id, "", null, "");
                    break;
                case 4:
                    glb.UpdateReportLog(user_id, 45, report_id, "", null, "");
                    break;
                case 5:
                    glb.UpdateReportLog(user_id, 46, report_id, "", null, "");
                    break;
            }
            switch (scopeId)
            {
                case 1:
                    glb.UpdateReportLog(user_id, 41, report_id, "", null, "");
                    break;
                case 2:
                    glb.UpdateReportLog(user_id, 42, report_id, "", null, "");
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