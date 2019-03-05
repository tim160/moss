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
using EC.Models.ViewModel;
using EC.Constants;
using log4net;
using EC.Models.ViewModels;

namespace EC.Controllers
{
    public class CasesController : BaseController
    {
        //
        // GET: /Cases/
        int total_report_count = 2;
        private ReportModel reportModel = new ReportModel();
        List<CasePreviewViewModel> preview_list = new List<CasePreviewViewModel>();
        CasePreviewViewModel temp_preview_case;

        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            logger.Info("Cases - Cases1");
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");


            logger.Info("Cases - Cases2");

            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();
            logger.Info("Cases - Cases3");

            /////     UsersUnreadReportsNumberViewModel vmUnreadReports = um.GetUserUnreadCasesNumbers(vmAllIDs);
            /*
            List<int> all_active_report_ids = vmAllIDs.all_active_report_ids;
            List<int> completed_report_ids = vmAllIDs.all_completed_report_ids;
            List<int> spam_report_ids = vmAllIDs.all_spam_report_ids;
            List<int> closed_report_ids = vmAllIDs.all_closed_report_ids;*/
            var pending_report_ids = vmAllIDs.all_pending_report_ids;
            ViewBag.pending_report_ids = PendingReports(pending_report_ids);
            #region Active Reports
            /*List <int> temp_all_active_report_ids = all_active_report_ids.OrderBy(t => t).ToList();
            int temp_report_count = total_report_count;
            if (all_active_report_ids.Count() < temp_report_count)
                temp_report_count = all_active_report_ids.Count();

            List<int> removedIDs = new List<int>();
            int temp_report_id;
            for (int i = 0; i < temp_report_count; i++)
            {
                temp_report_id = all_active_report_ids[i];
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
                removedIDs.Add(temp_report_id);
            }
            for (int i = 0; i < removedIDs.Count; i++)
            {
                temp_all_active_report_ids.Remove(removedIDs[i]);
            }

            ViewBag.ReportPreviewStart = preview_list;
            ViewBag.ReportPreviewVM = temp_all_active_report_ids;
            */
            #endregion
            logger.Info("Cases - Cases4");

            //ViewBag.um = um;
            ViewBag.user_id = user.id;
            //ViewBag.active_report_counters = vmUnreadReports.unread_active_reports;
            //ViewBag.completed_report_counters = vmUnreadReports.unread_completed_reports;
            //ViewBag.spam_report_counters = vmUnreadReports.unread_spam_reports;
            //ViewBag.closed_report_counters = vmUnreadReports.unread_closed_reports;

            //ViewBag.newCase = Request.Params.AllKeys.FirstOrDefault(x => x == "stylenewcase");
            logger.Info("Cases - Cases5");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            logger.Info("Cases - Cases6");

            return View();
        }

        public ActionResult Completed()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            
            //var userId = Session["userId"];
            //UserModel um = new UserModel(2);

            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();
            UsersUnreadReportsNumberViewModel vmUnreadReports = um.GetUserUnreadCasesNumbers(vmAllIDs);

            List<int> all_active_report_ids = vmAllIDs.all_active_report_ids;
            List<int> completed_report_ids = vmAllIDs.all_completed_report_ids;
            List<int> spam_report_ids = vmAllIDs.all_spam_report_ids;
            List<int> closed_report_ids = vmAllIDs.all_closed_report_ids;

            var pending_report_ids = vmAllIDs.all_pending_report_ids;
            ViewBag.pending_report_ids = PendingReports(pending_report_ids);

            ViewBag.completed_report_ids = completed_report_ids.OrderBy(t => t.ToString());

            List<int> temp_all_completed_report_ids = completed_report_ids;
            int temp_report_count = total_report_count;
            if (completed_report_ids.Count() < temp_report_count)
                temp_report_count = completed_report_ids.Count();

            List<int> removedIDs = new List<int>();
            int temp_report_id;
            for (int i = 0; i < temp_report_count; i++)
            {
                temp_report_id = completed_report_ids[i];
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
                removedIDs.Add(temp_report_id);
            }
            for (int i = 0; i < removedIDs.Count; i++)
            {
                temp_all_completed_report_ids.Remove(removedIDs[i]);
            }

            ViewBag.ReportPreviewStart = preview_list;
            ViewBag.ReportPreviewVM = temp_all_completed_report_ids;
            
            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = vmUnreadReports.unread_active_reports;
            ViewBag.completed_report_counters = vmUnreadReports.unread_completed_reports;
            ViewBag.spam_report_counters = vmUnreadReports.unread_spam_reports;
            ViewBag.closed_report_counters = vmUnreadReports.unread_closed_reports;

            return View();
        }

        public ActionResult Closed()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
     
            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();
            UsersUnreadReportsNumberViewModel vmUnreadReports = um.GetUserUnreadCasesNumbers(vmAllIDs);

            List<int> all_active_report_ids = vmAllIDs.all_active_report_ids;
            List<int> completed_report_ids = vmAllIDs.all_completed_report_ids;
            List<int> spam_report_ids = vmAllIDs.all_spam_report_ids;
            List<int> closed_report_ids = vmAllIDs.all_closed_report_ids;

            var pending_report_ids = vmAllIDs.all_pending_report_ids;
            ViewBag.pending_report_ids = PendingReports(pending_report_ids);

            ViewBag.closed_report_ids = closed_report_ids.OrderBy(t => t.ToString());

            List<int> temp_all_closed_report_ids = closed_report_ids.OrderBy(t => t).ToList();
            int temp_report_count = total_report_count;
            if (closed_report_ids.Count() < temp_report_count)
                temp_report_count = closed_report_ids.Count();

            List<int> removedIDs = new List<int>();
            int temp_report_id;
            for (int i = 0; i < temp_report_count; i++)
            {
                temp_report_id = closed_report_ids[i];
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
                removedIDs.Add(temp_report_id);
            }
            for (int i = 0; i < removedIDs.Count; i++)
            {
                temp_all_closed_report_ids.Remove(removedIDs[i]);
            }

            ViewBag.ReportPreviewStart = preview_list;
            ViewBag.ReportPreviewVM = temp_all_closed_report_ids;

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = vmUnreadReports.unread_active_reports;
            ViewBag.completed_report_counters = vmUnreadReports.unread_completed_reports;
            ViewBag.spam_report_counters = vmUnreadReports.unread_spam_reports;
            ViewBag.closed_report_counters = vmUnreadReports.unread_closed_reports;

            return View();
        }

        public ActionResult Spam()
        {

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();
            UsersUnreadReportsNumberViewModel vmUnreadReports = um.GetUserUnreadCasesNumbers(vmAllIDs);

            List<int> all_active_report_ids = vmAllIDs.all_active_report_ids;
            List<int> completed_report_ids = vmAllIDs.all_completed_report_ids;
            List<int> spam_report_ids = vmAllIDs.all_spam_report_ids;
            List<int> closed_report_ids = vmAllIDs.all_closed_report_ids;
            var pending_report_ids = vmAllIDs.all_pending_report_ids;
            ViewBag.pending_report_ids = PendingReports(pending_report_ids);

            ViewBag.user_id = user.id;
            ViewBag.um = um;
            ViewBag.active_report_counters = vmUnreadReports.unread_active_reports;
            ViewBag.completed_report_counters = vmUnreadReports.unread_completed_reports;
            ViewBag.spam_report_counters = vmUnreadReports.unread_spam_reports;
            ViewBag.closed_report_counters = vmUnreadReports.unread_closed_reports;

            ViewBag.spam_report_ids = spam_report_ids.OrderBy(t => t.ToString());

            List<int> temp_all_spam_report_ids = spam_report_ids.OrderBy(t => t).ToList();
            int temp_report_count = total_report_count;
            if (spam_report_ids.Count() < temp_report_count)
                temp_report_count = spam_report_ids.Count();

            int temp_report_id;
            List<int> removedIDs = new List<int>();

            for (int i = 0; i < temp_report_count; i++)
            {
                temp_report_id = spam_report_ids[i];
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
                removedIDs.Add(temp_report_id);
            }
            for (int i = 0; i < removedIDs.Count; i++)
            {
                temp_all_spam_report_ids.Remove(removedIDs[i]);
            }


            ViewBag.ReportPreviewStart = preview_list;
            ViewBag.ReportPreviewVM = temp_all_spam_report_ids;

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = vmUnreadReports.unread_active_reports;
            ViewBag.completed_report_counters = vmUnreadReports.unread_completed_reports;
            ViewBag.spam_report_counters = vmUnreadReports.unread_spam_reports;
            ViewBag.closed_report_counters = vmUnreadReports.unread_closed_reports;

            return View();
        }

        public List<int> PendingReports(List<int> all_pending_reports_ids)
        {
            List<int> pending_report_ids = new List<int>();
            if (all_pending_reports_ids.Count > 0)
            {
                all_pending_reports_ids.Sort();
                pending_report_ids.Add(all_pending_reports_ids[0]);
            }

            return pending_report_ids;
        }

        public ActionResult Preview(int case_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            ReportModel rm = new ReportModel(case_id);
            CasePreviewViewModel cpvm = new CasePreviewViewModel(rm, user.id);
            return PartialView("~/Views/Shared/Helpers/_CasePreview.cshtml", cpvm);
        }
    }
}