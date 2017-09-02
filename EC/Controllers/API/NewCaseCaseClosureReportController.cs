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

namespace EC.Controllers.API
{
    public class NewCaseCaseClosureReportController : BaseApiController
    {
        public class Filter
        {
            public int Report_id { get; set; }
            public report_cc_crime Report_cc_crime { get; set; }

            public report_case_closure_outcome Report_case_closure_outcome { get; set; }
        }

        [HttpGet]
        public object Get([FromUri] Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : DB.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);

            var report = DB.report.FirstOrDefault(x => x.id == filter.Report_id);

            var report_cc_crime = DB.report_cc_crime
                .Where(x => x.report_id == filter.Report_id)
                .FirstOrDefault();

            var report_non_mediator_involveds = DB.report_non_mediator_involved
                .Where(x => x.report_id == filter.Report_id & (x.role_in_report_id == 3 || x.Role == "Other"))
                .OrderBy(x => x.Name)
                .ToList();

            var report_case_closure_outcomes = DB.report_case_closure_outcome
                .Where(x => x.report_id == filter.Report_id & x.non_mediator_involved_id != null)
                .ToList();

            var reporter = DB.report_case_closure_outcome.FirstOrDefault(x => x.report_id == filter.Report_id & x.non_mediator_involved_id == null);
            if (reporter == null)
            {
                reporter = new report_case_closure_outcome
                {
                    report_id = filter.Report_id,
                    role_id = 1
                };
            }

            report_non_mediator_involveds.ForEach(involved =>
            {
                var item = report_case_closure_outcomes.FirstOrDefault(x => x.non_mediator_involved_id == involved.id);
                if (item == null)
                {
                    report_case_closure_outcomes.Add(new report_case_closure_outcome
                    {
                        non_mediator_involved_id = involved.id,
                        report_id = filter.Report_id,
                        role_id = 3
                    });
                }
            });

            var m = new
            {
                cc_crime_statistics_categories = DB.cc_crime_statistics_category
                    .Where(x => x.status_id ==2)
                    .OrderBy(x => x.crime_statistics_category_en)
                    .ToList(),

                cc_crime_statistics_locations = DB.cc_crime_statistics_location
                    .Where(x => x.status_id == 2)
                    .OrderBy(x => x.crime_statistics_location_en)
                    .ToList(),

                report_cc_crime = report_cc_crime != null ? report_cc_crime : new report_cc_crime { cc_is_clear_act_crime = false, cc_crime_statistics_category_id = 0, cc_crime_statistics_location_id = 0 },

                report_non_mediator_involveds = report_non_mediator_involveds,

                report_case_closure_outcomes = report_case_closure_outcomes,

                reporter = reporter,

                outcomes = DB.outcome.OrderBy(x => x.outcome_en).ToList(),
            };


            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post([FromBody] Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            //DEBUG
            //user = user != null ? user : DB.user.FirstOrDefault(x => x.id == 2);
            //DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            if (filter.Report_cc_crime != null)
            {
                var report_cc_crime = DB.report_cc_crime
                        .Where(x => x.report_id == filter.Report_id)
                        .FirstOrDefault();

                if (report_cc_crime == null)
                {
                    filter.Report_cc_crime.report_id = filter.Report_id;

                    filter.Report_cc_crime.cc_crime_statistics_category_id_user_id = user.id;
                    filter.Report_cc_crime.cc_crime_statistics_category_id_update_dt = DateTime.Now;

                    filter.Report_cc_crime.cc_crime_statistics_location_id_user_id = user.id;
                    filter.Report_cc_crime.cc_crime_statistics_location_id_last_update_dt = DateTime.Now;

                    filter.Report_cc_crime.executive_summary_user_id = user.id;
                    filter.Report_cc_crime.executive_summary_last_update_dt = DateTime.Now;

                    DB.report_cc_crime.Add(filter.Report_cc_crime);
                }
                else
                {
                    report_cc_crime.cc_is_clear_act_crime = filter.Report_cc_crime.cc_is_clear_act_crime;

                    report_cc_crime.executive_summary = filter.Report_cc_crime.executive_summary;
                    report_cc_crime.executive_summary_user_id = user.id;
                    report_cc_crime.executive_summary_last_update_dt = DateTime.Now;

                    report_cc_crime.cc_crime_statistics_category_id = filter.Report_cc_crime.cc_crime_statistics_category_id;
                    report_cc_crime.cc_crime_statistics_category_id_user_id = user.id;
                    report_cc_crime.cc_crime_statistics_category_id_update_dt = DateTime.Now;

                    report_cc_crime.cc_crime_statistics_location_id_user_id = user.id;
                    report_cc_crime.cc_crime_statistics_location_id_last_update_dt = DateTime.Now;
                    report_cc_crime.cc_crime_statistics_location_id = filter.Report_cc_crime.cc_crime_statistics_location_id;
                }
                DB.SaveChanges();
            }

            if (filter.Report_case_closure_outcome != null)
            {
                var item = DB.report_case_closure_outcome.FirstOrDefault(x => x.id == filter.Report_case_closure_outcome.id);
                if (item == null)
                {
                    DB.report_case_closure_outcome.Add(filter.Report_case_closure_outcome);
                }
                else
                {
                    item.outcome_id = filter.Report_case_closure_outcome.outcome_id;
                    item.note = filter.Report_case_closure_outcome.note;
                }
                DB.SaveChanges();
            }

            return Get(filter);
        }
   }
}