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


namespace EC.Controllers
{
    public class AnalyticsController : BaseController
    {
        protected IDateTimeHelper DateTimeHelper = new DateTimeHelper();

        // GET: Analytics
        public ActionResult Index()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

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

            company company_item = db.company.Where(item => (item.id == um._user.company_id)).FirstOrDefault();
            if (company_item != null)
            {
               
                ViewBag.step1_delay = company_item.step1_delay;
                ViewBag.step2_delay = company_item.step2_delay;
                ViewBag.step3_delay = company_item.step3_delay;
                ViewBag.step4_delay = company_item.step4_delay;
            }

            DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            int[] _month_end_spanshot = glb.AnalyticsByDate(null, _month_end_date, um._user.company_id, um._user.id);
            ViewBag._month_end_spanshot = _month_end_spanshot;

            string _today = DateTimeHelper.ConvertDateToShortString(DateTime.Today);
            ViewBag._today = _today;

     //       DataTable dtSecondaryTypes = glb.SecondaryTypesByDate( um._user.company_id, um._user.id);
    //        DataTable dtCompanyRelationships = glb.RelationshipToCompanyByDate( um._user.company_id, um._user.id);
            
      //      ViewBag._dtSecondaryTypes = dtSecondaryTypes;
       //     ViewBag._dtCompanyRelationships = dtCompanyRelationships;

            //int[] _company_average_days = glb.AverageStageDays(um._user.company_id, um._user.id);
            //int[] _average_stage_days = new int[] { _company_average_days[0] - 2, _company_average_days[1] - 3, _company_average_days[2] - 5, _company_average_days[3] - 5, _company_average_days[4] - 7 };

            //ViewBag._company_average_days = _company_average_days;
            //ViewBag._average_stage_days = _average_stage_days;

       //     DataTable dtCompanyLocationReport = glb.CompanyLocationReport(um._user.company_id, um._user.id);
            //ViewBag._dtCompanyLocationReport = glb.ConvertDataTabletoString(dtCompanyLocationReport);

         //   DataTable dtCompanyDepartmentReport = glb.CompanyDepartmentReport( um._user.company_id, um._user.id);
            //ViewBag._dtCompanyDepartmentReport = glb.ConvertDataTabletoString(dtCompanyDepartmentReport);

            //DataTable dtAnalyticsTimeline = glb.AnalyticsTimeline(um._user.company_id, um._user.id);
            //ViewBag._dtAnalyticsTimeline = glb.ConvertDataTabletoString(dtAnalyticsTimeline);

            GlobalFunctions f = new GlobalFunctions();
            List<Tuple<string, string>> temp_tuple = f.DepartmentsListDistinct(user.company_id, user.id);
            ViewBag.dropDownFirst = temp_tuple;

            temp_tuple = f.LocationsListDistinct(user.company_id, user.id);
            ViewBag.dropDownSecond = temp_tuple;

            temp_tuple = f.SecondaryTypesListDistinct(user.company_id, user.id);
            ViewBag.dropDownThird = temp_tuple;

            temp_tuple = f.RelationTypesListDistinct(user.company_id, user.id);
            ViewBag.dropDownFourth = temp_tuple;



            return View();
        }

        public ActionResult Tasks()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");
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

            string _today = DateTimeHelper.ConvertDateToShortString(DateTime.Today);
            ViewBag._today = _today;

            DataTable dtAnalyticsTimeline = glb.AnalyticsTimeline(um._user.company_id, um._user.id);
            ViewBag._dtAnalyticsTimeline = glb.ConvertDataTabletoString(dtAnalyticsTimeline);

            DataTable dtTasksColored = glb.TasksPerDay(um._user.company_id, um._user.id);
            ViewBag._dtTasksColored = glb.ConvertDataTabletoString(dtTasksColored);

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
            ViewBag.dt_series = glb.ConvertDataTabletoString(dt_series);


            
            return View();
        }

        public ActionResult Mediators()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");
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
            ViewBag._dtAnalyticsTimeline = glb.ConvertDataTabletoString(dtAnalyticsTimeline);

            return View();
        }

        public ActionResult CompanyDepartmentReportAdvanced(int userId, int companyId, ReportTypes types)
        {
            if (userId > 0 && companyId > 0)
            {
                DateTime dt1 = new DateTime(2014, 8,1);
                if (types.dateStart != 0)
                    dt1 = new DateTime(1970, 1, 1).AddTicks(types.dateStart * 10000);

                DateTime dt2  = DateTime.Today.AddDays(1);
                if (types.dateEnd != 0)
                    dt2 = new DateTime(1970, 1, 1).AddTicks(types.dateEnd * 10000);
                if (dt2 < dt1)
                {
                    DateTime dt3 = dt2;
                    dt2 = dt1;
                    dt1 = dt3;
                }

                GlobalFunctions func = new GlobalFunctions();
                JsonResult json = new JsonResult();
                json.Data = func.ReportAdvancedJson(companyId, userId, types.ReportsSecondaryTypesIDStrings, types.ReportsRelationTypesIDStrings, types.ReportsDepartmentIDStringss, types.ReportsLocationIDStrings, dt1, dt2);
                return json;
            }
            else
            {
                return null;
            }
        }
    }
}