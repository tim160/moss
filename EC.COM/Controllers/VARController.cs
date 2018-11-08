using EC.COM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class VARController : Controller
    {
        // GET: VAR
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string firstName, string lastName, string companyName, string phone, string email, int numberOfEmployees, string invitationCode)
        {
            var data = $"{firstName}|{lastName}|{companyName}|{phone}|{email}|{numberOfEmployees}|{invitationCode}";

            return RedirectToAction("Index", "Video", new { id = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data)) });
        }
    }
}