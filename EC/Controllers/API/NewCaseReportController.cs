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
    public class NewCaseReportController : BaseApiController
    {
        public class Filter
        {
            public int ReportFlag { get; set; }
        }

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);
            var rm = new ReportModel(id);

            var m = new
            {
                reportingFrom = rm.CountryString(),
                reporterWouldLike = rm._anonymousLevel_mediatorVersionByCaller(user.id),
                reporterName= rm.Get_reporter_name(user.id),
                reporterIs= rm.ReporterCompanyRelationShort(),
                incidentHappenedIn= rm.LocationString(),
                affectedDepartment= rm.DepartmentsString(),
                partiesInvolvedName= "(Margot) Cooper1",
                partiesInvolvedTitle= "CFO1",
                partiesInvolvedType= "Case Administrators excluded1",
                reportingAbout= rm.SecondaryTypeString(),
                incidentDate= rm.IncidentDateStringMonthLong(),
                report_by_myself = rm._report.report_by_myself,
                non_mediator_involved = DB.report_non_mediator_involved
                    .Where(x => x.report_id == id && x.added_by_reporter != false)
                    .OrderBy(x => x.Title)
                    .ToList()
                    .Select(x => new {
                        Name = $"{x.Name} {x.last_name}",
                        Title = x.Title,
                        Role = x.Role,
                    }),
                management_know_string = rm.ManagementKnowString(),
                is_reported_outside = rm.IsReportedOutside(),
                is_reported_urgent = rm.IsReportedUrgent(),
                secondary_type_string = rm.SecondaryTypeString(),
                incident_date_string = rm.IncidentDateStringMonthLong(),
                is_ongoing = rm.IsOngoing(),
                report_frequency_text = rm._report.report_frequency_text,
                has_injury_damage = rm.HasInjuryDamage(),
                injury_damage = rm._report.injury_damage,
                description = rm._report.description,
                attachments = DB.attachment.Where(x => x.report_id == id && !x.visible_reporter.HasValue && !x.visible_mediators_only.HasValue).OrderBy(x => x.file_nm),
            };


            return ResponseObject2Json(m);
        }
    }
}