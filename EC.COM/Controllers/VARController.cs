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

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.ECCOMVAR(firstName, lastName, companyName, Request.Url.AbsoluteUri.ToLower());
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(email.Trim());
            // em.Send(to, cc, "Employee Confidential Registration", body, true);

            return RedirectToAction("Index", "Video", new { id = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data)) });
        }
    }
}