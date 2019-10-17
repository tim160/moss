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
    public class AnalyticsRootCauseAnalysisController : BaseApiController
    {
        public class Filter
        {
            //public string SecondaryType { get; set; }
            public int SecondaryType { get; set; }
            public int [] companyId { get; set; }
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

            var company_behavioral = DB.company_root_cases_behavioral.Where(x => model.companyId.Contains(x.company_id)).ToList();
            var company_external = DB.company_root_cases_external.Where(x => model.companyId.Contains(x.company_id)).ToList();
            var company_organizational = DB.company_root_cases_organizational.Where(x =>model.companyId.Contains(x.company_id)).ToList();
            var idsB = company_behavioral.Select(x => x.id).ToList();
            var idsE = company_external.Select(x => x.id).ToList();
            var idsO = company_organizational.Select(x => x.id).ToList();

            var reportInfo = DB.report_investigation_methodology
                .Where(x => 
                    (x.report_secondary_type_id == model.SecondaryType || model.SecondaryType == 0) 
                    && (idsB.Contains(x.company_root_cases_behavioral_id.Value) || idsE.Contains(x.company_root_cases_external_id.Value) || idsO.Contains(x.company_root_cases_organizational_id.Value)) )
                .ToList();

            reportInfo = getValidListReports(reportInfo);

            var behavioral = reportInfo
                .Where(x => x.company_root_cases_behavioral_id.HasValue)
                .GroupBy(x => x.company_root_cases_behavioral_id)
                .Select(x => new {
                    id = x.Key,
                    name = company_behavioral.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    count = x.Count()
                })
                .ToList();


            var external = reportInfo
                .Where(x => x.company_root_cases_external_id.HasValue)
                .GroupBy(x => x.company_root_cases_external_id)
                .Select(x => new {
                    id = x.Key,
                    name = company_external.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    count = x.Count()
                })
                .ToList();

            var organizational = reportInfo
                .Where(x => x.company_root_cases_organizational_id.HasValue)
                .GroupBy(x => x.company_root_cases_organizational_id)
                .Select(x => new {
                    id = x.Key,
                    name = company_organizational.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    count = x.Count()
                })
                .ToList();

            var secondaryTypes = DB.company_secondary_type.Where(x => x.company_id == user.company_id).ToList();
            secondaryTypes.Insert(0, new company_secondary_type { id = 0, secondary_type_en = "All Incident Types" });
                                                                                               
            return new {
                SecondaryTypes = secondaryTypes,
                Behavioral = behavioral,
                BehavioralTotal = behavioral.Sum(x => x.count),
                External = external,
                ExternalTotal = external.Sum(x => x.count),
                Organizational = organizational,
                OrganizationalTotal = organizational.Sum(x => x.count),
                Colors = DB.color.OrderBy(x => x.id).Select(x => "#" + x.color_code),
            };
       }
        private List<report_investigation_methodology> getValidListReports(List<report_investigation_methodology> reportInfo)
        {
            var newListReports = new List<report_investigation_methodology>();
            foreach(var item in reportInfo)
            {
                if(checkExistCompany(item))
                    newListReports.Add(item);
            }
            return newListReports;
        }
        private bool checkExistCompany(report_investigation_methodology item)
        {
            if (DB.report.Find(item.report_id).company_id > 0)
            {
                return true;
            }
            return false;
        }
    }
}