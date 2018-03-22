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
                reportingFrom = rm._country_string,
                reporterWouldLike = rm._anonymousLevel_mediatorVersionByCaller(user.id),
                reporterName= rm.Get_reporter_name(user.id),
                reporterIs= rm._reporter_company_relation_short,
                incidentHappenedIn= rm._location_string,
                affectedDepartment= rm._departments_string,
                partiesInvolvedName= "(Margot) Cooper1",
                partiesInvolvedTitle= "CFO1",
                partiesInvolvedType= "Case Administrators excluded1",
                reportingAbout= rm._secondary_type_string,
                incidentDate= rm._incident_date_string,
                report_by_myself = rm._report.report_by_myself,
                non_mediator_involved = DB.report_non_mediator_involved
                    .Where(x => x.report_id == id)
                    .OrderBy(x => x.Title)
                    .ToList()
                    .Select(x => new {
                        Name = $"{x.Name} {x.last_name}",
                        Title = x.Title,
                        Role = x.Role,
                    }),
                management_know_string = rm._management_know_string,
                is_reported_outside = rm._is_reported_outside,
                is_reported_urgent = rm._is_reported_urgent,
                secondary_type_string = rm._secondary_type_string,
                incident_date_string = rm._incident_date_string,
                is_ongoing = rm._is_ongoing,
                report_frequency_text = rm._report.report_frequency_text,
                has_injury_damage = rm._has_injury_damage,
                injury_damage = rm._report.injury_damage,
                description = rm._report.description,
                attachments = DB.attachment.Where(x => x.report_id == id).OrderBy(x => x.file_nm),
            };


            return ResponseObject2Json(m);
        }
    }
}