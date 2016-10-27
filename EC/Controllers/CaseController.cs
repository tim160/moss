using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.DynamicData.ModelProviders;
using System.Web.Mvc;
using System.Web.UI.WebControls;

using System.Globalization;

using EC.Controllers.Utils;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Controllers.ViewModel;
using EC.Models.ViewModel;
using EC.Models.App.Case;
using EC.Common.Interfaces;
using EC.Core.Common;

namespace EC.Controllers
{
    public class CaseController : BaseController
    {

        private readonly UserModel userModel = UserModel.inst;
        private readonly CompanyModel companyModel = CompanyModel.inst;
        private readonly ReportModel reportModel = ReportModel.inst;
        private IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();


        public ActionResult Index(int? id, string popup)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            glb.UpdateReportRead(user_id, id.Value);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            // user
            UserModel um = new UserModel(user_id);
            List<task> tasks = rm.ReportTasks(0);

            List<TaskExtended> list_tsk = new List<TaskExtended>();
            int task_id = 0;
            ActiveCasesModel temp = new ActiveCasesModel();
            foreach (task _task in tasks)
            {
                task_id = _task.id;
                TaskExtended tsk = new TaskExtended(_task.id, user_id);
                temp.HasTaskFile(tsk);
                list_tsk.Add(tsk);
            }
            ViewBag.tasks = list_tsk;
            ViewBag.um = um;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            if(popup!=null)
            {
                ViewBag.popup = popup;
            }
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;

            return View();
        }


        // GET: Messages
        public ActionResult Messages(int? id, string popup)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            glb.UpdateReportRead(user_id, id.Value);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            // user
            UserModel um = new UserModel(user_id);
            CaseMessagesModel cm = new CaseMessagesModel(id.Value, 2, user_id);
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.rm = rm;
            ViewBag.um = um;
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm._secondary_type_string + " - " + rm._location_string;

            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            if (popup != null)
            {
                ViewBag.popup = popup;
            }
            glb.UpdateReadMessages(id.Value, user_id, 2);

            return View(cm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Messages([Bind(Include = "body_tx,report_id,sender_id,reporter_access")] message newMessage)
        {
            newMessage.created_dt = DateTime.Now;
            newMessage.ip_ds = "";
            newMessage.subject_ds = "";
            newMessage.reporter_access = 2;
            if (ModelState.IsValid)
            {
                db.message.Add(newMessage);
                try
                {
                    db.SaveChanges();
                    glb.UpdateReportLog(newMessage.sender_id, 8, newMessage.report_id, "", null, "");

                    // send emails to Case Admin    
                    ReportModel rm = new ReportModel(newMessage.report_id);

                    #region Email to Case Admin

                    foreach (user _user in rm._mediators_whoHasAccess_toReport)
                    {
                        if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()) && (_user.id != newMessage.sender_id))
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
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewMessage, body, true);
                        }
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    return View(newMessage);
                }

                 return RedirectToAction("Messages/" + newMessage.report_id.ToString());
              //  return RedirectToAction("Messages/208");

            }

            return View(newMessage);
        }
        // GET: Reporter Messages
        public ActionResult Reporter(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            glb.UpdateReportRead(user_id, id.Value);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            // user
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            // user
            UserModel um = new UserModel(user_id);
            ViewBag.rm = rm;
            ViewBag.um = um;
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm._secondary_type_string + " - " + rm._location_string;
            CaseMessagesModel cm = new CaseMessagesModel(id.Value, 1, user_id);

            glb.UpdateReadMessages(id.Value, user_id, 1);

            return View(cm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reporter([Bind(Include = "body_tx,report_id,sender_id,reporter_access")] message newMessage)
        {
            newMessage.created_dt = DateTime.Now;
            newMessage.ip_ds = "";
            newMessage.subject_ds = "";
            newMessage.reporter_access = 1;

            if (ModelState.IsValid)
            {
                db.message.Add(newMessage);
                try
                {
                    db.SaveChanges();
                    glb.UpdateReportLog(newMessage.sender_id, 7, newMessage.report_id, "", null, "");

                    // send emails to Case Admin    
                    ReportModel rm = new ReportModel(newMessage.report_id);

                    #region Email to Case Admin

                    foreach (user _user in rm._mediators_whoHasAccess_toReport)
                    {
                        if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()) && (_user.id != newMessage.sender_id))
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
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewMessage, body, true);
                        }
                    }

                    #endregion

                    #region Send email to reporter
                    user _reporter = rm._reporter_user;
                    if ((_reporter.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_reporter.email.Trim()))
                    {
                        List<string> to = new List<string>();
                        List<string> cc = new List<string>();
                        List<string> bcc = new List<string>();

                        to.Add(_reporter.email.Trim());
                        ///     bcc.Add("timur160@hotmail.com");

                        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                        eb.NewMessage(_reporter.first_nm, _reporter.last_nm, rm._report.display_name);
                        string body = eb.Body;
                        em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewMessage, body, true);
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    return View(newMessage);
                }
                return RedirectToAction("Reporter/" + newMessage.report_id.ToString());
                // return RedirectToAction("Reporter/208");
            }

            return View(newMessage);
        }


        // GET: Legal Messages
        public ActionResult Legal(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            glb.UpdateReportRead(user_id, id.Value);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            CaseMessagesModel cm = new CaseMessagesModel(id.Value, 3, user_id);
            ViewBag.rm = rm;
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm._secondary_type_string + " - " + rm._location_string;
            // user
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);

            // user
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            glb.UpdateReadMessages(id.Value, user_id, 3);

            return View(cm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Legal([Bind(Include = "body_tx,report_id,sender_id,reporter_access")] message newMessage)
        {
            newMessage.created_dt = DateTime.Now;
            newMessage.ip_ds = "";
            newMessage.subject_ds = "";
            newMessage.reporter_access = 3;

            if (ModelState.IsValid)
            {
                db.message.Add(newMessage);
                try
                {
                    db.SaveChanges();
            ////// do not insert it to report log.        glb.UpdateReportLog(newMessage.sender_id, 7, newMessage.report_id, "", null, "");
                }
                catch (Exception ex)
                {
                   return View(newMessage);
                }
                return RedirectToAction("Legal/" + newMessage.report_id.ToString());
             // return RedirectToAction("Reporter/208");
            }

            return View(newMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Task([Bind(Include = "body,task_id,user_id")] task_comment newTask)
        {
            newTask.created_date = DateTime.Now;
            newTask.title = "";

            if (ModelState.IsValid)
            {
                db.task_comment.Add(newTask);
                try
                {
                    db.SaveChanges();
               ///     glb.UpdateReportLog(newMessage.sender_id, 7, newMessage.report_id, "", null, "");
                }
                catch (Exception ex)
                {
                    return View();
                }
                return RedirectToAction("Task/" + newTask.task_id.ToString());
            }

            return View();
        }

        // GET: Activity - Read-only
       
        public ActionResult Activity(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            UserModel um = new UserModel(user_id);
            glb.UpdateReportRead(user_id, id.Value);


            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            // user
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.um = um;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            return View();
        }
        [HttpGet]
        public ActionResult GetAjaxActivity(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Case");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
       ///     if (user == null || user.id == 0)
       ////         return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value);
            UserModel um = new UserModel(user_id);
            glb.UpdateReportRead(user_id, id.Value);


            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            // user
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.um = um;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            return PartialView();
        }
    
        // GET: Team
        public ActionResult Team(int? id, string popup)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            ReportModel rm = new ReportModel(id.Value); 
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            UserModel um = new UserModel(user_id);
            glb.UpdateReportRead(user_id, id.Value);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            // user
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.attachmentFiles = getAttachmentFiles(id.Value);
            ViewBag.um = um;
            ViewBag.rm = rm;
            ShowMediators(id.Value);
            if (popup != null)
            {
                ViewBag.popup = popup;
            }
            ViewMediatorsClass _mediators = new ViewMediatorsClass { _involved_mediators_user_list = rm._involved_mediators_user_list, _mediators_whoHasAccess_toReport = rm._mediators_whoHasAccess_toReport, _available_toAssign_mediators = rm._available_toAssign_mediators };
            return View(_mediators);
        }

        public class ViewMediatorsClass
        {
            public List<user> _involved_mediators_user_list { get; set; }
            public List<user> _mediators_whoHasAccess_toReport { get; set; }
            public List<user> _available_toAssign_mediators { get; set; }

        }

        public void ShowMediators(int id)
        {
            ReportModel rm = new ReportModel(id);

            ViewBag._involved_mediators_user_list = rm._involved_mediators_user_list;
            ViewBag._mediators_whoHasAccess_toReport = rm._mediators_whoHasAccess_toReport;
            ViewBag._available_toAssign_mediators = rm._available_toAssign_mediators;
        }

        [HttpPost]
        public JsonResult GetCMsList(int id)
        {
            ReportModel rm = new ReportModel(id);
            List<int> _list = rm._mediators_whoHasAccess_toReport.OrderBy(item=>item.role_id).Select(item =>item.id).ToList();
            return Json(_list, JsonRequestBehavior.AllowGet);
        }


         // GET: Messages
        public ActionResult Task(int? id)
        {
            
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            TaskExtended tsk = new TaskExtended(id.Value, user_id);

            ReportModel rm = new ReportModel(tsk.TaskReportID);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Index", "Account");

            UserModel um = new UserModel(user_id);
            glb.UpdateReportRead(user_id, tsk.TaskReportID);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = tsk.TaskReportID });
            }
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            // user
          //  UserModel um = new UserModel(user_id);
            ViewBag.report_id = tsk.TaskReportID; // 167-171
            ViewBag.user_id = user_id;
            ViewBag.task_id = id;
            ViewBag.taskStatus = tsk.TaskStatus;
            ViewBag.um = um;
            ViewBag.attachmentFiles = getAttachmentFiles(tsk.TaskReportID);
            ViewBag.AttachmentFilesTask = getAttachmentFilesTask(id.Value);

            glb.UpdateTaskRead(user_id, id.Value);

            List<Int32> _task_comments_ids = new List<Int32>();

            _task_comments_ids = db.task_comment.Where(a => a.id == id.Value).Select(t => t.id).ToList();
            foreach (int _id in _task_comments_ids)
            {
     /////           glb.UpdateTaskCommentRead(user_id, _id);
            }
           // if (!db.task_comment_user_read.Any(item => ((item.user_id == user_id) && (item.task_comment_id == task_comment_id))))
            // get all tasks comments 
            // glb.UpdateTaskRead(user_id, id.Value);



            return View();
        }
        public List<attachment> getAttachmentFilesTask(int report_id)
        {
            EC.Models.ReportModel rm = new EC.Models.ReportModel(report_id);
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_task_id == report_id)).ToList();
            return attachmentFiles;
        }
        public List<attachment> getAttachmentFiles(int report_id)
        {
            EC.Models.ReportModel rm = new EC.Models.ReportModel(report_id);
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == report_id)).ToList();
            return attachmentFiles;
        }

        public bool AddToMediators()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return false;

            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);

            userModel.AddToMediators(mediator_id, report_id);

            UserModel _um = new UserModel(mediator_id);
            glb.UpdateReportLog(user.id, 5, report_id, _um._user.first_nm + " " + _um._user.last_nm, null, "");


            #region Email to Newly Assigned Mediator

            if ((_um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_um._user.email.Trim()))
            {
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> bcc = new List<string>();

                to.Add(_um._user.email.Trim());
        //        if ((user.email.Trim().Length > 0) && glb.IsValidEmail(user.email.Trim()))
          //          cc.Add(user.email);
                ///     bcc.Add("timur160@hotmail.com");
                ReportModel _rm = new ReportModel(report_id);

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                eb.MediatorAssigned(_um._user.first_nm, _um._user.last_nm, user.first_nm, user.last_nm, _rm._report.display_name);
                string body = eb.Body;
                em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_MediatorAssigned, body, true);
            }
            

            #endregion



         //   ShowMediators(209);
         //   return RedirectToAction("Team/" + report_id.ToString());
            return true;
        }

        public int RemoveMediator()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return -1;

            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);

            UserModel _um = new UserModel(mediator_id);

            int tasks_number = _um.UserTasks(1, report_id, true).Count();
            if (tasks_number == 0)
            {
                userModel.RemoveMediator(mediator_id, report_id);
                glb.UpdateReportLog(user.id, 6, report_id, _um._user.first_nm + " " + _um._user.last_nm, null, "");
            }
            else
            {
                return tasks_number;
            }

            return 0;
        }

        public bool CreateNewTask()
        {
            return userModel.CreateNewTask(Request.Form, Request.Files);
        }

        public bool ReassignTask()
        {
            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int task_id = Convert.ToInt16(Request["task_id"]);

            return userModel.ReassignTask(task_id, mediator_id);
        }

        public bool CloseTask()
        {
            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int task_id = Convert.ToInt16(Request["task_id"]);

            return userModel.CloseTask(task_id, mediator_id);
        }

        public bool ResolveCase()
        {
            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);
            string description = Request["description"].ToString().Trim();
            int? outcome_id = Convert.ToInt16(Request["outcome_id"]);
            string outcome = Request["outcome"].ToString().Trim();

            ReportModel rm = new ReportModel(report_id);
            int new_status = rm._investigation_status + 1;
            // not escalated - just closed
            if (new_status == 5)
                new_status = 6;
      //      return true;
            return userModel.ResolveCase(report_id, mediator_id, description, new_status, outcome_id, outcome);
        }

        public bool NewStatus()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return false;

            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);
            int promotion_id = Convert.ToInt16(Request["promotion_id"]);
            string description = Request["description"].ToString().Trim();
            int? outcome_id = Convert.ToInt16(Request["outcome_id"]);
            string outcome = Request["outcome"].ToString().Trim();

            if(mediator_id != user.id)
                return false;

            ReportModel rm = new ReportModel(report_id);
            UserModel um = new UserModel(user.id);
            bool has_access = rm.HasAccessToReport(user.id);

            if ((!has_access) || (user.role_id == 8))
            {
                return false;
            }
            if ((user.role_id != 4) && (user.role_id != 5) && (rm._investigation_status == Constant.investigation_status_closed))
            {
                return false;
            }

            int new_status_id = 3;

            if ((promotion_id == 1) && (rm._investigation_status == Constant.investigation_status_investigation))
            {
                new_status_id = Constant.investigation_status_completed;
                glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Started, null, "");
            }

            if (promotion_id == 0)
            {

                switch (rm._investigation_status)
                {
                    case 1:
                        new_status_id = Constant.investigation_status_review;
                        break;
                    case 2:
                        new_status_id = Constant.investigation_status_investigation;
                        break;
                    case 3:
                        new_status_id = Constant.investigation_status_resolution;
                        glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Started, null, "");
                        break;
                    case 4:
                        glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 25, report_id,"", null, "");
                        new_status_id = Constant.investigation_status_closed;
                        break;
                    case 6:
                        new_status_id = Constant.investigation_status_closed;
                        glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 25, report_id,"", null, "");
                        break;
                    case 9:
                        new_status_id = Constant.investigation_status_investigation;
                        glb.UpdateReportLog(user.id, 29, report_id, "", null, description);
                        break;
                    default:
                        new_status_id = Constant.investigation_status_investigation;
                        break;
                }
            }
            if ((promotion_id == 2))
            {
                new_status_id = Constant.investigation_status_spam;
            }


            bool _new = userModel.ResolveCase(report_id, mediator_id, description, new_status_id, outcome_id, outcome);
            return true;

        }


        public int CloseCase()
        {
         //   return 2;
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return -1;


            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);
            int promotion_value = Convert.ToInt16(Request["promotion_value"]);
            string description = Request["description"].ToString().Trim();
            int outcome_id = Convert.ToInt16(Request["outcome_id"]);
            string outcome = String.Empty;
            if (Request["outcome"]!=null)
            {
                outcome = Request["outcome"].ToString().Trim();
            }

            if (mediator_id != user.id)
                return -1;
            ReportModel rm = new ReportModel(report_id);
            UserModel um = new UserModel(user.id);
            bool has_access = rm.HasAccessToReport(user.id);

            if ((!has_access) || (user.role_id == 8))
            {
                return -1;
            }
            if (promotion_value == Constant.investigation_status_resolution)
            {
                CompanyModel cm = new CompanyModel(um._user.company_id);
                if (cm.AllMediators(cm._company.id, true, Constant.level_escalation_mediator).Count == 0)
                    return 0;
            }


            switch (promotion_value)
            {
                case 3:
                    if (rm._investigation_status == 9)
                        glb.UpdateReportLog(user.id, 29, report_id, "", null, description);

                    if (rm._investigation_status == 6)
                        glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    else if (rm._investigation_status == 3)
                        glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);

                    glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Started, null, description);

                    break;
                case 4:
                    glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Started, null, "");
                    break;
                case 6:
                    glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Started, null, "");
                    break;

                case 9:
                    if(rm._investigation_status == 6)
                        glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    else if (rm._investigation_status == 4)
                        glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    
                        glb.UpdateReportLog(user.id, 25, report_id, "", null, description);
                    break;


            }
            



            bool _new = userModel.ResolveCase(report_id, mediator_id, description, promotion_value, outcome_id, outcome);

            return 1;
        }
    }
}