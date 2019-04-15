using EC.Constants;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EC.Models;

namespace EC.Controllers.API
{
    public class AnaliticsDashboardCasesController : BaseApiController
    {
        [HttpGet]
        public Object AnalyticsByDate()
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return null;
            UserModel um = new UserModel(user.id);

            GlobalFunctions f = new GlobalFunctions();

            string[] _titleHeaderLegend = { "Spam", "New Report", "Report Review", "Under Investigation", "Awaiting Sign-Off", "Closed" };
            int[] _titleHeaderLegendIdx = { 6, 0, 1, 2, 3, 8 };
            string[] _miniSquareColor = { "#abb9bb", "#d47472", "#ff9b42", "#3099be", "#64cd9b", "#abb9bb" };

            int[] _today_spanshot = um.AnalyticsCasesArrayByDate(null);
            List<TodaySnapshot> resultsnapShot = new List<TodaySnapshot>();

            DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            int[] _month_end_spanshot = um.AnalyticsCasesArrayByDate(_month_end_date);
       

            for (int i = 0; i < _titleHeaderLegend.Length; i++)
            {
                var snapShot = new TodaySnapshot
                {
                    numberOfCases = _today_spanshot[_titleHeaderLegendIdx[i]],
                    titleHeaderLegend = _titleHeaderLegend[i],
                    miniSquareColor = _miniSquareColor[i],
                    month_end_spanshot = _month_end_spanshot[i],
                    plus_minus_sign = ""
                };
                if (snapShot.numberOfCases > snapShot.month_end_spanshot)
                  snapShot.plus_minus_sign = "+";
                if (snapShot.numberOfCases < snapShot.month_end_spanshot)
                  snapShot.plus_minus_sign = "-";
                resultsnapShot.Add(snapShot);
            }

            var resultObj = new
            {
                _today_spanshot = resultsnapShot
            };

            return ResponseObject2Json(resultObj);
        }
        [HttpPost]
        public Object GetTurnAroundTime()
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
             return null;
            UserModel um = new UserModel(user.id);
            var casesTurnAroundTime = um.AnalyticsCasesTurnAroundTime();
       
            List<TodaySnapshot> resultAroundTime = new List<TodaySnapshot>();
            resultAroundTime.Add(new TodaySnapshot
            {
                numberOfCases = casesTurnAroundTime[0],
                miniSquareColor = "#d47472",
                titleHeaderLegend = "New Report",
            });
            resultAroundTime.Add(new TodaySnapshot
            {
                numberOfCases = casesTurnAroundTime[1],
                miniSquareColor = "#ff9b42",
                titleHeaderLegend = "Report Review",
            });
            resultAroundTime.Add(new TodaySnapshot
            {
                numberOfCases = casesTurnAroundTime[2],
                miniSquareColor = "#3099be",
                titleHeaderLegend = "Under Investigation",
            });
            resultAroundTime.Add(new TodaySnapshot
            {
                numberOfCases = casesTurnAroundTime[3],
                miniSquareColor = "#64cd9b",
                titleHeaderLegend = "Awaiting Sign-Off",
            });

            CompanyModel cm = new CompanyModel(um._user.company_id);
            var CaseManagamentTime = new[]
            {
                new {Name = "Under Inves", value = cm._company.step3_delay },  //WARNING Under Inves
                new {Name = "Report Review", value = cm._company.step2_delay },
                new {Name = "Awaiting Sign-Off", value = cm._company.step4_delay },
                new {Name = "New Report", value = cm._company.step1_delay }
            };

            var resultobj = new
            {
                resultAroundTime = resultAroundTime,
                CaseManagamentTime = CaseManagamentTime
            };
            return ResponseObject2Json(resultobj);
        }
    }
    class TodaySnapshot
    {
        public int numberOfCases { get; set; }
        public String miniSquareColor { get; set; }
        public String titleHeaderLegend { get; set; }
        public int month_end_spanshot { get; set; }
       public string plus_minus_sign { get; set; }
  }
}
