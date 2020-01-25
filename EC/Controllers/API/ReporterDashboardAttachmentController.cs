using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EC.Models.Database;
using EC.Localization;
using EC.Core.Common;
using EC.Models;
using EC.Models.ViewModels;

namespace EC.Controllers.API
{
    public class ReporterDashboardAttachmentController : BaseApiController
    {
        [HttpPost]
        public object Post()
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            string mode = System.Web.HttpContext.Current.Request["mode"];
            string type = System.Web.HttpContext.Current.Request["type"];
            int report_id = 0;
            if (System.Web.HttpContext.Current.Request["report_id"]!=null)
            {
                report_id = Int32.Parse(System.Web.HttpContext.Current.Request["report_id"]);
            }
            

            if (user == null || user.id == 0)
            {
                return null;
            }
            var arrayFiles = new List<ReporterDashboardAttachmentFiles>();
            var dth = new DateTimeHelper();
            for (int i = 0; i < System.Web.HttpContext.Current.Request.Files.Count; i++)
            {
                var file = System.Web.HttpContext.Current.Request.Files[i];
                var report = DB.report.FirstOrDefault(x => x.id == report_id);

                var newFileAttach = new attachment();
                newFileAttach.file_nm = file.FileName;
                newFileAttach.extension_nm = System.IO.Path.GetExtension(file.FileName);
                newFileAttach.path_nm = "";
                newFileAttach.report_id = report_id;
                newFileAttach.status_id = 2;
                newFileAttach.effective_dt = DateTime.Now;
                newFileAttach.expiry_dt = DateTime.Now;
                newFileAttach.last_update_dt = DateTime.Now;
                newFileAttach.user_id = user.id;

                type = mode == "upload_rd" ? "reporter" : "staff";
                newFileAttach.visible_reporter = type == "reporter" ? true : false;
                newFileAttach.visible_mediators_only = type == "staff" ? true : false;

                DB.attachment.Add(newFileAttach);
                DB.SaveChanges();
                var dir = HttpContext.Current.Server.MapPath(String.Format("~/upload/reports/{0}", report.guid));
                var filename = String.Format("{0}_{1}{2}", user.id, DateTime.Now.Ticks, System.IO.Path.GetExtension(file.FileName));
                newFileAttach.path_nm = String.Format("\\upload\\reports\\{0}\\{1}", report.guid, filename);
                DB.SaveChanges();

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                file.SaveAs(string.Format("{0}\\{1}", dir, filename));

                ReportModel rm = new ReportModel(report_id);
                string recipient = String.Empty;
                if (newFileAttach.user_id != rm._report.reporter_user_id)
                {
                    if (newFileAttach.visible_reporter == null || newFileAttach.visible_reporter == true)
                    {
                        recipient = LocalizationGetter.GetString("VisibletoReporter", is_cc) + " & " + LocalizationGetter.GetString("CaseAdministratorsUp", is_cc);
                    }
                    else
                    {
                        recipient = LocalizationGetter.GetString("VisibleTo", is_cc) + " " + LocalizationGetter.GetString("CaseAdministratorsUp", is_cc);
                    }
                }
                else
                {
                    recipient = LocalizationGetter.GetString("UploadedByReporter", is_cc);
                }



                var m = new ReporterDashboardAttachmentFiles()
                {
                    file_nm = newFileAttach.file_nm,
                    path_nm = newFileAttach.path_nm,
                    effective_dt = dth.ConvertDateToLongMonthString(newFileAttach.effective_dt),
                    recipient = recipient
                };
                arrayFiles.Add(m);

            }

            return ResponseObject2Json(arrayFiles);
        }
    }
}