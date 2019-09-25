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
        public Object AnalyticsByDate([FromUri] int[] id)
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return null;
            UserModel um = new UserModel(user.id);

            string[] _titleHeaderLegend = { "Spam", "New Report", "Report Review", "Under Investigation", "Awaiting Sign-Off", "Closed" };
            int[] _titleHeaderLegendIdx = { 6, 0, 1, 2, 3, 8 };
            string[] _miniSquareColor = { "#abb9bb", "#d47472", "#ff9b42", "#3099be", "#64cd9b", "#abb9bb" };

            int[] _today_spanshot = um.AnalyticsCasesArrayByDate(null, id);
            List<TodaySnapshot> resultsnapShot = new List<TodaySnapshot>();
       

            for (int i = 0; i < _titleHeaderLegend.Length; i++)
            {
                var snapShot = new TodaySnapshot
                {
                    numberOfCases = _today_spanshot[_titleHeaderLegendIdx[i]],
                    titleHeaderLegend = _titleHeaderLegend[i],
                    miniSquareColor = _miniSquareColor[i]
                };
                resultsnapShot.Add(snapShot);
            }

            var resultObj = new
            {
                _today_spanshot = resultsnapShot
            };

            return ResponseObject2Json(resultObj);
        }
        [HttpPost]
        public Object GetTurnAroundTime(int[] id)
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
             return null;
            UserModel um = new UserModel(user.id);
            var casesTurnAroundTime = um.AnalyticsCasesTurnAroundTime(id);
       
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
                new {Name = "New Report", value = cm._company.step1_delay },
                new {Name = "Report Review", value = cm._company.step2_delay },
                new {Name = "Under Inves", value = cm._company.step3_delay },  //WARNING Under Inves
                new {Name = "Awaiting Sign-Off", value = cm._company.step4_delay }
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
    }
}
