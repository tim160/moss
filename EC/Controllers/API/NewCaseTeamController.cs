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
                involved_mediators_user_list = new List<UserAdv>(),
                mediators_whoHasAccess_toReport = new List<UserAdv>(),
                available_toAssign_mediators = new List<UserAdv>(),
                currentInfo = um._user,
            };

            foreach (var item in rm._involved_mediators_user_list.ToList())
            {
                um = new UserModel(item.id);
                m.involved_mediators_user_list.Add(new UserAdv
                {
                    user = item,
                    task_quantity = um.CaseTasksQuantity(id),
                    message_quantity = um.CaseMessagesQuantity(id),
                    action_quantity = um.CaseActionsQuantity(id),
                    location_string = um._location_string,
                    email = um._user.email,
                    owner = rm._report_owners.FirstOrDefault(x => x.user_id == item.id & x.status_id == 2) != null,
                });
            }
            foreach (var item in rm._mediators_whoHasAccess_toReport.ToList())
            {
                um = new UserModel(item.id);
                m.mediators_whoHasAccess_toReport.Add(new UserAdv
                {
                    user = item,
                    task_quantity = um.CaseTasksQuantity(id),
                    message_quantity = um.CaseMessagesQuantity(id),
                    action_quantity = um.CaseActionsQuantity(id),
                    location_string = um._location_string,
                    email = um._user.email,
                    owner = rm._report_owners.FirstOrDefault(x => x.user_id == item.id & x.status_id == 2) != null,
                });
            }
            foreach (var item in rm._available_toAssign_mediators.ToList())
            {
                um = new UserModel(item.id);
                m.available_toAssign_mediators.Add(new UserAdv
                {
                    user = item,
                    task_quantity = um.CaseTasksQuantity(id),
                    message_quantity = um.CaseMessagesQuantity(id),
                    action_quantity = um.CaseActionsQuantity(id),
                    location_string = um._location_string,
                    email = um._user.email,
                    owner = rm._report_owners.FirstOrDefault(x => x.user_id == item.id & x.status_id == 2) != null,
                });
            }

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
                UserModel userModel = UserModel.inst;
                GlobalFunctions glb = new GlobalFunctions();
                IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();

                userModel.AddToMediators(filter.AddToTeam.Value, filter.Report_id.Value);

                UserModel _um = new UserModel(filter.AddToTeam.Value);
                glb.UpdateReportLog(user.id, 5, filter.Report_id.Value, _um._user.first_nm + " " + _um._user.last_nm, null, "");

                if ((_um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_um._user.email.Trim()))
                {
                    List<string> to = new List<string>();
                    List<string> cc = new List<string>();
                    List<string> bcc = new List<string>();

                    to.Add(_um._user.email.Trim());
                    ReportModel _rm = new ReportModel(filter.Report_id.Value);

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.MediatorAssigned(_um._user.first_nm, _um._user.last_nm, user.first_nm, user.last_nm, _rm._report.display_name);
                    string body = eb.Body;
                    em.Send(to, cc, LocalizationGetter.GetString("Email_Title_MediatorAssigned", false), body, true);
                }
            }

            if (filter.RemoveFromTeam.HasValue)
            {
                UserModel _um = new UserModel(filter.RemoveFromTeam.Value);

                int tasks_number = _um.UserTasks(1, filter.Report_id.Value, true).Count();
                if (tasks_number == 0)
                {
                    UserModel userModel = UserModel.inst;
                    GlobalFunctions glb = new GlobalFunctions();

                    userModel.RemoveMediator(filter.RemoveFromTeam.Value, filter.Report_id.Value);
                    glb.UpdateReportLog(user.id, 6, filter.Report_id.Value, _um._user.first_nm + " " + _um._user.last_nm, null, "");
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
            }

            return Get(filter.Report_id.Value);
        }
    }
}