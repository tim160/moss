using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index(string id)
        {
            // web.config
            // <add key="MainSite" value="http://localhost:8093/" />
            return Redirect($"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}Report/Company/{id}");
        }
    }
}