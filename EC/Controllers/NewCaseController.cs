using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Models.ViewModels;
using EC.Models;
using EC.Models.ECModel;
using Rotativa.MVC;
using EC.Localization;
using EC.Business.Actions;

namespace EC.Controllers
{

    public class NewCaseController : BaseController
    {
        private TaskExtended taskService = new TaskExtended();
        // GET: NewCase
        public ActionResult Index(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service", new { returnUrl = Request.Url.LocalPath });

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");
            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;

            readStatusModel.UpdateReportRead(user_id, id);

            ViewBag.user_id = user_id;
            ViewBag.report_id = id;
            ViewBag.guid = rm._report.guid;
            ViewBag.attachmentFiles = getAttachmentFiles(id);

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion

            HeaderPos headerPos = new HeaderPos { cc_extension = cc_ext, _investigation_status = rm._investigation_status };
            ViewBag.headerPos = headerPos;
            return View();
        }

        public ActionResult InvestigationNotes(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;
            ViewBag.user_id = user_id;
            ViewBag.report_id = id;
            ViewBag.attachmentFiles = getAttachmentFiles(id);

            readStatusModel.UpdateReportRead(user_id, id);

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion

            HeaderPos headerPos = new HeaderPos { cc_extension = cc_ext, _investigation_status = rm._investigation_status };
            ViewBag.headerPos = headerPos;
            return View();
        }
        public ActionResult CaseClosureReport(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            #endregion



            int user_id = user.id;

            readStatusModel.UpdateReportRead(user_id, id);

            ViewBag.user_id = user_id;
            ViewBag.report_id = id;
            ViewBag.attachmentFiles = getAttachmentFiles(id);

            return View();
        }

        public ActionResult Activity(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;

            readStatusModel.UpdateReportRead(user_id, id);
            ViewBag.user_id = user_id;
            ViewBag.report_id = id;

            return View();
        }

        public ActionResult Tasks(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            List<task> tasks = rm.ReportTasks(0);

            readStatusModel.UpdateReportRead(user_id, id);

            List<TaskExtended> list_tsk = new List<TaskExtended>();
            int task_id = 0;
            foreach (task _task in tasks)
            {
                task_id = _task.id;
                TaskExtended tsk = new TaskExtended(_task.id, user_id);
                tsk.HasTaskFile();
                list_tsk.Add(tsk);
            }
            ViewBag.tasks = list_tsk;
            ViewBag.um = um;
            ViewBag.attachmentFiles = getAttachmentFiles(id);
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;

            return View();
        }

        public ActionResult Messages(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;
            UserModel um = new UserModel(user_id);

            readStatusModel.UpdateReportRead(user_id, id);
            readStatusModel.UpdateReadMessages(id, user_id, 2);

            ViewBag.um = um;
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;

            return View();
        }

        public ActionResult Reporter(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;
            UserModel um = new UserModel(user_id);

            readStatusModel.UpdateReportRead(user_id, id);
            readStatusModel.UpdateReadMessages(id, user_id, 1);

            ViewBag.um = um;
            ViewBag.report_id = id; // 167-171
            ViewBag.user_id = user_id;

            return View("Messages");
        }

        public ActionResult Team(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

            int user_id = user.id;
            ViewBag.user_id = user_id;
            ViewBag.report_id = id;

            readStatusModel.UpdateReportRead(user_id, id);

            return View();
        }

        public ActionResult Attachments(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            readStatusModel.UpdateReportRead(user.id, id);

            if ((rm._investigation_status == 1) || (rm._investigation_status == 2) || (rm._investigation_status == 7))
            {
                // case is not approved to work on it yet, need to approve first. if == 7 - its spam, so they will share the view.
                return RedirectToAction("Index", "NewReport", new { id = id });
            }

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

        public ActionResult AttachmentDelete(int id, int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(report_id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            var att = db.attachment.FirstOrDefault(x => x.id == id && x.report_id == report_id);
            if (att != null)
            {
                if (att.user_id == user.id)
                {
                    db.attachment.Remove(att);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Attachments", new { id = id });
        }

        [HttpPost]
        public ActionResult Attachments(int id, string mode, string type)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = user ?? db.user.FirstOrDefault(x => x.id == 2);
            //user = user ?? db.user.FirstOrDefault(x => x.id == 167);
            //
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if ((mode == "upload") || (mode == "upload_rd"))
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength == 0)
                    {
                        continue;
                    }
                    var report = db.report.FirstOrDefault(x => x.id == id);

                    var a = new attachment();
                    a.file_nm = file.FileName;
                    a.extension_nm = System.IO.Path.GetExtension(file.FileName);
                    a.path_nm = "";
                    a.report_id = id;
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
            return RedirectToAction("Attachments", new { report_id = id });
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
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            var file = db.attachment.FirstOrDefault(x => x.report_id == id & x.id == file_id);
            file.visible_mediators_only = type == 1;
            file.visible_reporter = type == 2;
            db.SaveChanges();

            return RedirectToAction("Attachments", new { report_id = id });
        }

        public ActionResult Task(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Cases");

            //int user_id = 2;
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            TaskExtended tsk = new TaskExtended(id.Value, user_id);

            ReportModel rm = new ReportModel(tsk.TaskReportID);
            if (!rm.HasAccessToReport(user_id))
                return RedirectToAction("Login", "Service");

            UserModel um = new UserModel(user_id);
            readStatusModel.UpdateReportRead(user_id, tsk.TaskReportID);

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

            readStatusModel.UpdateTaskRead(user_id, id.Value);

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
                user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
                if (user == null || user.id == 0)
                    return RedirectToAction("Login", "Service");

                ReportModel rm = new ReportModel(newTask.id);
                if (!rm.HasAccessToReport(user.id))
                    return RedirectToAction("Login", "Service");

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

        public ActionResult ReassignTask(int id, int mediator_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ReportModel rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            taskService.ReassignTask(id, mediator_id, is_cc, Session);
            return RedirectToAction("Task", new { id = id });
        }

        public ActionResult PrintToPdf(int id, Guid? rg, Guid? ug, bool pdf = true)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if ((user == null) && (rg.HasValue) && (ug.HasValue))
            {
                user = db.user.FirstOrDefault(x => x.guid == ug);
            }
            if (user == null || user.id == 0)
            {
                return RedirectToAction("Login", "Service");
            }

            var rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if (pdf)
            {
                var report = db.report.FirstOrDefault(x => x.id == id);
                var fn = $"{rm.CompanyName()} Case Closure Report {rm._report.display_name}";
                //return new ActionAsPdf("PrintToPdf", new { id = id, rg = report.guid, ug = user.guid, pdf = false }) { FileName = fn };
                return new ActionAsPdf("PrintToPdf", new { id = id, rg = report.guid, ug = user.guid, pdf = false }) { };
            }

            ViewBag.user_id = user.id;
            return View(rm);
        }

        public ActionResult PrintToPdfOriginal(Guid id, bool pdf = true)
        {
            var report = db.report.FirstOrDefault(x => x.guid == id);

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            var rm = new ReportModel(report.id);
            if (!rm.HasAccessToReport(user.id))
                return RedirectToAction("Login", "Service");

            if (pdf)
            {
                var fn = $"Report to {rm.CompanyName()}";
                //return new ActionAsPdf("PrintToPdf", new { id = id, pdf = false }) { FileName = fn };
                return new ActionAsPdf("PrintToPdfOriginal", new { id = id, pdf = false }) { };
            }

            ViewBag.Roles = db.role_in_report.ToList();
            return View(rm);
            //return new ReportController().PrintToPdf(id);
        }

        public List<attachment> getAttachmentFilesTask(int id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return null;

            var rm = new ReportModel(id);
            if (!rm.HasAccessToReport(user.id))
                return null;

            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_task_id == id)).ToList();
            return attachmentFiles;
        }

        public bool CloseTask()
        {
            int mediator_id = Convert.ToInt16(Request["user_id"]);
            int task_id = Convert.ToInt16(Request["task_id"]);
            return taskService.CloseTask(task_id, mediator_id, Session);
        }


        public List<attachment> getAttachmentFiles(int id)
        {
            List<attachment> attachmentFiles = db.attachment.Where(item => (item.report_id == id && !item.visible_mediators_only.HasValue && !item.visible_reporter.HasValue)).ToList();
            return attachmentFiles;
        }

        public bool CreateNewTask()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return false;
            return taskService.CreateNewTask(Request.Form, Request.Files, is_cc, Session);
        }

        public int CloseCaseValidate()
        {
            int report_id = Convert.ToInt16(Request["report_id"]);
            ReportModel rm = new ReportModel(report_id);
            GetDBEntityModel getDBEntityModel = new GetDBEntityModel();
            if (!getDBEntityModel.GetSecondaryTypeMandatory().Any())
            {
                return -2;
            }
            if ((!db.report_mediator_involved.Any(x => x.report_id == report_id)) && (!db.report_non_mediator_involved.Any(x => x.report_id == report_id)))
            {
                //return -2;
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


            bool was_case_promoted = um.PromoteCase(report_id, mediator_id, description, promotion_value, reason_id, sign_off_mediator_id);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (sign_off_mediator_id != 0 && promotion_value == (int)CaseStatusConstants.CaseStatusValues.Completed)
                    {
                        #region Save to Assign Mediator from drop
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
                        }
                        else if (db.report_mediator_assigned.Any(x => x.report_id == report_id && x.mediator_id == sign_off_mediator_id && x.status_id != 2))
                        {
                            var inactive_mediator = db.report_mediator_assigned.Where(x => x.report_id == report_id && x.mediator_id == sign_off_mediator_id && x.status_id != 2).FirstOrDefault();
                            if (inactive_mediator != null)
                                inactive_mediator.status_id = 2;
                        }
                        #endregion

                        #region Save to Sign-off mediator from drop
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
                        #endregion
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    logger.Error("NewCase / CloseCase saving error " + ex.Message);
                }
            }

            if (was_case_promoted)
            {
                if (promotion_value == (int)CaseStatusConstants.CaseStatusValues.Completed)
                {
                    Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                    //need to find selected user in ddl
                    user um_temp = db.user.Find(sign_off_mediator_id);

                    // all mediators except the sign-off mediator  getting email with name
                    eb.CaseCloseApprovePlatformManager(rm._report.display_name, um_temp.first_nm + " " + um_temp.last_nm);
                    // List of all mediators except the sign-off mediator
                    List<user> mediators_to_update = rm.MediatorsWhoHasAccessToReport().Where(t => t.id != sign_off_mediator_id).ToList();
                    foreach (var mediators in mediators_to_update)
                    {
                        emailNotificationModel.SaveEmailBeforeSend(user.id, mediators.id, user.company_id, mediators.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", LocalizationGetter.GetString("Email_Title_NextStep", is_cc), eb.Body, false, 10);
                    }

                    eb.CaseCloseApprove(rm._report.display_name);
                    // send to actual sign-off user
                    emailNotificationModel.SaveEmailBeforeSend(user.id, um_temp.id, user.company_id, um_temp.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", LocalizationGetter.GetString("Email_Title_NextStep", is_cc), eb.Body, false, 9);

                }
                else if (promotion_value == (int)CaseStatusConstants.CaseStatusValues.Investigation && old_status == (int)CaseStatusConstants.CaseStatusValues.Closed)
                {
                    //case was re-open, send re-open email
                }
                else if (promotion_value == (int)CaseStatusConstants.CaseStatusValues.Investigation && old_status == (int)CaseStatusConstants.CaseStatusValues.Completed)
                {
                    //case was returned to investigation, send rereturn to investigation email
                }
                else if (promotion_value == (int)CaseStatusConstants.CaseStatusValues.Closed)
                {
                    #region sendsms
                    TextMessage text = new TextMessage();
                    text.Send();
                    #endregion

                    #region send email

                    Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                    //Case Owner and Platform Manager.
                    eb.CaseCloseApproveClosed(rm._report.display_name);
                    List<user> mediators_to_update = rm.MediatorsWhoHasAccessToReport().ToList();

                    foreach (var mediators in mediators_to_update)
                    {
                        emailNotificationModel.SaveEmailBeforeSend(user.id, mediators.id, user.company_id, mediators.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", LocalizationGetter.GetString("Email_Title_CaseClosureReport", is_cc), eb.Body, false, 73);
                    }


                    #endregion
                }
            }
            switch (promotion_value)
            {

                case 3:
                    if (old_status == 9)
                      logModel.UpdateReportLog(user.id, 29, report_id, "", null, description);
                    logModel.UpdateReportLog(user.id, 21, report_id, LocalizationGetter.GetString("_Started", is_cc), null, description);

                    break;
                case 6:
                    logModel.UpdateReportLog(user.id, 27, report_id, LocalizationGetter.GetString("_Started", is_cc), null, "");
                    break;

                case 9:

                    if (old_status == 6)
                      logModel.UpdateReportLog(user.id, 27, report_id, LocalizationGetter.GetString("_Completed", is_cc), null, description);
                  logModel.UpdateReportLog(user.id, 25, report_id, "", null, description);
                    break;
            }
            return 1;
        }




    }
}