﻿using System;
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
using EC.Constants;

namespace EC.Controllers
{
    public class CaseController : BaseController
    {

        private readonly UserModel userModel = UserModel.inst;
        private readonly CompanyModel companyModel =new CompanyModel();
        private readonly ReportModel reportModel =new ReportModel();
        private IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();


        public ActionResult Index(int? id, string popup)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm.SecondaryTypeString() + " - " + rm.LocationString();

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

                    foreach (user _user in rm.MediatorsWhoHasAccessToReport())
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

                            /* /////tim Added email saving
                            notification_processed np = new notification_processed();
                            np.receivers = _user.email.Trim();
                            np.notification_text = body;
                            np.subject = App_LocalResources.GlobalRes.Email_Title_NewMessage;
                            db.notification_processed.Add(np);
                            using (ECEntities adv = new ECEntities())
                            {
                                adv.SaveChanges();
                            }
                            */
                        }
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());

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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm.SecondaryTypeString() + " - " + rm.LocationString();
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

                    foreach (user _user in rm.MediatorsWhoHasAccessToReport())
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
                    logger.Error(ex.ToString());
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            ViewBag.report_name = " - #" + rm._report.display_name + " - " + rm.SecondaryTypeString() + " - " + rm.LocationString();
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
                    logger.Error(ex.ToString());
                   return View(newMessage);
                }
                return RedirectToAction("Legal/" + newMessage.report_id.ToString());
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
                    logger.Error(ex.ToString());
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            ViewMediatorsClass _mediators = new ViewMediatorsClass { _involved_mediators_user_list = rm.InvolvedMediatorsUserList(), _mediators_whoHasAccess_toReport = rm.MediatorsWhoHasAccessToReport(), _available_toAssign_mediators = rm.AvailableToAssignMediators() };
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

            ViewBag._involved_mediators_user_list = rm.InvolvedMediatorsUserList();
            ViewBag._mediators_whoHasAccess_toReport = rm.MediatorsWhoHasAccessToReport();
            ViewBag._available_toAssign_mediators = rm.AvailableToAssignMediators();
        }

        [HttpPost]
        public JsonResult GetCMsList(int id)
        {
            ReportModel rm = new ReportModel(id);
            List<int> _list = rm.MediatorsWhoHasAccessToReport().OrderBy(item=>item.role_id).Select(item =>item.id).ToList();
            return Json(_list, JsonRequestBehavior.AllowGet);
        }


         // GET: Messages
        public ActionResult Task(int? id)
        {
            
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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

        public List<attachment> getAttachmentFilesTask(int task_id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_task_id == task_id)).ToList();
            return attachmentFiles;
        }

        /// <summary>
        /// initial report attachments
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public List<attachment> getAttachmentFiles(int report_id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == report_id && !item.visible_mediators_only.HasValue && !item.visible_reporter.HasValue)).ToList();
            return attachmentFiles;
        }


        public bool AddToMediators()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
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
            string outcome = String.Empty;
            if (Request["outcome"] != null)
            {
                outcome = Request["outcome"].ToString().Trim();
            }

            ReportModel rm = new ReportModel(report_id);
            int new_status = rm._investigation_status + 1;
            // not escalated - just closed
            if (new_status == 5)
                new_status = 6;
      //      return true;
            int? outcome_id = 0;
            if (Request["outcome_id"] != "")
            {
                outcome_id = Convert.ToInt16(Request["outcome_id"]);
            }
            int? reason_id = 0;
            if (Request["case_closure_reason_id"] != "")
            {
                reason_id = Convert.ToInt16(Request["case_closure_reason_id"]);
            }
            string executive_summary = "";
            if(Request["executive_summary"]!= null)
                executive_summary = Request["executive_summary"].ToString().Trim();
            string facts_established = "";
            if (Request["facts_established"] != null)
                facts_established = Request["facts_established"].ToString().Trim();
            string investigation_methodology = "";
            if (Request["investigation_methodology"] != null)
                investigation_methodology = Request["investigation_methodology"].ToString().Trim();
            string description_outcome = "";
            if (Request["description_outcome"] != null)
                description_outcome = Request["description_outcome"].ToString().Trim();
            string recommended_actions = "";
            if (Request["recommended_actions"] != null)
                recommended_actions = Request["recommended_actions"].ToString().Trim();

            int sign_off_mediator_id = 0;
            if (Request["sign_off_mediator_id"] != null)
                sign_off_mediator_id = Convert.ToInt32(Request["sign_off_mediator_id"]);

            bool? is_clery_act_crime = null;
            if (Request["is_clery_act_crime"] != null)
                is_clery_act_crime = Convert.ToBoolean(Request["is_clery_act_crime"]);

            int crime_statistics_location_id = 0;
            if (Request["crime_statistics_location_id"] != null)
                crime_statistics_location_id = Convert.ToInt32(Request["crime_statistics_location_id"]);

            int crime_statistics_category_id = 0;
            if (Request["crime_statistics_category_id"] != null)
                crime_statistics_category_id = Convert.ToInt32(Request["crime_statistics_category_id"]);


            bool _new = userModel.ResolveCase(report_id, mediator_id, description, new_status,reason_id, sign_off_mediator_id);
            if (_new)
            {
                #region Email Ready
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> bcc = new List<string>();

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                string body = "";
                #endregion

                #region Email To Mediators About Case Update
                rm = new ReportModel(report_id);
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

       ////////                 em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                    }
                }
                #endregion
            }
            return _new;

        }

        public bool NewStatus()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return false;

            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);
            int promotion_id = Convert.ToInt16(Request["promotion_id"]);
            string description = Request["description"].ToString().Trim();

            string outcome = String.Empty;
            if (Request["outcome"] != null)
            {
                outcome = Request["outcome"].ToString().Trim();
            }

            int? outcome_id = 0;
            if (Request["outcome_id"] != "")
            {
                outcome_id = Convert.ToInt16(Request["outcome_id"]);
            }
            int reason_id = 0;
            if (Request["case_closure_reason_id"] != "")
            {
                reason_id = Convert.ToInt16(Request["case_closure_reason_id"]);
            }
            string executive_summary = "";
            if (Request["executive_summary"] != null)
                executive_summary = Request["executive_summary"].ToString().Trim();
            string facts_established = "";
            if (Request["facts_established"] != null)
                facts_established = Request["facts_established"].ToString().Trim();
            string investigation_methodology = "";
            if (Request["investigation_methodology"] != null)
                investigation_methodology = Request["investigation_methodology"].ToString().Trim();
            string description_outcome = "";
            if (Request["description_outcome"] != null)
                description_outcome = Request["description_outcome"].ToString().Trim();
            string recommended_actions = "";
            if (Request["recommended_actions"] != null)
                recommended_actions = Request["recommended_actions"].ToString().Trim();


            int sign_off_mediator_id = 0;
            if (Request["sign_off_mediator_id"] != null)
                sign_off_mediator_id = Convert.ToInt32(Request["sign_off_mediator_id"]);

            bool? is_clery_act_crime = null;
            if (Request["is_clery_act_crime"] != null)
                is_clery_act_crime = Convert.ToBoolean(Request["is_clery_act_crime"]);

            int crime_statistics_location_id = 0;
            if (Request["crime_statistics_location_id"] != null)
                crime_statistics_location_id = Convert.ToInt32(Request["crime_statistics_location_id"]);

            int crime_statistics_category_id = 0;
            if (Request["crime_statistics_category_id"] != null)
                crime_statistics_category_id = Convert.ToInt32(Request["crime_statistics_category_id"]);



            if (mediator_id != user.id)
                return false;

            ReportModel rm = new ReportModel(report_id);
            UserModel um = new UserModel(user.id);
            bool has_access = rm.HasAccessToReport(user.id);

            if ((!has_access) || (user.role_id == 8))
            {
                return false;
            }
            if ((user.role_id != 4) && (user.role_id != 5) && (rm._investigation_status == ECGlobalConstants.investigation_status_closed))
            {
                return false;
            }

            int new_status_id = 3;

            if ((promotion_id == 1) && (rm._investigation_status == ECGlobalConstants.investigation_status_investigation))
            {
                new_status_id = ECGlobalConstants.investigation_status_completed;
  ///////              glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Started, null, "");
            }

            if (promotion_id == 0)
            {

                switch (rm._investigation_status)
                {
                    case 1:
                        new_status_id = ECGlobalConstants.investigation_status_review;
                        break;
                    case 2:
                        new_status_id = ECGlobalConstants.investigation_status_investigation;
                        break;
                    case 3:
                        new_status_id = ECGlobalConstants.investigation_status_resolution;
        ////                glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Started, null, "");
                        break;
                    case 4:
              /////          glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 25, report_id,"", null, "");
                        new_status_id = ECGlobalConstants.investigation_status_closed;
                        break;
                    case 6:
                        new_status_id = ECGlobalConstants.investigation_status_closed;
         ////               glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                        glb.UpdateReportLog(user.id, 25, report_id,"", null, "");
                        break;
                    case 9:
                        new_status_id = ECGlobalConstants.investigation_status_investigation;
                        glb.UpdateReportLog(user.id, 29, report_id, "", null, description);
                        break;
                    default:
                        new_status_id = ECGlobalConstants.investigation_status_investigation;
                        break;
                }
            }
            if ((promotion_id == 2))
            {
                new_status_id = ECGlobalConstants.investigation_status_spam;
            }

            bool _new = userModel.ResolveCase(report_id, mediator_id, description, new_status_id, reason_id, sign_off_mediator_id);

            if (_new)
            {
                #region Email Ready
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> bcc = new List<string>();

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                string body = "";
                #endregion

                #region Email To Mediators About Case Update
                rm = new ReportModel(report_id);
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        to = new List<string>();
                        cc = new List<string>();
                        bcc = new List<string>();

                        to.Add(_user.email.Trim());

         //////               eb.NextStep(_user.first_nm, _user.last_nm, rm._report.display_name);
                        body = eb.Body;

         ///////               em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                    }
                }
                #endregion
            
            
            
            }
            return true;

        }

        public int CloseCaseValidate()
        {
            int report_id = Convert.ToInt16(Request["report_id"]);
            ReportModel rm = new ReportModel(report_id);

            if (!rm.getSecondaryTypeMandatory().Any())
            {
                return -2;
            }
            if ((!db.report_mediator_involved.Any(x => x.report_id == report_id)) && (!db.report_non_mediator_involved.Any(x => x.report_id == report_id)))
            {
                return -2;
            }
            var note1 = db.report_inv_notes.FirstOrDefault(x => x.report_id == report_id & x.type == 1)?.note;
            var note2 = db.report_inv_notes.FirstOrDefault(x => x.report_id == report_id & x.type == 2)?.note;

            if ((String.IsNullOrEmpty(note1)) || (String.IsNullOrEmpty(note2)))
            {
                return -2;
            }

            var list = db.report_non_mediator_involved.Where(x => x.report_id == report_id && x.role_in_report_id == 3).Select(x => x.id).ToList();
            var list_cco = db.report_case_closure_outcome.Where(x => x.report_id == report_id).ToList();
            if ((list.Any()) && (!list_cco.Any(x => x.non_mediator_involved_id.HasValue && list.Contains(x.non_mediator_involved_id.Value))))
            {
                return -2;
            }

            return 0;
        }

        public int CloseCase()
        {
         //   return 2;
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return -1;

            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int report_id = Convert.ToInt16(Request["report_id"]);
            int promotion_value = Convert.ToInt16(Request["promotion_value"]);
            string description = Request["description"].ToString().Trim();

            int reason_id = 0;
            if (Request["case_closure_reason_id"] != "")
            {
                reason_id = Convert.ToInt16(Request["case_closure_reason_id"]);
            }

            int sign_off_mediator_id = 0;
            if (Request["sign_off_mediator_id"] != null)
                sign_off_mediator_id = Convert.ToInt32(Request["sign_off_mediator_id"]);
            
            if (mediator_id != user.id)
                return -1;

            UserModel um = new UserModel(user.id);
            ReportModel rm = new ReportModel(report_id);
            bool has_access = rm.HasAccessToReport(user.id);

            int old_status = rm._investigation_status;

            if ((!has_access) || (user.role_id == 8))
            {
                return -1;
            }


            bool _new = userModel.ResolveCase(report_id, mediator_id, description, promotion_value, reason_id,sign_off_mediator_id);

            if (!db.report_mediator_assigned.Any(x => x.report_id == report_id && x.mediator_id == sign_off_mediator_id))
            {
                db.report_mediator_assigned.Add(new report_mediator_assigned
                {
                    report_id = report_id,
                    mediator_id = sign_off_mediator_id,
                    assigned_dt = DateTime.Now,
                    status_id = 2,
                    last_update_dt = DateTime.Now,
                    user_id = user.id,
                });
                db.SaveChanges();
            }

            if (sign_off_mediator_id != 0)
            {
                report_signoff_mediator report_signoff_mediator = null;
                foreach (var item in db.report_signoff_mediator.Where(x => x.report_id == report_id))
                {
                    item.status_id = 1;
                    if (item.user_id == sign_off_mediator_id)
                    {
                        report_signoff_mediator = item;
                    }
                }
                if (report_signoff_mediator == null)
                {
                    report_signoff_mediator = new report_signoff_mediator
                    {
                        createdby_user_id = user.id,
                        created_on = DateTime.Now,
                        report_id = report_id,
                        user_id = sign_off_mediator_id,
                    };
                    db.report_signoff_mediator.Add(report_signoff_mediator);
                }
                report_signoff_mediator.status_id = 2;
                db.SaveChanges();
            }

            if (_new)
            {
                #region Email Ready
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> bcc = new List<string>();

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                string body = "";
                #endregion

                #region Email To Mediators About Case Update
                foreach (user _user in rm.MediatorsWhoHasAccessToReport())
                {
                    if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                    {
                        to = new List<string>();
                        cc = new List<string>();
                        bcc = new List<string>();

                        to.Add(_user.email.Trim());
                        UserModel um_temp = new UserModel(_user.id);
                        if ((promotion_value == ECGlobalConstants.investigation_status_resolution || promotion_value == ECGlobalConstants.investigation_status_completed) && um_temp._user.id == sign_off_mediator_id)
                        {
                            eb.CaseCloseApprove( rm._report.display_name);
                            body = eb.Body;
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                        }
                        else if((promotion_value == ECGlobalConstants.investigation_status_resolution || promotion_value == ECGlobalConstants.investigation_status_completed) && um_temp._user.role_id == 4)
                        {
                            eb.CaseCloseApprovePlatformManager(rm._report.display_name);
                            body = eb.Body;
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NextStep, body, true);
                        }
                    }
                }
                #endregion
            }
            switch (promotion_value)
            {

                case 3:
                    if (old_status == 9)
                        glb.UpdateReportLog(user.id, 29, report_id, "", null, description);

                    if (old_status == 6)
                    {
                     //////   glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    }
                    else if (old_status == 3)
                    {
                     /////   glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    }
                    else
                        glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Started, null, description);
                    /* if (rm._investigation_status == 9)
                         glb.UpdateReportLog(user.id, 29, report_id, "", null, description);

                     if (rm._investigation_status == 6)
                         glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                     else if (rm._investigation_status == 3)
                         glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                     else
                         glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Started, null, description);
                         */
                    break;
                case 4:
                   
         //////           glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Started, null, "");
                    break;
                case 6:
       ///////             glb.UpdateReportLog(user.id, 21, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Started, null, "");
                    break;

                case 9:

                    if (old_status == 6)
                        glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);


     ////               if (rm._investigation_status == 6)
    ////                    glb.UpdateReportLog(user.id, 27, report_id, App_LocalResources.GlobalRes._Completed, null, description);
   /////                 else if (rm._investigation_status == 4)
 /////                       glb.UpdateReportLog(user.id, 22, report_id, App_LocalResources.GlobalRes._Completed, null, description);
                    
                        glb.UpdateReportLog(user.id, 25, report_id, "", null, description);
                    break;


            }




       
            return 1;
        }

        public ActionResult Attachments(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            var rm = new ReportModel(id);
            ViewBag.rm = rm;
            ViewBag.report_id = id;
            ViewBag.user_id = user.id;
            ViewBag.attachmentFiles = getAttachmentFiles(id);
            var files = db.attachment
                .Where(item => item.report_id == id && item.status_id == 2 && (item.visible_mediators_only.HasValue || item.visible_reporter.HasValue || item.user_id == rm._report.reporter_user_id))
                .ToList(); ;
            var users = files.Select(x => x.user_id).ToList();
            ViewBag.attachmentAdvFiles = files;
            ViewBag.attachmentAdvUsers = db.user.Where(x => users.Contains(x.id)).ToList();
            ViewBag.popup = null;

            var report_secondary_type = db.report_secondary_type.Where(x => x.report_id == id);
            ViewBag.report_secondary_type = db.company_secondary_type
                    .Where(x => x.company_id == rm._report.company_id)
                    .Where(x => report_secondary_type.Select(z => z.secondary_type_id).Contains(x.id))
                    .OrderBy(x => x.secondary_type_en)
                    .ToList();

            var company_case_routing = db.company_case_routing.Where(x => x.company_id == rm._report.company_id).ToList();
            var ids = company_case_routing.Select(x => x.id).ToList();
            ViewBag.company_case_routing = company_case_routing;
            ViewBag.company_case_routing_attachments = db.company_case_routing_attachments
                .Where(x => ids.Contains(x.company_case_routing_id) & x.status_id == 2)
                .OrderBy(x => x.company_case_routing_id)
                .ThenBy(x => x.file_nm)
                .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Attachments(int report_id, string mode, string type)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //user = user ?? db.user.FirstOrDefault(x => x.id == 167);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            if ((mode == "upload") || (mode == "upload_rd"))
            {
                for(int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var report = db.report.FirstOrDefault(x => x.id == report_id);

                    var a = new attachment();
                    a.file_nm = file.FileName;
                    a.extension_nm = System.IO.Path.GetExtension(file.FileName);
                    a.path_nm = "";
                    a.report_id = report_id;
                    a.status_id = 2;
                    a.effective_dt = DateTime.Now;
                    a.expiry_dt = DateTime.Now;
                    a.last_update_dt = DateTime.Now;
                    a.user_id = user.id;

                    type = mode == "upload_rd" ? "reporter" : "staff";
                    a.visible_reporter = type == "reporter" ? true : false;
                    a.visible_mediators_only = type == "staff" ? true : false;

                    db.attachment.Add(a);
                    db.SaveChanges();

                    var dir = Server.MapPath(String.Format("~/upload/reports/{0}", report.guid));
                    var filename = String.Format("{0}_{1}{2}", user.id, DateTime.Now.Ticks, System.IO.Path.GetExtension(file.FileName));
                    a.path_nm = String.Format("\\upload\\reports\\{0}\\{1}", report.guid, filename);
                    db.SaveChanges();

                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    file.SaveAs(string.Format("{0}\\{1}", dir, filename));
                }
            }
            if (mode == "upload_rd")
            {
                return RedirectToAction("Attachments", "ReporterDashboard", new { id = user.id });
            }
            return RedirectToAction("Attachments", new { id = report_id });
        }

        [HttpPost]
        public ActionResult AttachmentDelete(int report_id, int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //user = user ?? db.user.FirstOrDefault(x => x.id == 167);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            var file = db.attachment.FirstOrDefault(x => x.report_id == report_id & x.id == id);
            file.status_id = 1;
            db.SaveChanges();

            return RedirectToAction("Attachments", new { id = report_id });
        }

        [HttpPost]
        public ActionResult AttachmentType(int id, int file_id, int type)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //user = user ?? db.user.FirstOrDefault(x => x.id == 167);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            var file = db.attachment.FirstOrDefault(x => x.report_id == id & x.id == file_id);
            file.visible_mediators_only = type == 1;
            file.visible_reporter = type == 2;
            db.SaveChanges();

            return RedirectToAction("Attachments", new { id = id });
        }

        [HttpPost]
        public bool MakeCaseOwner(int user_id, int report_id)
        {
            foreach(var item in db.report_owner.Where(x => x.report_id == report_id & x.status_id != 1).ToList())
            {
                item.report_id = 1;
            }

            var owner = db.report_owner.FirstOrDefault(x => x.report_id == report_id  & x.user_id == user_id);
            if (owner != null)
            {
                owner.status_id = 2;
            }
            else
            {
                owner = new report_owner
                {
                    created_on = DateTime.Now,
                    report_id = report_id,
                    status_id = 2,
                    user_id = user_id,
                };
                db.report_owner.Add(owner);
            }
            db.SaveChanges();

            return true;
        }
    }
}