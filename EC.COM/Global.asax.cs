﻿using EC.COM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EC.COM
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Data.Entity.Database.SetInitializer<DBContext>(null);

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!Context.Request.IsSecureConnection && !Context.Request.IsLocal)
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        }

    }
}
