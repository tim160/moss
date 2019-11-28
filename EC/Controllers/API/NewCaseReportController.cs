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

            var roles = DB.role_in_report.ToList();

            var m = new
            {
                reportingFrom = rm._reportStringModel.CountryString(),
                reporterWouldLike = rm._anonymousLevel_mediatorVersionByCaller(user.id),
                reporterName = rm.Get_reporter_name(user.id),
                reporterIs = rm._reportStringModel.ReporterCompanyRelationShort(),
                incidentHappenedIn = rm._reportStringModel.LocationString(),
                affectedDepartment = rm._reportStringModel.DepartmentsString(is_cc),
                partiesInvolvedName = "",
                partiesInvolvedTitle = "",
                partiesInvolvedType = "",
           //   partiesInvolvedName = "(Margot) Cooper1",
             // partiesInvolvedTitle = "CFO1",
           //   partiesInvolvedType = "Case Administrators excluded1",

              reportingAbout = rm._reportStringModel.SecondaryTypeString(),
                incidentDate = rm._reportStringModel.IncidentDateStringMonthLong(),
                report_by_myself = rm._report.report_by_myself,
                non_mediator_involved = DB.report_non_mediator_involved
                    .Where(x => x.report_id == id && x.added_by_reporter != false)
                    .OrderBy(x => x.Title)
                    .ToList()
                    .Select(x => new
                    {
                        Name = $"{x.Name} {x.last_name}",
                        Title = x.Title,
                        Role = !String.IsNullOrEmpty(x.Role) ? x.Role : roles.FirstOrDefault(z => z.id == x.role_in_report_id)?.role_en,
                    }),
                management_know_string = rm._reportStringModel.ManagementKnowString(),
                is_reported_outside = rm._reportStringModel.IsReportedOutside(),
                is_reported_urgent = rm._reportStringModel.IsReportedUrgent(),
                secondary_type_string = rm._reportStringModel.SecondaryTypeString(),
                incident_date_string = rm._reportStringModel.IncidentDateStringMonthLong(),
                is_ongoing = rm._reportStringModel.IsOngoing(),
                report_frequency_text = rm._report.report_frequency_text,
                has_injury_damage = rm._reportStringModel.HasInjuryDamage(),
                injury_damage = rm._report.injury_damage,
                description = rm._report.description,
                attachments = DB.attachment.Where(x => x.report_id == id && !x.visible_reporter.HasValue && !x.visible_mediators_only.HasValue).OrderBy(x => x.file_nm),
                reporterPhone = rm._reporter_user.phone != string.Empty && rm._report.incident_anonymity_id == 3 ? rm._reporter_user.phone : null,
                reporterEmail = rm._reporter_user.email != string.Empty && rm._report.incident_anonymity_id == 3 ? rm._reporter_user.email : null,
                incident_anonymity_id = rm._report.incident_anonymity_id,
                agentName = rm._report.agent_id > 0 ? DB.user.Find(rm._report.agent_id).first_nm : ""
            };


            return ResponseObject2Json(m);
        }
    }
}
