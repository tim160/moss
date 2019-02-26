﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using EC.Utils;
using Newtonsoft.Json;

namespace EC
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            log4net.Config.XmlConfigurator.Configure();

            var settings = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
 ///           if (!Context.Request.IsSecureConnection && !Context.Request.IsLocal && !Context.Request.Url.AbsoluteUri.ToLower().Contains("ase.com.ua") && !Context.Request.Url.AbsoluteUri.Contains("192.168."))
 ///               Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            //Server.GetLastError().Message
        }

        public void Application_PreSendRequestHeaders(Object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}