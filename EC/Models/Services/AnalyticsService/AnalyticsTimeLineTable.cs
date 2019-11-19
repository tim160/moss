using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EC.Models.Services.AnalyticsService
{
    public class AnalyticsTimeLineTable
    {
        public static DataTable dtAnalyticsTimeLineTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("month", typeof(string));
            dt.Columns.Add("pending", typeof(int));
            dt.Columns.Add("review", typeof(int));
            dt.Columns.Add("investigation", typeof(int));
            dt.Columns.Add("resolution", typeof(int));
            dt.Columns.Add("escalation", typeof(int));
            dt.Columns.Add("completed", typeof(int));
            dt.Columns.Add("closed", typeof(int));
            dt.Columns.Add("spam", typeof(int));
            dt.Columns.Add("notused", typeof(int));

            return dt;
        }
    }
}