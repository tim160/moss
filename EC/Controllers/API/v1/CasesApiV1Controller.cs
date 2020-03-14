using EC.Constants;
using EC.Localization;
using EC.Models;
using EC.Models.ViewModels;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
using System.Threading.Tasks;
using EC.Common.Util.Filters;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/cases")]
    [JwtAuthentication]
    [Authorize]
    public class CasesApiV1Controller : BaseApiController
    {
        [HttpGet]
        [Route]
        [ResponseType(typeof(PagedList<CompanyModel>))]
        public async Task<IHttpActionResult> GetList(int userId, int reportFlag = 0)
        {
            var _started = DateTime.Now;

            UserModel um = new UserModel(userId);
            ReadStatusModel rsm = new ReadStatusModel();
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();

            UsersUnreadReportsNumberViewModel vmUnreadReports = rsm.GetUserUnreadCasesNumbers(vmAllIDs, userId);

            List<int> report_ids = new List<int>();
            switch (reportFlag)
            {
                case 1:
                    //active
                    report_ids = vmAllIDs.all_active_report_ids;
                    break;
                case 2:
                    //completed
                    report_ids = vmAllIDs.all_completed_report_ids;
                    break;
                case 5:
                    //closed
                    report_ids = vmAllIDs.all_closed_report_ids;
                    break;
                case 3:
                    //spam
                    report_ids = vmAllIDs.all_spam_report_ids;

                    break;
                case 4:
                    //pending
                    report_ids = vmAllIDs.all_pending_report_ids;

                    break;
                default:
                    report_ids = vmAllIDs.all_report_ids;
                    break;
            }

            string investigation_status = LocalizationGetter.GetString("Investigation");
            int delay_allowed = 2;
            int investigation_status_id = (int)CaseStatusConstants.CaseStatusValues.Investigation;
            if (report_ids != null && report_ids.Count > 0)
            {
                ReportModel tempRm = new ReportModel(report_ids[0]);
                investigation_status = tempRm._reportStringModel.InvestigationStatusString();
                delay_allowed = tempRm.GetDelayAllowed();
                investigation_status_id = tempRm._report.status_id;
            }

            if (investigation_status.ToLower().Contains(LocalizationGetter.GetString("Investigation").ToLower()))
                investigation_status = LocalizationGetter.GetString("Investigation");

            report_ids = report_ids.OrderByDescending(t => t).ToList();

            var reports = um.ReportPreviews(report_ids, investigation_status, delay_allowed, is_cc).ToList();

            string title = LocalizationGetter.GetString("ActiveCasesUp");
            title = reportFlag == 2 ? LocalizationGetter.GetString("CompletedcasesUp") : title;
            title = reportFlag == 5 ? LocalizationGetter.GetString("ClosedCasesUp") : title;
            title = reportFlag == 3 ? LocalizationGetter.GetString("SpamcasesUp") : title;
            title = reportFlag == 4 ? LocalizationGetter.GetString("NewReportsUp") : title;

            CompanyModel cm = new CompanyModel(um._user.company_id);
            var additionalCompanies = cm.AdditionalCompanies(true).Select(additional_company => new
            {
                id = additional_company.id,
                company_nm = additional_company.company_nm,
            });
            var m = new
            {
                Mode = reportFlag,

                Reports = reports,

                Counts = new
                {
                    Active = vmUnreadReports.unread_active_reports,
                    Completed = vmUnreadReports.unread_completed_reports,
                    Spam = vmUnreadReports.unread_spam_reports,
                    Closed = vmUnreadReports.unread_closed_reports,
                    Pending = vmUnreadReports.unread_pending_reports,
                },

                Title = title,

                Companies = additionalCompanies.Distinct()
            };
            return ApiOk(m);
        }
    }
}