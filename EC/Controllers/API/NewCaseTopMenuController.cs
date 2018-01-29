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
    public class NewCaseTopMenuController : BaseApiController
    {
        public class Filter
        {
            public int ReportId { get; set; }
            public bool IsLifeThreating { get; set; }
        }

        [HttpGet]
        [Route("api/NewCaseTopMenu/get")]
        public object Get([FromUri] int reportId)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // -- DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // -- DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            var log = DB.report_log.OrderByDescending(x => x.id).FirstOrDefault(x => x.report_id == reportId && (x.action_id == 16 || (x.action_id == 24)));
            var action = log == null ? null : DB.action.FirstOrDefault(x => x.id == log.action_id);
            var loguser = log == null ? null : DB.user.FirstOrDefault(x => x.id == log.user_id);
            var lifeThreatingInfo = log == null ? "" : $"{action.description_en} at {log.created_dt.ToString()} by {loguser.first_nm} {loguser.last_nm}";

            return new {
                LifeThreating = DB.report.FirstOrDefault(x => x.id == reportId).cc_is_life_threating,
                LifeThreatingInfo = lifeThreatingInfo,
            };
       }

        [HttpPost]
        [Route("api/NewCaseTopMenu/setLifeThreating")]
        public object SetLifeThreating(Filter model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // -- DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // -- DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            GlobalFunctions glb = new GlobalFunctions();

            var report = DB.report.FirstOrDefault(x => x.id == model.ReportId);
            if (report.cc_is_life_threating != true)
            {
                report.cc_is_life_threating = model.IsLifeThreating;
                DB.SaveChanges();

                glb.UpdateReportLog(user.id, model.IsLifeThreating ? 16 : 24, report.id, "", null, "");

                CompanyModel cm = new CompanyModel(report.company_id);
                glb.CampusSecurityAlertEmail(report, Request.RequestUri, DB, cm._company.cc_campus_alert_manager_email);
                glb.CampusSecurityAlertEmail(report, Request.RequestUri, DB, cm._company.cc_daily_crime_log_manager_email);
            }

            return new
            {
                LifeThreating = DB.report.FirstOrDefault(x => x.id == model.ReportId).cc_is_life_threating,
            };
        }
    }
}