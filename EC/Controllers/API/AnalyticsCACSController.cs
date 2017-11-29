using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using EC.Models;
using EC.Models.Database;
using EC.Constants;
using EC.Core.Common;
using EC.App_LocalResources;
using EC.Models.ViewModel;
using EC.Common.Interfaces;
using System.Data.Entity.SqlServer;

namespace EC.Controllers.API
{
    public class AnalyticsCACSController : BaseApiController
    {
        public class Filter
        {
            public int Category { get; set; }
        }

        [HttpGet]
        public object Get([FromUri] Filter model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // -- DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // -- DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            var categories = new List<cc_crime_statistics_category>() { new cc_crime_statistics_category { id = 0, crime_statistics_category_en = "All Clery Act Crime Categories" } };
            categories.AddRange(DB.cc_crime_statistics_category
                    .Where(x => x.status_id == 2)
                    .OrderBy(x => x.crime_statistics_category_en)
                    .ToList());

            var data = (from r in DB.report.Where(x => x.company_id == user.company_id)
                     join sjcc in DB.report_cc_crime.Where(x => x.cc_crime_statistics_location_id == model.Category || model.Category == 0) on r.id equals sjcc.report_id into jcc
                     from cc in jcc.DefaultIfEmpty()
                     join sjccl in DB.cc_crime_statistics_location on cc.cc_crime_statistics_location_id equals sjccl.id into jccl
                     from ccl in jccl.DefaultIfEmpty()
                     group r by new { ccl } into g
                     select new {
                         key = g.Key.ccl.crime_statistics_location_en,
                         values = g.GroupBy(x => SqlFunctions.DatePart("YEAR", x.reported_dt)).Select(z => new { x = z.Key, y = z.Count() })
                     })
                     .Where(x => x.key != null)
                     .ToList();

            return new {
                cc_crime_statistics_categories = categories,

                report_cc_crime = data
            };
       }
    }
}