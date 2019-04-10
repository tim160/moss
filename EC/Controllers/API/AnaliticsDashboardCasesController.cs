using EC.Constants;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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
            GlobalFunctions f = new GlobalFunctions();

            string[] _titleHeaderLegend = { "Spam", "New Report", "Report Review", "Under Investigation", "Awaiting Sign-Off", "Closed" };
            int[] _titleHeaderLegendIdx = { 6, 0, 1, 2, 3, 8 };
            string[] _miniSquareColor = { "#abb9bb", "#d47472", "#ff9b42", "#3099be", "#64cd9b", "#abb9bb" };
            int[] _today_spanshot = f.AnalyticsByDate(null, null, user.company_id, user.id);
            List<TodaySnapshot> resultsnapShot = new List<TodaySnapshot>();

            DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            int[] _month_end_spanshot = f.AnalyticsByDate(null, _month_end_date, user.company_id, user.id);


            for (int i = 0; i < _titleHeaderLegend.Length; i++)
            {
                var snapShot = new TodaySnapshot
                {
                    numberOfCases = _today_spanshot[_titleHeaderLegendIdx[i]],
                    titleHeaderLegend = _titleHeaderLegend[i],
                    miniSquareColor = _miniSquareColor[i],
                    month_end_spanshot = _month_end_spanshot[i]
                };
                resultsnapShot.Add(snapShot);
            }

            var resultObj = new
            {
                _today_spanshot = resultsnapShot
            };

            return ResponseObject2Json(resultObj);
        }
    }
    class TodaySnapshot
    {
        public int numberOfCases { get; set; }
        public String miniSquareColor { get; set; }
        public String titleHeaderLegend { get; set; }
        public int month_end_spanshot { get; set; }
    }
}
