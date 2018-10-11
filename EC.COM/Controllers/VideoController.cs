using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        public ActionResult Index(string id)
        {
            return View();
        }
    }
}