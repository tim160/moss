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
            var rm = new ReportModel(filter.Report_id);

            var report = DB.report.FirstOrDefault(x => x.id == filter.Report_id);

            var report_cc_crime = DB.report_cc_crime
                .Where(x => x.report_id == filter.Report_id)
                .FirstOrDefault();

            var report_case_closure_outcome = (from mi in DB.report_non_mediator_involved.Where(x => x.report_id == filter.Report_id)
                                       join jo in DB.report_case_closure_outcome.Where(x => x.report_id == filter.Report_id) on mi.id equals jo.non_mediator_involved_id into j1
                                       from o in j1.DefaultIfEmpty()
                                       join joc in DB.company_outcome.Where(x => x.company_id == report.company_id) on o.outcome_id equals joc.id into j2
                                       from oc in j2.DefaultIfEmpty()
                                       select new
                                       {
                                           mediator = mi,
                                           outcome = o,
                                           outcome_c = oc,
                                       }).ToList()
                                       .Select(x => new {
                                           mediator = x.mediator,
                                           outcome = x.outcome == null ? new Models.Database.report_case_closure_outcome { non_mediator_involved_id = x.mediator.id } : x.outcome,
                                           outcome_c = x.outcome_c,
                                       }).ToList();

            var reporter = DB.report_case_closure_outcome.FirstOrDefault(x => x.report_id == filter.Report_id & x.non_mediator_involved_id == null);
            if (reporter == null)
            {
                reporter = new report_case_closure_outcome
                {
                    report_id = filter.Report_id,
                    role_id = 1
                };
            }

            var outcomes = DB.company_outcome
                .AsNoTracking()
                .Where(x => x.company_id == report.company_id & x.status_id == 2 && x.outcome_en.ToLower() != "none")
                .OrderBy(x => x.outcome_en)
                .ToList();

            var none_outcome = DB.company_outcome.Where(s => s.company_id == report.company_id && s.status_id == 2 && s.outcome_en.ToLower() == "none").SingleOrDefault();
            if (none_outcome != null)
            {
                outcomes.Insert(0, none_outcome);
            }

            var rep_outcome = DB.report_case_closure_outcome.FirstOrDefault(x => x.report_id == filter.Report_id & x.non_mediator_involved_id == null);

            if (rep_outcome == null)
            {
                rep_outcome = new report_case_closure_outcome
                {
                    non_mediator_involved_id = null,
                    report_id = filter.Report_id                    
                };
                DB.report_case_closure_outcome.Add(rep_outcome);
                DB.SaveChanges();
            }

            var m = new
            {
                cc_crime_statistics_categories = DB.cc_crime_statistics_category
                    .Where(x => x.status_id == 2)
                    .OrderBy(x => x.crime_statistics_category_en)
                    .ToList(),

                cc_crime_statistics_locations = DB.cc_crime_statistics_location
                    .Where(x => x.status_id == 2)
                    .OrderBy(x => x.crime_statistics_location_en)
                    .ToList(),

                report_cc_crime = report_cc_crime != null ? report_cc_crime : new report_cc_crime { cc_is_clear_act_crime = true, cc_crime_statistics_category_id = 0, cc_crime_statistics_location_id = 0 },

                report_case_closure_outcome = report_case_closure_outcome,

                reporter = reporter,

                outcomes = outcomes,

                rep_outcome = DB.report_case_closure_outcome
                    .Where(x => x.report_id == filter.Report_id & x.non_mediator_involved_id == null)
                    .Select(x => new {
                        outcome = x,
                        outcome_c = DB.company_outcome.FirstOrDefault(z => z.id == x.outcome_id),
                    }).FirstOrDefault(),
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
                    item = new report_case_closure_outcome
                    {
                        report_id = filter.Report_id,
                        outcome_id = filter.Report_case_closure_outcome.outcome_id,
                        note = filter.Report_case_closure_outcome.note,
                        non_mediator_involved_id = filter.Report_case_closure_outcome.non_mediator_involved_id
                    };
                    DB.report_case_closure_outcome.Add(item);
                }
                else
                {
                    item.outcome_id = filter.Report_case_closure_outcome.outcome_id;
                    item.note = filter.Report_case_closure_outcome.note;
                }
                DB.SaveChanges();

                var mediator = DB.report_non_mediator_involved.FirstOrDefault(x => x.id == item.non_mediator_involved_id);
                var outcome = DB.company_outcome.FirstOrDefault(x => x.id == item.outcome_id);

                GlobalFunctions gf = new GlobalFunctions();
                if ((mediator != null) && (mediator.role_in_report_id == 3)) //49	Recommended Outcome for Subject Added
                {
                    gf.UpdateReportLog(user.id, 49, filter.Report_id, outcome.outcome_en, null, "");
                }
                else //50	Recommended Action for Witness or Reporter Added
                {
                    gf.UpdateReportLog(user.id, 50, filter.Report_id, outcome.outcome_en, null, "");
                }
            }

            return Get(filter);
        }
   }
}