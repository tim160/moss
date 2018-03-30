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
        url: "Case/Messages/{id}",
        defaults: new
        {
            controller = "Case",
            action = "Messages",
            id = UrlParameter.Optional
        }
    );

            routes.MapRoute(
                name: "CaseTask",
                url: "Case/Task/{id}",
                defaults: new
                {
                    controller = "Case",
                    action = "Task",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "CaseAttachments",
                url: "Case/Attachments/{id}",
                defaults: new
                {
                    controller = "Case",
                    action = "Attachments",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
            name: "CaseReporter",
            url: "Case/Reporter/{id}",
            defaults: new
            {
                controller = "Case",
                action = "Reporter",
                id = UrlParameter.Optional
            }
        );

            routes.MapRoute(
    name: "CaseActivity",
    url: "Case/Activity/{id}",
    defaults: new
    {
        controller = "Case",
        action = "Activity",
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
            name: "CaseTeam",
            url: "Case/Team/{id}",
            defaults: new
            {
                controller = "Case",
                action = "Team",
                id = UrlParameter.Optional
            }
        );
            routes.MapRoute(
                name: "AddToMediators",
                url: "Case/AddToMediators",
                 defaults: new
                 {
                     controller = "Case",
                     action = "AddToMediators",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "MakeCaseOwner",
                url: "Case/MakeCaseOwner",
                 defaults: new
                 {
                     controller = "Case",
                     action = "MakeCaseOwner",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "CloseTask",
                url: "Case/CloseTask",
                 defaults: new
                 {
                     controller = "Case",
                     action = "CloseTask",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "ResolveCase",
                url: "Case/ResolveCase",
                 defaults: new
                 {
                     controller = "Case",
                     action = "ResolveCase",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "ReassignTask",
                url: "Case/ReassignTask",
                 defaults: new
                 {
                     controller = "Case",
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
                name: "CaseClose",
                url: "Case/CloseCase",
                 defaults: new
                 {
                     controller = "Case",
                     action = "CloseCase",
                     id = UrlParameter.Optional
                 });

            routes.MapRoute(
                name: "RemoveMediator",
                url: "Case/RemoveMediator",
                 defaults: new
                 {
                     controller = "Case",
                     action = "RemoveMediator",
                     id = UrlParameter.Optional
                 });


            routes.MapRoute(
                name: "CreateNewTask",
                url: "Case/CreateNewTask",
                 defaults: new
                 {
                     controller = "Case",
                     action = "CreateNewTask",
                     id = UrlParameter.Optional
                 });
            routes.MapRoute(
                name: "CloseCase",
                url: "Case/CloseCase",
                 defaults: new
                 {
                     controller = "Case",
                     action = "CloseCase",
                     report_id = UrlParameter.Optional
                 });


            routes.MapRoute(
            name: "NewStatus",
            url: "Case/NewStatus",
             defaults: new
             {
                 controller = "Case",
                 action = "NewStatus",
                 id = UrlParameter.Optional
             });

            routes.MapRoute(
              name: "Case",
              url: "Case/{id}",
              defaults: new
              {
                  controller = "Case",
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
