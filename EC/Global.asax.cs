using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using EC.Common.Util;
using log4net;
using Newtonsoft.Json;

namespace EC
{
	public class Global : HttpApplication
	{
		private void Application_Start(object sender, EventArgs e)
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

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			if (!Context.Request.IsSecureConnection
				&& !Context.Request.IsLocal
				&& !Context.Request.Url.AbsoluteUri.ToLower().Contains("ase.com.ua")
				&& !Context.Request.Url.AbsoluteUri.ToLower().Contains("stark.")
				&& !Context.Request.Url.AbsoluteUri.Contains("192.168."))
			{
				string redirect_url = Context.Request.Url
					.ToString()
					.Replace("http:", "https:");
				Response.Redirect(redirect_url, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
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

		public void Application_PreSendRequestHeaders(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
	}
}