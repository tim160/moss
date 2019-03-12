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
using EC.Models.App.Case;
using EC.Localization;

namespace EC.Controllers.API
{
    public class NewCaseTeamController : BaseApiController
    {
        GlobalFunctions glb = new GlobalFunctions();
        public class Filter
        {
            public int? ReportFlag { get; set; }
            public int? Report_id { get; set; }
            public string NewMessage { get; set; }
            public int? AddToTeam { get; set; }
            public int? RemoveFromTeam { get; set; }
            public int? MakeCaseOwner { get; set; }
        }

        public class UserAdv
        {
            public user user { get; set; }
            public int task_quantity { get; set; }
            public int message_quantity { get; set; }
            public int action_quantity { get; set; }
            public string location_string { get; set; }
            public string email { get; set; }
            public bool owner { get; set; }
            public string user_photo { get; set; }
        }

        [HttpGet]
        public object Get(int id, bool success = true, string message = "")
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            var _started = DateTime.Now;
            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);
            var rm = new ReportModel(id);

            var m = new
            {
                involved_mediators_user_list = new List<UserAdv>(),
                mediators_whoHasAccess_toReport = new List<UserAdv>(),
                available_toAssign_mediators = new List<UserAdv>(),
                currentInfo = um._user,
                message = message,
                success = success
            };
            var _started1 = DateTime.Now;

            foreach (var item in rm.InvolvedMediatorsUserList())
            {
                um = new UserModel(item.id);
                m.involved_mediators_user_list.Add(new UserAdv
                {
                    user = item,
                    task_quantity = 0,
                    message_quantity = 0,
                    action_quantity = 0,
                    location_string = um._location_string,
                    email = item.email,
                    user_photo = glb.Photo_Path_String(item.photo_path, 1, 5),
                    owner = false,
                });
            }
            var counter1 = (DateTime.Now - _started1).TotalMilliseconds;

            var _started2 = DateTime.Now;

            foreach (var item in rm.MediatorsWhoHasAccessToReport())
            {
                um = new UserModel(item.id);
                m.mediators_whoHasAccess_toReport.Add(new UserAdv
                {
                    user = item,
                    task_quantity = um.CaseTasksQuantity(id),
                    message_quantity = um.CaseMessagesQuantity(id),
                    action_quantity = um.CaseActionsQuantityNoCheck(id),
                    location_string = um._location_string,
                    email = item.email,
                    user_photo = glb.Photo_Path_String(item.photo_path, 1, 5),
                    owner = rm.ReportOwners().FirstOrDefault(x => x.user_id == item.id & x.status_id == 2) != null,
                });
            }
            var counter2 = (DateTime.Now - _started2).TotalMilliseconds;
            var _started3 = DateTime.Now;

            foreach (var item in rm.AvailableToAssignMediators())
            {
                um = new UserModel(item.id);
                m.available_toAssign_mediators.Add(new UserAdv
                {
                    user = item,
                    task_quantity = um.CaseTasksQuantity(id),
                    message_quantity =  um.CaseMessagesQuantity(id),
                    action_quantity = um.CaseActionsQuantityNoCheck(id),
                    location_string = um._location_string,
                    email = item.email,
                    user_photo = glb.Photo_Path_String(item.photo_path, 1, 5),
                    owner = false,
                });
            }
            var counter3 = (DateTime.Now - _started3).TotalMilliseconds;
            var counter = (DateTime.Now - _started).TotalMilliseconds;
            var cc = counter1 + counter2 + counter3;
            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post(Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            if (filter.AddToTeam.HasValue)
            {
                GlobalFunctions glb = new GlobalFunctions();
                IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();

                

                UserModel _um = new UserModel(filter.AddToTeam.Value);
                _um.AddToMediators(filter.AddToTeam.Value, filter.Report_id.Value);
                _um = _um._user.company_id == user.company_id ? _um : null;
                glb.UpdateReportLog(user.id, 5, filter.Report_id.Value, _um._user.first_nm + " " + _um._user.last_nm, null, "");

                if ((_um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_um._user.email.Trim()))
                {
                    List<string> to = new List<string>();
                    List<string> cc = new List<string>();
                    List<string> bcc = new List<string>();

                    to.Add(_um._user.email.Trim());
                    ReportModel _rm = new ReportModel(filter.Report_id.Value);
                    _rm = _rm._report.company_id == user.company_id ? _rm : null;

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.MediatorAssigned(_um._user.first_nm, _um._user.last_nm, user.first_nm, user.last_nm, _rm._report.display_name);
                    string body = eb.Body;
                    em.Send(to, cc, LocalizationGetter.GetString("Email_Title_MediatorAssigned", is_cc), body, true);
                }
            }

            if (filter.RemoveFromTeam.HasValue)
            {
                UserModel _um = new UserModel(filter.RemoveFromTeam.Value);

                int tasks_number = _um.UserTasks(1, filter.Report_id.Value, true).Count();
                if (tasks_number == 0)
                {
                    GlobalFunctions glb = new GlobalFunctions();

                    _um.RemoveMediator(filter.RemoveFromTeam.Value, filter.Report_id.Value);
                    glb.UpdateReportLog(user.id, 6, filter.Report_id.Value, _um._user.first_nm + " " + _um._user.last_nm, null, "");
                }
                else
                {
                    return Get(filter.Report_id.Value, false, "User have a tasks");
                }
            }
            if (filter.MakeCaseOwner.HasValue)
            {
                var list = DB.report_owner.Where(x => x.report_id == filter.Report_id).ToList();
                foreach (var item in list)
                {
                    item.status_id = item.user_id == filter.MakeCaseOwner.Value ? 2 : 1;
                }
                if (list.FirstOrDefault(x => x.user_id == filter.MakeCaseOwner.Value) == null)
                {
                    DB.report_owner.Add(new report_owner
                    {
                        created_on = DateTime.Now,
                        report_id = filter.Report_id.Value,
                        user_id = filter.MakeCaseOwner.Value,
                        status_id = 2,
                    });
                }
                DB.SaveChanges();

                UserModel _um = new UserModel(filter.MakeCaseOwner.Value);
                _um = _um._user.company_id == user.company_id ? _um : null;
                ReportModel _rm = new ReportModel(filter.Report_id.Value);
                _rm = _rm._report.company_id == user.company_id ? _rm : null;

                List<string> to = new List<string>();
                to.Add(_um._user.email.Trim());

                List<string> cc = new List<string>();

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                eb.SetCaseOwner(_um._user.first_nm, _um._user.last_nm, user.first_nm, user.last_nm, _rm._report.display_name);
                string body = eb.Body;
                em.Send(to, cc, LocalizationGetter.GetString("Email_Title_SetCaseOwner", is_cc), body, true);
            }

            return Get(filter.Report_id.Value);
        }
    }
}