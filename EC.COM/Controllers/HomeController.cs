using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult CheckStatus()
        {
            return View();
        }

        [HttpGet]
        public string GetRequest(string fname, string lname, string cname, string phone, string email)
        {
            string result = "Thank you for your request, you’ll be contacted shortly.";
            if (fname != "" && lname != "" && cname != "" && phone != "" && email != "")
            {

                try
                {
                    string[] emails = ConfigurationManager.AppSettings["email"].Split(';');

                    for (int i = 0; i < emails.Count(); i++)
                    {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.To.Add(emails[i]);
                        mailMessage.Subject = "New user in http://employeeconfidential.com/";
                        mailMessage.Body = "Hello,\n\n new request in http://employeeconfidential.com/ \n\n First name is " + fname +
                            "\n\n Last Name is " + lname + "\n\n Company name is " + cname + "\n\n Phone is " + phone + "\n\n email is " + email;

                        SmtpClient smtpClient = new SmtpClient();
                        //smtpClient.UseDefaultCredentials = true;

                        smtpClient.Send(mailMessage);
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            else
            {
                result = "Please complete all fields";
            }

            return result;
        }
    }
}
