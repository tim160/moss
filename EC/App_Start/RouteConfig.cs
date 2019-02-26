using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace EC
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            #region /Case
            routes.MapRoute(
        name: "CaseMessages",
        url: "NewCase/Messages/{id}",
        defaults: new
        {
            controller = "NewCase",
            action = "Messages",
            id = UrlParameter.Optional
        }
    );

            routes.MapRoute(
                name: "CaseTask",
                url: "NewCase/Task/{id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Task",
                    id = UrlParameter.Optional
                }
            );


            routes.MapRoute(
    name: "SaveLoginChanges",
    url: "Report/SaveLoginChanges",
     defaults: new
     {
         controller = "Report",
         action = "SaveLoginChanges",
         id = UrlParameter.Optional
     });
            routes.MapRoute(
                name: "AddToMediators",
                url: "NewCase/AddToMediators",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "AddToMediators",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "MakeCaseOwner",
                url: "NewCase/MakeCaseOwner",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "MakeCaseOwner",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "CloseTask",
                url: "NewCase/CloseTask",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "CloseTask",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "ResolveCase",
                url: "NewCase/ResolveCase",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "ResolveCase",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "ReassignTask",
                url: "NewCase/ReassignTask",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "ReassignTask",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "SendSpamMessage",
                url: "NewReport/SendSpamMessage",
                defaults: new
                {
                    controller = "NewReport",
                    action = "SendSpamMessage",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "AcceptOrReopenCase",
                url: "NewReport/AcceptOrReopenCase",
                defaults: new
                {
                    controller = "NewReport",
                    action = "AcceptOrReopenCase",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseClose",
                url: "NewCase/CloseCase",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "CloseCase",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "NewCloseCaseValidate",
                url: "NewCase/CloseCaseValidate",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "CloseCaseValidate",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "RemoveMediator",
                url: "NewCase/RemoveMediator",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "RemoveMediator",
                     id = UrlParameter.Optional
                 });


            routes.MapRoute(
                name: "CreateNewTask",
                url: "NewCase/CreateNewTask",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "CreateNewTask",
                     id = UrlParameter.Optional
                 });
            routes.MapRoute(
                name: "CloseCase",
                url: "NewCase/CloseCase",
                 defaults: new
                 {
                     controller = "NewCase",
                     action = "CloseCase",
                     report_id = UrlParameter.Optional
                 });


            routes.MapRoute(
            name: "NewStatus",
            url: "NewCase/NewStatus",
             defaults: new
             {
                 controller = "NewCase",
                 action = "NewStatus",
                 id = UrlParameter.Optional
             });

            routes.MapRoute(
              name: "NewCase",
              url: "NewCase/{id}",
              defaults: new
              {
                  controller = "NewCase",
                  action = "Index",
                  id = UrlParameter.Optional
              });


            #endregion

            routes.MapRoute(
                 name: "Start",
                 url: "Index/Start/{id}",
                 defaults: new
                 {
                     controller = "Index",
                     action = "Start",
                     id = UrlParameter.Optional
                 }
             );
            routes.MapRoute(
                 name: "CompanySearch",
                 url: "Index/Start/{lookup}",
                 defaults: new
                 {
                     controller = "Index",
                     action = "CompanyLookup",
                     lookup = UrlParameter.Optional
                 }
             );

            routes.MapRoute(
                name: "NewReport",
                url: "NewReport/{id}",
                defaults: new
                {
                    controller = "NewReport",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
               name: "ReporterDashboardMessages",
               url: "ReporterDashboard/Messages/{id}",
               defaults: new
               {
                   controller = "ReporterDashboard",
                   action = "Messages",
                   id = UrlParameter.Optional
               }
           );
            routes.MapRoute(
                name: "ReporterDashboardActivity",
                url: "ReporterDashboard/Activity/{id}",
                defaults: new
                {
                    controller = "ReporterDashboard",
                    action = "Activity",
                    id = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                 name: "ReporterDashboardAddedMessages",
                 url: "ReporterDashboard/AddedMessages",
                 defaults: new
                 {
                     controller = "ReporterDashboard",
                     action = "AddedMessages"
                 }
             );


            routes.MapRoute(
                name: "UpdateDelays",
                url: "Settings/UpdateDelays",
                 defaults: new
                 {
                     controller = "Settings",
                     action = "UpdateDelays",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "ReporterDashboardSettings",
                url: "ReporterDashboard/Settings/{id}",
                defaults: new
                {
                    controller = "ReporterDashboard",
                    action = "Settings",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                 name: "ReporterDashboardSave",
                 url: "ReporterDashboard/SaveFields",
                 defaults: new
                 {
                     controller = "ReporterDashboard",
                     action = "SaveFields",
                     id = UrlParameter.Optional
                 }
             );

            routes.MapRoute(
                 name: "PaymentHistory",
                 url: "Payment/History",
                 defaults: new
                 {
                     controller = "Payment",
                     action = "History"
                 }
             );
            routes.MapRoute(
                 name: "PaymentIndex",
                 url: "Payment/Index",
                 defaults: new
                 {
                     controller = "Payment",
                     action = "History"
                 }
             );
            routes.MapRoute(
                name: "PaymentReceipt",
                url: "Payment/Receipt/{id}",
                defaults: new
                {
                    controller = "Payment",
                    action = "Receipt",
                    id = UrlParameter.Optional
                }
            );


            routes.MapRoute(
                name: "ReporterDashboard",
                url: "ReporterDashboard/{id}",
                defaults: new
                {
                    controller = "ReporterDashboard",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "CompanyDepartmentReportAdvanced",
                url: "Analytics/CompanyDepartmentReportAdvanced",
                defaults: new
                {
                    controller = "Analytics",
                    action = "CompanyDepartmentReportAdvanced",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseIndex",
                url: "NewCase/Index/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Index",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseActivity",
                url: "NewCase/Activity/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Activity",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseTasks",
                url: "NewCase/Tasks/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Tasks",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseMessages",
                url: "NewCase/Messages/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Messages",
                    report_id = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "NewCaseReporter",
                url: "NewCase/Reporter/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Reporter",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseInvestigationNotes",
                url: "NewCase/InvestigationNotes/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "InvestigationNotes",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "NewCaseCaseClosureReport",
                url: "NewCase/CaseClosureReport/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "CaseClosureReport",
                    report_id = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "NewCaseTeam",
                url: "NewCase/Team/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Team",
                    report_id = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "NewCaseAttachments",
                url: "NewCase/Attachments/{report_id}",
                defaults: new
                {
                    controller = "NewCase",
                    action = "Attachments",
                    report_id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "lang",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                constraints: new { lang = @"ru|en" },
                namespaces: new[] { "EC.Controllers.Controllers" }
            );

            routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { Controller = "Index", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
