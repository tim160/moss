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
            };


            return ResponseObject2Json(m);
        }
    }
}