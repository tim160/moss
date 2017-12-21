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

            var data = (from year in new[] { DateTime.Now.Year - 2, DateTime.Now.Year - 1, DateTime.Now.Year }
                        from crime_location in DB.cc_crime_statistics_location
                        group year by new { crime_location } into g

                        select new
                        {
                            key = g.Key.crime_location.crime_statistics_location_en,
                            values = g.GroupBy(x => x).Select(x => new
                            {
                                x = x.Key,
                                y = DB.report.Join(DB.report_cc_crime.Where(rcc => rcc.cc_crime_statistics_category_id == model.Category || model.Category == 0), r => r.id, c => c.report_id, (r, c) => new { report = r, crime = c })
                               .Count(z => 
                                    z.report.reported_dt.Year == x.Key 
                                    & z.crime.cc_crime_statistics_location_id == g.Key.crime_location.id
                                    & z.report.company_id == user.company_id
                                    )
                            })
                        })
                     .Where(x => x.key != null)
                     .ToList();


            return new {
                cc_crime_statistics_categories = categories,

                report_cc_crime = data,

                totals = data.SelectMany(x => x.values).GroupBy(x => x.x).Select(z => new { year = z.Key, count = z.Sum(x => x.y) }).OrderBy(x => x.year)
            };
       }
    }
}