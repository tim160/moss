using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult NotFound()
        {
            //404
            return View();
        }

        // GET: Errors
        public ActionResult Index()
        {
            // 500 error
            return View();
        }

        // GET: Errors
        public ActionResult Unavailable()
        {
            // 503 error
            return View();
        }
    }
}