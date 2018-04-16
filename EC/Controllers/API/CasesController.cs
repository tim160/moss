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
using EC.Localization;
using EC.Models.ViewModels;

namespace EC.Controllers.API
{
    public class CasesController : BaseApiController
    {
        public class Filter
        {
            public int ReportFlag { get; set; }
        }

        [HttpGet]
        public object Get([FromUri] Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();
            UsersUnreadReportsNumberViewModel vmUnreadReports = um.GetUserUnreadCasesNumbers(vmAllIDs);

            var report_ids = um.ReportsSearchIds(um._user.company_id, filter.ReportFlag);

            List<int> all_active_report_ids = vmAllIDs.all_active_report_ids;
            List<int> completed_report_ids = vmAllIDs.all_completed_report_ids;
            List<int> spam_report_ids = vmAllIDs.all_spam_report_ids;
            List<int> closed_report_ids = vmAllIDs.all_closed_report_ids;
            List<int> all_pending_reports_ids = vmAllIDs.all_pending_report_ids;

            var reports = report_ids.Select(x => new CasePreviewViewModel(x, user.id)).ToList();

            IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();
            var userIds = reports.Select(x => x.last_sender_id).ToList();

            string title = LocalizationGetter.GetString("ActiveCasesUp");
            title = filter.ReportFlag == 2 ? LocalizationGetter.GetString("CompletedcasesUp") : title;
            title = filter.ReportFlag == 5 ? LocalizationGetter.GetString("ClosedCasesUp") : title;
            title = filter.ReportFlag == 3 ? LocalizationGetter.GetString("SpamcasesUp") : title;

            var severities = DB.severity.ToList();

            var m = new
            {
                Mode = filter.ReportFlag,

                Reports = reports,

                ReportsAdv = reports
                    .Select(x =>
                    {
                        var rm = new ReportModel(x.report_id);
                        return new
                        {
                            id = x.report_id,
                            //total_days = rm._total_days,
                            //total_days = Math.Floor((DateTime.Now - rm._report.incident_dt).TotalDays),
                            total_days = Math.Floor((DateTime.Now.Date - rm._report.reported_dt.Date).TotalDays),
                            case_dt_s = rm._report.incident_dt.Ticks,
                            cc_is_life_threating = rm._report.cc_is_life_threating,
                            last_investigation_status_date = m_DateTimeHelper.ConvertDateToLongMonthString(rm._last_investigation_status_date),
                            mediators = rm._mediators_whoHasAccess_toReport.Select(z => new {
                                id = z.id,
                                first_nm = z.first_nm,
                                last_nm = z.last_nm,
                                photo_path = z.photo_path,
                            }),
                            owners = rm._report_owners.Where(z => z.status_id == 2),
                            severity_id = rm._report.severity_id,
                            severity_s = !rm._report.severity_id.HasValue ? "" : severities.FirstOrDefault(z => z.id == rm._report.severity_id).severity_en,
                        };
                    }).ToList(),

                Users = DB.user
                    .AsNoTracking()
                    .Where(x => userIds.Contains(x.id))
                    .ToList(),

                UserIds = userIds,

                Counts = new
                {
                    Active = vmUnreadReports.unread_active_reports,
                    Completed = vmUnreadReports.unread_completed_reports,
                    Spam = vmUnreadReports.unread_spam_reports,
                    Closed = vmUnreadReports.unread_closed_reports,
                    Pending = vmUnreadReports.unread_pending_reports,
                },

                Title = title,
            };

            return ResponseObject2Json(m);
        }
    }
}