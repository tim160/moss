using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Models.ViewModels;
using EC.Models;
using EC.Models.ECModel;

namespace EC.Controllers
{
    
    public class NewCaseController : BaseController
    {
        public ECEntities db = new ECEntities();
        
        // GET: NewCase
        public ActionResult Index(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account", new { returnUrl = Request.Url.LocalPath });

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");


            int user_id = user.id;

            glb.UpdateReportRead(user_id, report_id);

            ViewBag.user_id = user_id;
            ViewBag.report_id = report_id;
            ViewBag.attachmentFiles = getAttachmentFiles(report_id);
            
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion
            
            HeaderPos headerPos = new HeaderPos { cc_extension = cc_ext, _investigation_status = rm._investigation_status };
            ViewBag.headerPos = headerPos;
            return View();
        }
        public List<attachment> getAttachmentFiles(int report_id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == report_id && !item.visible_mediators_only.HasValue && !item.visible_reporter.HasValue)).ToList();
            return attachmentFiles;
        }

        public ActionResult InvestigationNotes(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            ViewBag.user_id = user_id;
            ViewBag.report_id = report_id;
            ViewBag.attachmentFiles = getAttachmentFiles(report_id);

            glb.UpdateReportRead(user_id, report_id);

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion

            HeaderPos headerPos = new HeaderPos { cc_extension = cc_ext, _investigation_status = rm._investigation_status };
            ViewBag.headerPos = headerPos;
            return View();
        }
        public ActionResult CaseClosureReport(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion

            

            int user_id = user.id;

            glb.UpdateReportRead(user_id, report_id);

            ViewBag.user_id = user_id;
            ViewBag.report_id = report_id;
            ViewBag.attachmentFiles = getAttachmentFiles(report_id);

            return View();
        }

        public ActionResult Activity(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            glb.UpdateReportRead(user_id, report_id);
            ViewBag.user_id = user_id;
            ViewBag.report_id = report_id;

            return View();
        }

        public ActionResult Tasks(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            List<task> tasks = rm.ReportTasks(0);

            glb.UpdateReportRead(user_id, report_id);

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
            ViewBag.attachmentFiles = getAttachmentFiles(report_id);
            ViewBag.report_id = report_id; // 167-171
            ViewBag.user_id = user_id;

            return View();
        }

        public bool CreateNewTask()
        {
            UserModel userModel = UserModel.inst;
            return userModel.CreateNewTask(Request.Form, Request.Files);
        }

        public ActionResult Messages(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            UserModel um = new UserModel(user_id);

            glb.UpdateReportRead(user_id, report_id);
            glb.UpdateReadMessages(report_id, user_id, 2);

            ViewBag.um = um;
            ViewBag.report_id = report_id; // 167-171
            ViewBag.user_id = user_id;

            return View();
        }

        public ActionResult Reporter(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            UserModel um = new UserModel(user_id);

            glb.UpdateReportRead(user_id, report_id);
            glb.UpdateReadMessages(report_id, user_id, 2);

            ViewBag.um = um;
            ViewBag.report_id = report_id; // 167-171
            ViewBag.user_id = user_id;

            return View("Messages");
        }

        public ActionResult Team(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
            ViewBag.user_id = user_id;
            ViewBag.report_id = report_id;

            glb.UpdateReportRead(user_id, report_id);

            return View();
        }

        public ActionResult Attachments(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Index", "Account");

            glb.UpdateReportRead(user.id, report_id);

            ViewBag.rm = rm;
            ViewBag.report_id = report_id;
            ViewBag.user_id = user.id;
            ViewBag.attachmentFiles = getAttachmentFiles(report_id);
            var files = db.attachment
                .Where(item => item.report_id == report_id && item.status_id == 2 && (item.visible_mediators_only.HasValue || item.visible_reporter.HasValue || item.user_id == rm._report.reporter_user_id))
                .ToList(); ;
            var users = files.Select(x => x.user_id).ToList();
            ViewBag.attachmentAdvFiles = files;
            ViewBag.attachmentAdvUsers = db.user.Where(x => users.Contains(x.id)).ToList();
            ViewBag.popup = null;

            var report_secondary_type = db.report_secondary_type.Where(x => x.report_id == report_id);
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
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength == 0)
                    {
                        continue;
                    }
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
            return RedirectToAction("Attachments", new { report_id = report_id });
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

            return RedirectToAction("Attachments", new { report_id = id });
        }

        public List<attachment> getAttachmentFilesTask(int task_id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_task_id == task_id)).ToList();
            return attachmentFiles;
        }

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
    }
}