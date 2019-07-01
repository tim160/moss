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
        public ActionResult Index(string quickview, string code)
        {
            bool quickView = false;
            if (!string.IsNullOrWhiteSpace(quickview))
              quickView = true;
            ViewBag.quickView = quickView;


      ViewBag.preReg = false;
      if (!string.IsNullOrWhiteSpace(code))
      {
        using (var db = new DBContext())
        { 
          var model = db.CompanyInvitations.FirstOrDefault(x => x.Invitation_code.ToLower().Trim() == code.ToLower().Trim());
          if (model != null && model.Employee_price_type == 3)
          {

            ViewBag.preReg = true;
          }
        }
      }
             
          return View();
        }

        [HttpPost]
        public ActionResult Index(string firstName, string lastName, string companyName, string phone, string email, int numberOfEmployees, string invitationCode, string quickView)
        {
            string emailed_code = "";
            var data = $"{firstName}|{lastName}|{companyName}|{phone}|{email}|{numberOfEmployees}|{invitationCode}|{quickView}";

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.ECCOMVAR(firstName, lastName, companyName, Request.Url.AbsoluteUri.ToLower());
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(email.Trim());
            // em.Send(to, cc, "Employee Confidential Registration", body, true);
            var is_prepaid = false;
            using (var db = new DBContext())
            {
        var check_prepaid = db.CompanyInvitations.FirstOrDefault(x => x.Invitation_code.ToLower().Trim() == invitationCode.ToLower().Trim());
        if (check_prepaid != null && check_prepaid.Employee_price_type == 3)
        {
          is_prepaid = true;
        }

        var model = db.VarInfoes.FirstOrDefault(x => x.Email == email);

        if (is_prepaid)
          emailed_code = Guid.NewGuid().ToString();

         if (model == null)
                {
                    var varinfo = new Data.VarInfoModel
                    {
                        First_nm = firstName,
                        Last_nm = lastName,
                        Company_nm = companyName,
                        Phone = phone,
                        Email = email,
                        Invitation_code = invitationCode,
                        Employee_no = numberOfEmployees,
                        Created_dt = DateTime.Now,

                        /*Non_employee_no = model.NumberOfNonEmployees,
                        Customers_no = model.NumberOfClients,

                        Annual_plan_price = model.PriceNE,
                        Non_employee_price = model.PriceNNE,
                        Customers_price = model.PriceC,
                        Onboarding_price = model.PriceR,
                        Total_price = model.Year * (model.PriceNE + model.PriceNNE + model.PriceC) + model.PriceR,

                        Year = model.Year,
                        Registered_dt = DateTime.Now,
                        Onboarding_session_numbers = model.sessionNumber,*/
                    };

                    if (is_prepaid)
                    {
                      varinfo.Emailed_code_to_customer = emailed_code;
                    }
                    db.VarInfoes.Add(varinfo);
                }
                else
                {
                    model.First_nm = firstName ?? model.First_nm;
                    model.Last_nm = lastName ?? model.Last_nm;
                    model.Company_nm = companyName ?? model.Company_nm;
                    model.Phone = phone ?? model.Phone;
                    model.Email = email ?? model.Email;
                    model.Invitation_code = invitationCode ?? model.Invitation_code;
                    model.Employee_no = numberOfEmployees > 0 ? numberOfEmployees : model.Employee_no;
                    if (is_prepaid)
                    {
                      model.Emailed_code_to_customer = emailed_code;
                      model.Is_registered = false;
                    }
                }
                

 
         

           db.SaveChanges();

      }
          if (quickView == "1")
            return RedirectToAction("Index", "Book", new { id = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data)), quickView = 1 });
          else if (is_prepaid)
          {
            return RedirectToAction("CompanyRegistrationVideo", "Book", new { id = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data)), invitation = invitationCode, invitationcode = invitationCode, emailedcode = emailed_code });
          }
          else
          {
            return RedirectToAction("Index", "Video", new { id = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data)) });
          }
        }

        public ActionResult DirectSale()
        {
          return RedirectToAction("Index", "Var", new { quickview = "ec" });
        }
    }
}