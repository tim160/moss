using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Data;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using System.Data.SqlClient;
using EC.Core.Common;
using EC.Common.Interfaces;
using EC.Constants;
using EC.Common.Util;

namespace EC.Controllers
{
    public class AnalyticsController : BaseController
    {
        protected IDateTimeHelper DateTimeHelper = new DateTimeHelper();

        // GET: Analytics
        public ActionResult Index()
        {

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");
            Session[ECGlobalConstants.CurrentUserMarcker] = user;
            Session["userName"] = user.login_nm;
            Session["userId"] = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            ViewBag.user_id = user.id;
            ViewBag.company_id = user.company_id;
            ViewBag.companyName = db.company.Where(company_name => company_name.id == user.company_id).Select(company_name => company_name.company_nm).FirstOrDefault();
            string _today = DateTimeHelper.ConvertDateToLongMonthString(DateTime.Today);
            ViewBag._today = _today;

            return View("Dashboard");
        }

        public ActionResult Tasks()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            int user_id = user.id;
            ViewBag.user_id = user_id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            int[] _today_spanshot = glb.AnalyticsByDate(null, null, um._user.company_id, um._user.id);
            ViewBag._today_spanshot = _today_spanshot;

            DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            int[] _month_end_spanshot = glb.AnalyticsByDate(null, _month_end_date, um._user.company_id, um._user.id);
            ViewBag._month_end_spanshot = _month_end_spanshot;

            string _today = DateTimeHelper.ConvertDateToLongMonthString(DateTime.Today);
            ViewBag._today = _today;

            DataTable dtAnalyticsTimeline = glb.AnalyticsTimeline(um._user.company_id, um._user.id);
            ViewBag._dtAnalyticsTimeline = StringUtil.ConvertDataTabletoString(dtAnalyticsTimeline);

            DataTable dtTasksColored = glb.TasksPerDay(um._user.company_id, um._user.id);
            ViewBag._dtTasksColored = StringUtil.ConvertDataTabletoString(dtTasksColored);

            DataTable dt_series = new DataTable();
            dt_series.Columns.Add("valueField", typeof(string));
            dt_series.Columns.Add("name", typeof(string));
            dt_series.Columns.Add("stack", typeof(string));

            int _id = 0;
            DataRow _dr1;
            ReportModel rm = new ReportModel();
            string id_string = "";
            foreach (DataColumn _column in dtTasksColored.Columns)
            {
                _dr1 = dt_series.NewRow();
                if (_column.ColumnName.ToLower().Contains("case"))
                {
                    _id = 0;
                    id_string = _column.ColumnName.ToLower().Replace("case", "");
                    try
                    {
                        _id = Convert.ToInt32(id_string);
                    }
                    catch
                    {
                        _id = 0;
                    }
                    if (_id != 0)
                    {
                        rm = new ReportModel(_id);

                        _dr1["valueField"] = "case" + _id;
                        _dr1["name"] = rm._report.display_name;
                        _dr1["stack"] = "female";

                        dt_series.Rows.Add(_dr1);
                    }
                }
            }
            ViewBag.dt_series = StringUtil.ConvertDataTabletoString(dt_series);



            return View();
        }

        public ActionResult Mediators()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            ViewBag.user_id = user_id;

            DataTable dtAnalyticsTimeline = glb.AnalyticsTimeline(um._user.company_id, um._user.id);
            ViewBag._dtAnalyticsTimeline = StringUtil.ConvertDataTabletoString(dtAnalyticsTimeline);

            return View();
        }

        public ActionResult CompanyDepartmentReportAdvanced(ReportTypes types)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];

            if (user != null)
            {
                DateTime dt1 = new DateTime(2014, 8, 1);
                DateTime dt2 = DateTime.Today.AddDays(1);
                if (types.dateStart.Year < 2014)
                {
                    types.dateStart = dt1;
                    types.dateEnd = dt2;
                }

                JsonResult json = new JsonResult();
                json.Data = glb.ReportAdvancedJson(types.companyIdArray, user.id,
                    types.ReportsSecondaryTypesIDStrings,
                    types.ReportsRelationTypesIDStrings,
                    types.ReportsDepartmentIDStringss,
                    types.ReportsLocationIDStrings, types.dateStart, types.dateEnd);
                return json;
            }
            else
            {
                return null;
            }
        }

        public ActionResult CACSReport()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            if (!is_cc)
                return RedirectToAction("Index", "Analytics");

            int user_id = user.id;
            ViewBag.user_id = user_id;

            string _today = DateTimeHelper.ConvertDateToLongMonthString(DateTime.Today);
            ViewBag._today = _today;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            UserModel um = new UserModel(user_id);
            ViewBag.companyName = db.company.Where(company_name => company_name.id == user.company_id).Select(company_name => company_name.company_nm).FirstOrDefault();
            ViewBag.um = um;

            return View();
        }

        public ActionResult RootCauseAnalysis()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            ViewBag.user_id = user.id;

            string _today = DateTimeHelper.ConvertDateToLongMonthString(DateTime.Today);
            ViewBag._today = _today;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            UserModel um = new UserModel(user.id);
            ViewBag.companyName = db.company.Where(company_name => company_name.id == user.company_id).Select(company_name => company_name.company_nm).FirstOrDefault();
            ViewBag.um = um;

            return View();
        }

        public ActionResult Dashboard()
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

            ViewBag.user_id = user.id;
            ViewBag.companyName = db.company.Where(company_name => company_name.id == user.company_id).Select(company_name => company_name.company_nm).FirstOrDefault();
            string _today = DateTimeHelper.ConvertDateToLongMonthString(DateTime.Today);
            ViewBag._today = _today;

            return View();
        }
    }
}