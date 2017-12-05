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
                return RedirectToAction("Index", "Account");

            ReportModel rm = new ReportModel(report_id);
            int user_id = user.id;
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
            int user_id = user.id;
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
        public ActionResult CaseClosureReport(int report_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : db.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;
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

            int user_id = user.id;
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
            int user_id = user.id;
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
    }
}