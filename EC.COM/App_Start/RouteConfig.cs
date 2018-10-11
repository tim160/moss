using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EC.COM
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ReportCompany",
                url: "Report/{id}",
                defaults: new { controller = "Report", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Video",
                url: "Video/{id}",
                defaults: new { controller = "Video", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Book",
                url: "Book/{id}",
                defaults: new { controller = "Book", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
