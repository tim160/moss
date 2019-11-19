using EC.Common.Interfaces;
using EC.Core.Common;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EC.Models.Services.AnalyticsService
{
    public class TimeLineAnalyticsService
    {
        private IDateTimeHelper m_DateTimeHelper;
        public TimeLineAnalyticsService()
        {
            m_DateTimeHelper = new DateTimeHelper();
        }
        public DataTable AnalyticsTimeline(int company_id, int user_id)
        {
            DataTable dt = AnalyticsTimeLineTable.dtAnalyticsTimeLineTable();
            DataRow dr = dt.NewRow();

            #region Initial Row
            dr["month"] = "";
            dr["pending"] = 0;
            dr["review"] = 0;
            dr["investigation"] = 0;
            dr["resolution"] = 0;
            dr["escalation"] = 0;
            dr["closed"] = 0;
            dr["spam"] = 0;
            dt.Rows.Add(dr);

            #endregion

            int _month = DateTime.Today.Month;
            int temp_month = 0;
            int year = DateTime.Today.Year - 1;
            if (_month == 12)
                year = DateTime.Today.Year;

            DateTime _start = DateTime.Today;

            for (int i = 1; i < 13; i++)
            {
                dr = dt.NewRow();
                temp_month = _month + i;
                if (temp_month > 12)
                {
                    temp_month = temp_month - 12;
                    year = DateTime.Today.Year;
                }
                _start = new DateTime(year, temp_month, 1);


                dr = AnalyticsTimeLineRow(_start, company_id, user_id);
                dt.Rows.Add(dr.ItemArray);
            }

            dr = AnalyticsTimeLineRow(DateTime.Today, company_id, user_id);
            dr[0] = " ";
            dt.Rows.Add(dr.ItemArray);

            return dt;
        }
        private DataRow AnalyticsTimeLineRow(DateTime _start, int company_id, int user_id)
        {
            DataRow dr = AnalyticsTimeLineTable.dtAnalyticsTimeLineTable().NewRow();

            #region month_name
            int month = _start.Month;
            string month_s = "";
            Dictionary<int, string> Monthes = m_DateTimeHelper.FullMonth();
            if ((month > 0) && (month < 13))
                Monthes.TryGetValue(month, out month_s);
            #endregion

            dr["month"] = month_s;
            dr["pending"] = 0;
            dr["review"] = 0;
            dr["investigation"] = 0;
            dr["resolution"] = 0;
            dr["escalation"] = 0;
            dr["completed"] = 0;
            dr["spam"] = 0;
            dr["closed"] = 0;
            dr["notused"] = 0;
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            List<report> _all_reports = um.ReportsSearch(company_id, 0);

            int _status = 0;
            foreach (report _report in _all_reports)
            {
                rm = new ReportModel(_report.id);
                _status = rm.report_status_id_by_date(_start);
                if (_status != 0)
                {
                    dr[_status] = (Int32)dr[_status] + 1;
                }
            }

            return dr;
        }

    }
}