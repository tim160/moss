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
        readonly bool isCC = bool.Parse(ConfigurationManager.AppSettings["isCC"]);

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View($"Index{(isCC ? "-CC" : "")}");
        }

        public ActionResult About()
        {
            return View($"About{(isCC ? "-CC" : "")}");
        }

        [HttpGet]
        public string GetRequest(string fname, string lname, string cname, string phone, string email)
        {
            string result = "Thank you for your request, you’ll be contacted shortly.";
            if (!string.IsNullOrWhiteSpace(fname) && !string.IsNullOrWhiteSpace(lname) && !string.IsNullOrWhiteSpace(cname) && !string.IsNullOrWhiteSpace(phone) && !string.IsNullOrWhiteSpace(email))
            {

                try
                {
                    string[] emails = ConfigurationManager.AppSettings["email"].Split(';');
                    string domain_name = "http://employeeconfidential.com/";
                    if(isCC)
                        domain_name = "http://campusconfidential.com/";
                    for (int i = 0; i < emails.Count(); i++)
                    {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.To.Add(emails[i]);
                        mailMessage.Subject = "New user in " + domain_name;
                        mailMessage.Body = "Hello,\n\n new request in " + domain_name + " \n\n First name is " + fname +
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
