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

namespace EC.Controllers
{
    public class CasesController : BaseController
    {
        //
        // GET: /Cases/
        int total_report_count = 2;
        private readonly ActiveCasesModel casesModel = new ActiveCasesModel();
        private ReportModel reportModel = new ReportModel();
        List<CasePreviewViewModel> preview_list = new List<CasePreviewViewModel>();
        CasePreviewViewModel temp_preview_case;

        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ViewBag.pending_report_ids = PendingReports();

            UserModel um = new UserModel(user.id);
            List<int> all_active_report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            List<int> spam_report_ids = um.ReportsSearchIds(um._user.company_id, 3);
            List<int> closed_report_ids = um.ReportsSearchIds(um._user.company_id, 5);

            #region Active Reports
            List<int> temp_all_active_report_ids = all_active_report_ids.OrderBy(t => t).ToList();
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

            #endregion

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = UnreadReportsInProgressNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadReportsInProgressNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadReportsInProgressNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadReportsInProgressNumber(closed_report_ids, user.id);
            ViewBag.newCase = Request.Params.AllKeys.FirstOrDefault(x => x == "stylenewcase");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            return View();
        }

        public ActionResult Completed()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            ViewBag.pending_report_ids = PendingReports();
            //var userId = Session["userId"];
            //UserModel um = new UserModel(2);

            UserModel um = new UserModel(user.id);
            List<int> all_active_report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            List<int> spam_report_ids = um.ReportsSearchIds(um._user.company_id, 3);
            List<int> closed_report_ids = um.ReportsSearchIds(um._user.company_id, 5);

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
            ViewBag.active_report_counters = UnreadReportsInProgressNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadReportsInProgressNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadReportsInProgressNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadReportsInProgressNumber(closed_report_ids, user.id);

            return View();
        }

        public ActionResult Closed()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            ViewBag.pending_report_ids = PendingReports();

            UserModel um = new UserModel(user.id);
            List<int> all_active_report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            List<int> closed_report_ids = um.ReportsSearchIds(um._user.company_id, 5);
            List<int> spam_report_ids = um.ReportsSearchIds(um._user.company_id, 3);

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
            ViewBag.active_report_counters = UnreadReportsInProgressNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadReportsInProgressNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadReportsInProgressNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadReportsInProgressNumber(closed_report_ids, user.id);

            return View();
        }

        public ActionResult Spam()
        {

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            ViewBag.pending_report_ids = PendingReports();

            UserModel um = new UserModel(user.id);
            List<int> all_active_report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            List<int> spam_report_ids = um.ReportsSearchIds(um._user.company_id, 3);
            List<int> closed_report_ids = um.ReportsSearchIds(um._user.company_id, 5);


            ViewBag.user_id = user.id;
            ViewBag.um = um;
            ViewBag.active_report_counters = UnreadReportsInProgressNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadReportsInProgressNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadReportsInProgressNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadReportsInProgressNumber(closed_report_ids, user.id);

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
            ViewBag.active_report_counters = UnreadReportsInProgressNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadReportsInProgressNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadReportsInProgressNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadReportsInProgressNumber(closed_report_ids, user.id);

            return View();
        }

        public List<int> PendingReports()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            UserModel um = new UserModel(user.id);

            List<int> all_pending_reports_ids = um.ReportsSearchIds(um._user.company_id, 4);
            List<int> pending_report_ids = new List<int>();
            if (all_pending_reports_ids.Count > 0)
            {
                all_pending_reports_ids.Sort();
                pending_report_ids.Add(all_pending_reports_ids[0]);
            }

            return pending_report_ids;
        }


        public int UnreadReportsInProgressNumber(List<int> _report_ids, int user_id)
        {
            int _count = 0;

            //    select rep_id from[Marine].[dbo].[testt]  z
            //where id in (select max(id) from[Marine].[dbo].[testt] z2 group by z2.rep_id) and z.status_id = 4
            var refGroupReportLogs = (from m in db.report_log
                                      group m by m.report_id into refGroup
                                      //   orderby refGroup.Id descending
                                      select refGroup.OrderByDescending(x => x.created_dt).FirstOrDefault());

            var refGroupReportReadDate = db.report_user_read.Where(item => ((item.user_id == user_id)));

            DateTime dt1, dt2;
            foreach (int ID in _report_ids)
            {
                if (refGroupReportReadDate.Where(item => ((item.report_id == ID))).Count() == 0)
                {
                    dt2 = ECGlobalConstants._default_date;
                }
                else
                {
                    dt2 = refGroupReportReadDate.Where(item => ((item.report_id == ID))).Select(t => t.read_date).FirstOrDefault();
                }

                if (refGroupReportLogs.Where(item => ((item.report_id == ID))).Count() == 0)
                {
                    dt1 = ECGlobalConstants._default_date.AddDays(2);
                }
                else
                {
                    dt1 = refGroupReportLogs.Where(item => ((item.report_id == ID))).Select(t => t.created_dt).FirstOrDefault();
                }

                if (dt2 < dt1)
                    _count++;
            }
            return _count;
        }

        public ActionResult Preview(int case_id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            ReportModel rm = new ReportModel(case_id);
            UserModel um = new UserModel(user.id);
            CasePreviewViewModel cpvm = new CasePreviewViewModel(rm, um);
            return PartialView("~/Views/Shared/Helpers/_CasePreview.cshtml", cpvm);
        }
    }
}