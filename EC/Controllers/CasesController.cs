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

namespace EC.Controllers
{
    public class CasesController : BaseController
    {
        //
        // GET: /Cases/

        private readonly ActiveCasesModel casesModel = new ActiveCasesModel();
        private readonly ReportModel reportModel = ReportModel.inst;
        List<CasePreviewViewModel> preview_list = new List<CasePreviewViewModel>();
        CasePreviewViewModel temp_preview_case;

        public ActionResult Index()
        {
            //   List<report> unread_report = UserModel.inst.UnreadReport(user_id, 0);
            //   ViewBag.pending_report_ids = PendingReports();
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            ViewBag.pending_report_ids = PendingReports();
      ///      ViewBag.pending_report_ids = new List<int>();// PendingReports();

            UserModel um = new UserModel(user.id);
            List<int> all_active_report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            List<int> spam_report_ids = um.ReportsSearchIds(um._user.company_id, 3);
            List<int> closed_report_ids = um.ReportsSearchIds(um._user.company_id, 5);

         /*   List<int> all_active_report_ids = new List<int>();// um.ReportsSearchIds(um._user.company_id, 1);
            List<int> completed_report_ids = new List<int>();// um.ReportsSearchIds(um._user.company_id, 2);
            List<int> spam_report_ids = new List<int>();// um.ReportsSearchIds(um._user.company_id, 3);*/
            int temp_report_id;
            #region Active Reports

            preview_list = new List<CasePreviewViewModel>();
            for (int i = all_active_report_ids.Count - 1; i >= 0; i--)
            {
                temp_report_id = all_active_report_ids[i];
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
            }
            ViewBag.ReportPreviewVM = preview_list;
            ViewBag.active_report_ids = all_active_report_ids.OrderBy(t => t.ToString());

            #endregion

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters =  UnreadActiveReportNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadActiveReportNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadActiveReportNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadActiveReportNumber(closed_report_ids, user.id);

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
            user user = (user)Session[Constants.CurrentUserMarcker];
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

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = UnreadActiveReportNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadActiveReportNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadActiveReportNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadActiveReportNumber(closed_report_ids, user.id);
            
            int temp_report_id;
            preview_list = new List<CasePreviewViewModel>();
            for (int i = completed_report_ids.Count - 1; i >= 0; i--)
            {
                temp_report_id = completed_report_ids[i];
                //       if (temp_report_id == 208 || temp_report_id == 209 || temp_report_id == 210 || temp_report_id == 211 || temp_report_id == 212)
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
            }
            ViewBag.ReportPreviewVM = preview_list;

            return View();
        }

        public ActionResult Closed()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
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

            ViewBag.completed_report_ids = completed_report_ids.OrderBy(t => t.ToString());

            ViewBag.um = um;
            ViewBag.user_id = user.id;
            ViewBag.active_report_counters = UnreadActiveReportNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadActiveReportNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadActiveReportNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadActiveReportNumber(closed_report_ids, user.id);

            int temp_report_id;
            preview_list = new List<CasePreviewViewModel>();
            for (int i = closed_report_ids.Count - 1; i >= 0; i--)
            {
                temp_report_id = closed_report_ids[i];
                //       if (temp_report_id == 208 || temp_report_id == 209 || temp_report_id == 210 || temp_report_id == 211 || temp_report_id == 212)
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
            }
            ViewBag.ReportPreviewVM = preview_list;

            return View();
        }

        public ActionResult Spam()
        {

            user user = (user)Session[Constants.CurrentUserMarcker];
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
            ViewBag.active_report_counters = UnreadActiveReportNumber(all_active_report_ids, user.id);
            ViewBag.completed_report_counters = UnreadActiveReportNumber(completed_report_ids, user.id);
            ViewBag.spam_report_counters = UnreadActiveReportNumber(spam_report_ids, user.id);
            ViewBag.closed_report_counters = UnreadActiveReportNumber(closed_report_ids, user.id);

            ViewBag.spam_report_ids = spam_report_ids;
            preview_list = new List<CasePreviewViewModel>();
            int temp_report_id;
            for (int i = spam_report_ids.Count - 1; i >= 0; i--)
            {
                temp_report_id = spam_report_ids[i];
                //       if (temp_report_id == 208 || temp_report_id == 209 || temp_report_id == 210 || temp_report_id == 211 || temp_report_id == 212)
                temp_preview_case = new CasePreviewViewModel(temp_report_id, user.id);
                preview_list.Add(temp_preview_case);
            }
            ViewBag.ReportPreviewVM = preview_list;

            return View();
        }

        public List<int> PendingReports()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            UserModel um = new UserModel(user.id);

            List<int> all_pending_reports_ids = um.ReportsSearchIds(um._user.company_id, 4);
            all_pending_reports_ids.Sort();
            List<int> pending_report_ids = new List<int>();
            ///////    pending_report_ids.Add(216);
            int temp_report_id;

            //      for (int i = all_pending_reports_ids.Count - 1; i >= 0; i--)
            for (int i = 0; i < all_pending_reports_ids.Count; i++)
            {
                temp_report_id = all_pending_reports_ids[i];
                if (pending_report_ids.Count < 1)
                {
                    ////                  if (temp_report_id == 215 || temp_report_id == 216 || temp_report_id == 217)
                    //                  if (db.report_user_read.Where(item => ((item.user_id == user_id) && (item.report_id == temp_report.id))).Count() == 0)
                    pending_report_ids.Add(temp_report_id);
                }
            }/**/
            return pending_report_ids;
        }


        public int UnreadActiveReportNumber(List<int> _report_ids, int user_id)
        {
            int _count = 0;

            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            foreach (int i in _report_ids)
            {
                rm = new ReportModel(i);
                if (rm._Is_New_Activity(user_id))
                    _count++;

            }
            return _count;
        }

        // do not need it? 
        public int ActiveReportCounters(int user_id)
        {
            int _count = 0;
            
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            List<int> _report_ids = um.ReportsSearchIds(um._user.company_id, 1);
            foreach (int i in _report_ids)
            {
                rm = new ReportModel(i);
                if (rm._Is_New_Activity(user_id))
                    _count++;
            
            }
            return _count;
        }

        public int SpamReportCounters(int user_id)
        {
            int _count = 0;
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            List<int> _report_ids = um.ReportsSearchIds(um._user.company_id, 3);
            foreach (int i in _report_ids)
            {
                rm = new ReportModel(i);
                if (rm._Is_New_Activity(user_id))
                    _count++;

            }

            return _count;
        }
        public int ClosedReportCounters(int user_id)
        {
            int _count = 0;
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            List<int> _report_ids = um.ReportsSearchIds(um._user.company_id, 2);
            foreach (int i in _report_ids)
            {
                rm = new ReportModel(i);
                if (rm._Is_New_Activity(user_id))
                    _count++;

            }

            return _count;
        }
    }
}