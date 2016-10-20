using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Mail;

namespace Front.Controllers
{
    public class MainFrontSideController : Controller
    {
        //
        // GET: /MainFrontSide/

        public ActionResult Index()
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
                    string emailManager = ConfigurationManager.AppSettings["emailManager"].ToString();

                    for (int i = 0; i < emails.Count(); i++)
                    {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.To.Add(emails[i]);
                        mailMessage.From = new MailAddress(emailManager);
                        mailMessage.Subject = "New user in http://employeeconfidential.com/";
                        mailMessage.Body = "Hello,\n\n new request in http://employeeconfidential.com/ \n\n First name is " + fname +
                            "\n\n Last Name is " + lname + "\n\n Company name is " + cname + "\n\n Phone is " + phone + "\n\n email is " + email;
                        SmtpClient smtpClient = new SmtpClient("employeeconfidential.com");

                        smtpClient.Credentials = new System.Net.NetworkCredential
                        ("employeeconfidential@employeeconfidential.com", "confidentialConfidential1$3");

                    ///    m_FromAddress = "newrequest@employeeconfidential.com";

                  /*      SmtpClient smtpClient = new SmtpClient("voteplayers.com");

                        // smtpClient.Credentials = new System.Net.NetworkCredential("test@voteplayers.com", "123456");
                        //  smtpClient.Send(mailMessage);
                        m_Server = "voteplayers.com";
                        m_Username = "test@voteplayers.com";
                        m_Password = "123456";
                        m_Port = 25;*/


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
