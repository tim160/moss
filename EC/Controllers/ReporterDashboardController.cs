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
using EC.Controllers.utils;
using EC.Models.ViewModel;
using EC.Models.App.Case;

namespace EC.Controllers
{
    public class ReporterDashboardController : BaseController
    {
        EC.Models.App.Case.CaseMessagesModel temp_cm;
        // GET: ReporterDashboard
        public ActionResult Index(int? id)
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            user = AuthHelper.GetCookies(HttpContext);
            if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id ==6  || user.role_id == 7)
                return RedirectToAction("Index", "Account");


        //    ViewBag.user_id = id.Value; // 167-171
            id = user.id;

            if ((!id.HasValue) || (id.Value == 0))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            UserModel um = new UserModel(id.Value);
            int report_id = um._reporter_report_id;
            ViewBag.report_id = report_id;
            ViewBag.user_id = id.Value;

            if(report_id == 0)
                 return RedirectToAction("Index", "Account");

            ViewBag.attachmentFiles = getAttachmentFiles(report_id);

            return View();
        }

        // GET: Reporter Messages
        public ActionResult Messages(int? id)
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7)
                return RedirectToAction("Index", "Account");

            //    ViewBag.user_id = id.Value; // 167-171
            id = user.id;

            if ((!id.HasValue) || (id.Value == 0))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            ViewBag.user_id = id.Value; // 167-171

            UserModel um = new UserModel(id.Value);
            int report_id = um._reporter_report_id;
            ViewBag.report_id = report_id;
            ViewBag.user_id = id.Value;

            ReportModel rm = new ReportModel(report_id);

            ViewBag.rm = rm;
            ViewBag.um = um;
            ViewBag.unread_message_number = um.Unread_Messages_Quantity(report_id, 1);

            CaseMessagesModel cm = new CaseMessagesModel(report_id, 1, id.Value);

            GlobalFunctions glb = new GlobalFunctions();
            glb.UpdateReadMessages(report_id, id.Value, 1);

            return View(cm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Messages([Bind(Include = "body_tx,report_id,sender_id,reporter_access")]  CaseMessagesModel cm)
        {
            temp_cm = cm;
            cm._message.created_dt = DateTime.Now;
            cm._message.ip_ds = "";
            cm._message.subject_ds = "";

            return View(cm);

            if (ModelState.IsValid)
            {
       /*         db.message.Add(newMessage);
                try
                {
                    db.SaveChanges();
                    glb.UpdateReportLog(newMessage.sender_id, 7, newMessage.report_id, "", null, "");

                    // send emails to Case Admin    

                    #region Email to Case Admin
                    ReportModel rm = new ReportModel(newMessage.report_id);

                    foreach (user _user in rm._mediators_whoHasAccess_toReport)
                    {
                        if ((_user.email.Trim().Length > 0) && glb.IsValidEmail(_user.email.Trim()))
                        {
                            List<string> to = new List<string>();
                            List<string> cc = new List<string>();
                            List<string> bcc = new List<string>();

                            to.Add(_user.email.Trim());
                              ///     bcc.Add("timur160@hotmail.com");

                            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                            eb.NewMessage(_user.first_nm, _user.last_nm, rm._report.display_name);
                            string body = eb.Body;
                   ///////         em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewMessage, body, true);
                        }
                    }
  

                    #endregion

                }
                catch (Exception ex)
                {
                    return View(newMessage);
                }
     ////////           return RedirectToAction("Messages/" + newMessage.sender_id.ToString());
                // return RedirectToAction("Messages/208");
*/
            }

            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7)
                return RedirectToAction("Index", "Account");

          //  return RedirectToAction("Messages/" + user.id.ToString());
            #region Commented --- Values in Session
      /*      EC.Models.App.Case.CaseMessagesModel cm1 = new Models.App.Case.CaseMessagesModel(newMessage.report_id, 1, newMessage.sender_id);

           
            user temp_user = (user)Session[Constants.CurrentUserMarcker];
            if (temp_user == null || temp_user.id == 0 || temp_user.role_id == 4 || temp_user.role_id == 5 || temp_user.role_id == 6 || temp_user.role_id == 7)
                return RedirectToAction("Index", "Account");


            //    ViewBag.user_id = id.Value; // 167-171
            int? id = user.id;

            if ((!id.HasValue) || (id.Value == 0))
                return RedirectToAction("Index", "Account");

            ViewBag.user_id = id.Value; // 167-171

            UserModel um = new UserModel(id.Value);
            int report_id = um._reporter_report_id;
            ViewBag.report_id = report_id;
            ViewBag.user_id = id.Value; 

            return View(cm);
         //   return Json(new { someValue = someValue });*/
            #endregion
        }



        public PartialViewResult AddedMessages()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
      //      if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7)
     //           return RedirectToAction("Index", "Account");
            int sender_id = Convert.ToInt16(Request["sender_id"]);
            if (user.id != sender_id)
            {
                // security issue
                sender_id = user.id;
            }

            int report_id = Convert.ToInt16(Request["report_id"]);
            // check if sender has access to report
            int reporter_access = Convert.ToInt16(Request["reporter_access"]);
            string body_tx = (string)(Request["body_tx"]);

            message _message = new message();

            _message.created_dt = DateTime.Now;
            _message.ip_ds = "";
            _message.subject_ds = "";

            _message.body_tx = body_tx;
            _message.report_id = report_id;
            _message.reporter_access = reporter_access;
            _message.sender_id = sender_id;


            db.message.Add(_message);
            try
            {
                db.SaveChanges();
                glb.UpdateReportLog(_message.sender_id, 7, _message.report_id, "", null, "");

                // send emails to Case Admin    

                #region Email to Case Admin
                ReportModel rm = new ReportModel(_message.report_id);

                foreach (user _user in rm._mediators_whoHasAccess_toReport)
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        List<string> to = new List<string>();
                        List<string> cc = new List<string>();
                        List<string> bcc = new List<string>();

                        to.Add(_user.email.Trim());
                        ///     bcc.Add("timur160@hotmail.com");

                        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                        eb.NewMessage(_user.first_nm, _user.last_nm, rm._report.display_name);
                        string body = eb.Body;
                        ///////         em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewMessage, body, true);
                    }
                }


                #endregion

            }
            catch (Exception ex)
            {
                ///// return or log error?? 
            }

            CaseMessagesViewModel cmvm = new CaseMessagesViewModel().BindMessageToViewMessage(_message, sender_id);
            PartialViewResult pvr = PartialView("~/Views/Shared/Helpers/_SingleMessage.cshtml", cmvm);
            return pvr;
        }


        // GET: Reporter Messages
        public ActionResult Settings(int? id)
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7)
                return RedirectToAction("Index", "Account");

            //    ViewBag.user_id = id.Value; // 167-171
            id = user.id;

            if ((!id.HasValue) || (id.Value == 0))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            ViewBag.user_id = id.Value; // 167-171

            UserModel um = new UserModel(id.Value);
            int report_id = um._reporter_report_id;
            ViewBag.report_id = report_id;
            ReportModel rm = new ReportModel(report_id);//same as activity on bottom.
            CompanyModel cm = new CompanyModel(rm._report.company_id);  
            string anon_level = "";
            List<anonymity> list_anon = cm.GetAnonymities(rm._report.company_id, 0).Where(t => t.id == rm._report.incident_anonymity_id).ToList();
            foreach (anonymity _anon in list_anon)
            {
                anon_level = string.Format(_anon.anonymity_company_en, rm._company_name);
            }
            Session["incidentAnonymity"] = list_anon[0].id;
            ViewBag.anon_level = anon_level;
            //ViewBag.notification_new_reports_flag = user.notification_new_reports_flag;
            return View(rm._reporter_user);
        }

        // GET: Reporter Messages
        public ActionResult Activity(int? id)
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id == 4 || user.role_id == 5 || user.role_id == 6 || user.role_id == 7)
                return RedirectToAction("Index", "Account");


            //    ViewBag.user_id = id.Value; // 167-171
            id = user.id;

            if ((!id.HasValue) || (id.Value == 0))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            ViewBag.user_id = id.Value; // 167-171
            UserModel um = new UserModel(id.Value);
            int report_id = um._reporter_report_id;
            ViewBag.report_id = report_id;

            return View();
        }

        private string Unread_message_number_string1(int report_id, int user_id)
        {
            EC.Models.UserModel um = new EC.Models.UserModel(user_id);

            int unread_message = um.Unread_Messages_Quantity(report_id, 0);
            var unread_message_number_string = "";
            if (unread_message > 0)
            {
                unread_message_number_string = unread_message.ToString();
            }
            return unread_message_number_string;
        }

        public List<attachment> getAttachmentFiles(int report_id)
        {
            EC.Models.ReportModel rm = new EC.Models.ReportModel(report_id);
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == report_id)).ToList();

            return attachmentFiles;
        }

        public bool SaveFields(user _user)
        {
            if (Request.IsAjaxRequest() && Session["incidentAnonymity"]!=null)
            {
                int incidentAnonymity = (int) Session["incidentAnonymity"];
                user userSession = (user)Session[Constants.CurrentUserMarcker];
                if(incidentAnonymity > 0) 
                return ReporterDashboardModel.inst.UpdateUser(Request, userSession, incidentAnonymity);
            }
            return false;
        }
    }
} 