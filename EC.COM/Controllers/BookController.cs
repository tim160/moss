﻿using EC.COM.Data;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.COM.Models;
using System.Configuration;
using log4net;


namespace EC.COM.Controllers
{
     
     public class BookController : Controller
     {
        public GlobalFunctions glb = new GlobalFunctions();
        private bool is_cc = false;
        public ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //     public ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Book
        public ActionResult Index(string id = "", string quickView = "")
        {
            var data = System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(id)).Split('|');
            if (data.Length < 6)
            {
                return RedirectToAction("Index", "Var");
            }
            int count;
            if (!int.TryParse(data[5], out count))
            {
                count = 0;
            }
            var model = new CalculateModel
            {
                FirstName = data[0],
                LastName = data[1],
                CompanyName = data[2],
                Phone = data[3],
                Email = data[4],
                NumberOfEmployees = count,
                InvitationCode = data[6],
            };

          bool QuickView = false;
          if (!string.IsNullOrWhiteSpace(quickView))
            QuickView = true;
          ViewBag.quickView = QuickView;
          return View(model);
        }

        public ActionResult Buy(CalculateModel model)
        {
            model = DoCalculate(model);

            using (var db = new DBContext())
            {
                var varinfo = db.VarInfoes.FirstOrDefault(x => x.Email == model.Email);
                /*var varinfo = new Data.VarInfoModel
                {
                    First_nm = model.FirstName,
                    Last_nm = model.LastName,
                    Company_nm = model.CompanyName,
                    Phone = model.Phone,
                    Email = model.Email,
                    Invitation_code = model.InvitationCode,
                    Employee_no = model.NumberOfEmployees,
                    Non_employee_no = model.NumberOfNonEmployees,
                    Customers_no = model.NumberOfClients,

                    Annual_plan_price = model.PriceNE,
                    Non_employee_price = model.PriceNNE,
                    Customers_price = model.PriceC,
                    Onboarding_price = model.PriceR,
                    Total_price = model.Year * (model.PriceNE + model.PriceNNE + model.PriceC) + model.PriceR,

                    Year = model.Year,
                    Registered_dt = DateTime.Now,
                    Onboarding_session_numbers = model.sessionNumber,
                };
                db.VarInfoes.Add(varinfo);*/

                varinfo.Invitation_code = model.InvitationCode;
                varinfo.Employee_no = model.NumberOfEmployees;
                varinfo.Non_employee_no = model.NumberOfNonEmployees;
                varinfo.Customers_no = model.NumberOfClients;

                varinfo.Annual_plan_price = model.PriceNE;
                varinfo.Non_employee_price = model.PriceNNE;
                varinfo.Customers_price = model.PriceC;
                varinfo.Onboarding_price = model.PriceR;
                varinfo.CallCenterHotline = model.PriceCC;

                varinfo.Total_price = model.Year * (model.PriceNE + model.PriceNNE + model.PriceC + model.PriceCC) + model.PriceR;

                varinfo.Year = model.Year;
                varinfo.Registered_dt = DateTime.Now;
                varinfo.Onboarding_session_numbers = model.sessionNumber;
                db.SaveChanges();

                //var data = $"{model.InvitationCode}|{model.FirstName}|{model.LastName}|{model.CompanyName}|{model.Phone}|{model.Email}|{model.NumberOfEmployees}|{model.NumberOfNonEmployees}|{model.NumberOfClients}|{(model.PriceNE + model.PriceNNE + model.PriceC)}";
                //data = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data));
                //return Redirect($"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}new/company?data={data}");
                //return View();
                return RedirectToAction("Order", new { id = varinfo.Id, email = model.Email, company = model.CompanyName, quickView = model.QuickView });
            }
        }

        public CalculateModel DoCalculate(CalculateModel model)
        {
            model.PriceNE = 0m;
            model.PriceNNE = 0m;
            model.PriceC = 0m;
            model.PriceR = 0m;
            model.PriceCC = 0m;

            model.sessionNumber = 0;

            using (var db = new DBContext())
            {
                string invitation_code = String.IsNullOrEmpty(model.InvitationCode) ? "EC" : model.InvitationCode;
                int[] invitation_code_checks = new int[] { 1, 50, 100, 200, 500, 600, 700, 800, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 10000, 12000, 15000, 20000, 25000, 50000 };

                var items = db.CompanyInvitations
                    .Where(x => x.Invitation_code.ToLower() == invitation_code.ToLower())
                    .ToList();
                bool is_invitation_complete = true;
                foreach (var check in invitation_code_checks)
                {
                  var is_inside = items.FirstOrDefault(x => check >= x.From_quantity && check <= x.To_quantity);
                  if (is_inside == null)
                  {
                    is_invitation_complete = false;
                  //  logger.Info("is_invitation_complete - false, quant - " + check.ToString());
                  }
                }
                if(!is_invitation_complete)
                  items = db.CompanyInvitations.Where(x => x.Invitation_code.ToLower() == "ec").ToList();
                var ne = items.FirstOrDefault(x => model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity);
                //logger.Info("NE  " + ne.Id);

                if (model.callCenter)
                {
                  var callCenterItem = db.CompanyInvitations.Where(x => x.Invitation_code.ToLower() == "empathia1" && model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity).FirstOrDefault();
                  if (callCenterItem != null && callCenterItem.Employee_price.HasValue)
                  {
                    if (model.Year == 1)
                    {
                      model.PriceCC = callCenterItem.Employee_price_type.Value == 1 ? callCenterItem.Employee_price.Value : callCenterItem.Employee_price.Value * model.NumberOfEmployees;
                    }
                    else
                    {
                      model.PriceCC = callCenterItem.Employee_price_type.Value == 1 ? callCenterItem.TwoYearPerYear_employee_price.Value : callCenterItem.TwoYearPerYear_employee_price.Value * model.NumberOfEmployees;
                    }
                  }
                }

                if ((ne != null) && (ne.Employee_price.HasValue) && (ne.Employee_price_type.HasValue))
                {
                 // logger.Info("NE  " + ne.Employee_price);
                  model.PriceNE = ne.Employee_price_type.Value == 1 ? ne.Employee_price.Value : ne.Employee_price.Value * model.NumberOfEmployees;
                    if (model.Year == 2)
                    {
                        model.PriceNE = ne.Employee_price_type.Value == 1 ? ne.TwoYearPerYear_employee_price.Value : ne.TwoYearPerYear_employee_price.Value * model.NumberOfEmployees;
                    }
                    model.PriceR = ne.Onboarding_fee.Value;
                    if (ne.Onboarding_session_numbers.HasValue)
                    {
                        model.sessionNumber = ne.Onboarding_session_numbers.Value;
                    }
                }
                var nne = items.FirstOrDefault(x => model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity);
                if ((nne != null) && (nne.Contractor_price.HasValue) && (nne.Contractor_price_type.HasValue))
                {
                    model.PriceNNE = nne.Contractor_price_type.Value == 1 ? nne.Contractor_price.Value : nne.Contractor_price.Value * model.NumberOfNonEmployees;
                }
                var c = items.FirstOrDefault(x => model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity);
                if ((c != null) && (c.Employee_price.HasValue) && (c.Employee_price_type.HasValue))
                {
                    model.PriceC = c.Customer_price_type.Value == 1 ? c.Customer_price.Value : c.Customer_price.Value * model.NumberOfClients;
                }
            }

            //model.PriceNE = model.PriceNE * (model.Year == 1 ? 1.2m : 2m);
            model.PriceNNE = model.PriceNNE * (model.Year == 1 ? 1.2m : 1m);
            model.PriceC = model.PriceC * (model.Year == 1 ? 1.2m : 1m);
            model.AnnualyTotal = model.PriceNE + model.PriceNNE + model.PriceC + model.PriceCC;
            model.GrandTotal = model.AnnualyTotal * model.Year + model.PriceR;
            model.sessionN = "";
            switch (model.sessionNumber)
            {

                case 1:
                    model.sessionN = "Up to one session";
                    break;
                case 2:
                    model.sessionN = "Up to two sessions";
                    break;
                case 3:
                    model.sessionN = "Up to three sessions";
                    break;
                default:
                    {
                        model.sessionN = "";
                        break;
                    }
            }



            return model;
        }

        [HttpPost]
        public JsonResult Calculate(CalculateModel model)
        {
            model = DoCalculate(model);

            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";

            var data = new
            {
                priceNE = CutTheZeroos(model.PriceNE),
                priceNNE = CutTheZeroos(model.PriceNNE),
                priceC = CutTheZeroos(model.PriceC),
                priceCC = CutTheZeroos(model.PriceCC),
                priceT = CutTheZeroos((model.PriceNE + model.PriceNNE + model.PriceC + model.PriceCC)),
                priceR = CutTheZeroos(model.PriceR),
                sessionN = model.sessionN,
                
            };

            return new JsonResult
            {
                Data = data,
            };
        }


        string CutTheZeroos(decimal value)
    {
      NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
      nfi.NumberGroupSeparator = ",";

      decimal mWhole = Math.Truncate(value);
      // get the fractional value
      decimal mFraction = value - mWhole;


      int t =Convert.ToInt32(mFraction * 100);
      if(t == 0)
        return value.ToString("N0", nfi);
      return value.ToString("N2", nfi);
    }
        public ActionResult Order(int id, string email, string company, string quickView)
        {
            var db = new DBContext();
            var model = db.VarInfoes.FirstOrDefault(x => x.Id == id && x.Email == email && x.Company_nm == company);

            bool QuickView = false;
            if (!string.IsNullOrWhiteSpace(quickView))
              QuickView = true;
            ViewBag.quickView = QuickView;

            return View(new OrderViewModel
            {
                VarInfo = model,
                ExpirationYear = DateTime.Now.Year,
                ExpirationMonth = DateTime.Now.Month,
            });
        }

        [HttpPost]
        public ActionResult Order(OrderViewModel model)
        {
            var db = new DBContext();
            var varinfo = db.VarInfoes.FirstOrDefault(x => x.Id == model.VarInfo.Id && x.Email == model.VarInfo.Email);
            varinfo.Emailed_code_to_customer = varinfo.Emailed_code_to_customer ?? Guid.NewGuid().ToString();
            varinfo.Registered_dt = DateTime.Now;
            db.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.OrderConfirmation_Email(
                varinfo.Emailed_code_to_customer,
                String.IsNullOrEmpty(varinfo.First_nm) && String.IsNullOrEmpty(varinfo.Last_nm) ? "Customer" : varinfo.First_nm,
                varinfo.Last_nm,
                varinfo.Annual_plan_price.ToString(),
                varinfo.Onboarding_price.ToString(),
                (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd'st', yyyy", new CultureInfo("en-US")),
                varinfo.Last_nm,
                varinfo.Company_nm,
                model.NameOnCard,
                varinfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode{varinfo.Emailed_code_to_customer}&invitationcode=VAR&quickview={model.QuickView}",
                $"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}new/registration/{varinfo.Emailed_code_to_customer}"
                );
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(varinfo.Email.Trim());
            em.Send(to, cc, "Employee Confidential Registration", body, true);
            //var resultErrorMessage = await em.QuickSendEmailAsync(varinfo.Email.Trim(), "copy", "Employee Confidential Registration", body, true);
            //if (resultErrorMessage.exception != null)
            //{
            //    logger.Info("ReportController / New" + resultErrorMessage.exception.Message);
            //}

            return RedirectToAction("CompanyRegistrationVideo", "Book", new { emailedcode = varinfo.Emailed_code_to_customer, quickview = model.QuickView, invitationcode = "VAR" });
        }

        public ActionResult CompanyRegistrationVideo(string emailedcode,string quickview, string invitationcode)
        {
            ViewBag.EmailedCode = emailedcode;

            bool quickView = false;
            if (!string.IsNullOrWhiteSpace(quickview) && (quickview =="1" || quickview == "true"))
            {
              quickView = true;
            }
            ViewBag.quickView = quickView;


            ViewBag.preReg = false;
            if (!string.IsNullOrWhiteSpace(invitationcode))
            {
              using (var db = new DBContext())
              {
                var model = db.CompanyInvitations.FirstOrDefault(x => x.Invitation_code.ToLower().Trim() == invitationcode.ToLower().Trim());
                if (model != null && model.Employee_price_type == 3)
                {
                  ViewBag.preReg = true;
                }
              }
            }

          return View();
        }

        [HttpPost]
        public ActionResult Payment(VarInfoModel varInfo, string stripeToken, string QuickView)
        {
            string quickView = "";
            if (!string.IsNullOrWhiteSpace(QuickView) && QuickView =="1")
              quickView = "1";
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["ApiPayKey"]);

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = stripeToken; // Using ASP.NET MVC

            var db = new DBContext();
            var varinfo = db.VarInfoes.FirstOrDefault(x => x.Id == varInfo.Id && x.Email == varInfo.Email);

            var options = new ChargeCreateOptions
            {
                Amount = System.Convert.ToInt64(varinfo.Total_price),
                Currency = "usd",
                Description = "Employee Confidential Subscription Services",
                SourceId = token,
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            varinfo.Emailed_code_to_customer = charge.Id;
            varinfo.Registered_dt = varinfo.Registered_dt ?? DateTime.Now;
            db.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.OrderConfirmation_Email(
                varinfo.Emailed_code_to_customer,
                String.IsNullOrEmpty(varinfo.First_nm) && String.IsNullOrEmpty(varinfo.Last_nm) ? "Customer" : varinfo.First_nm,
                varinfo.Last_nm,
                varinfo.Annual_plan_price.ToString(),
                varinfo.Onboarding_price.ToString(),
                (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd'st', yyyy", new CultureInfo("en-US")),
                varinfo.Last_nm,
                varinfo.Company_nm,
                "",
                varinfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Video/Index",
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode{varinfo.Emailed_code_to_customer}&invitationcode=VAR&quickview={quickView}");
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(varinfo.Email.Trim());
            em.Send(to, cc, "Employee Confidential Registration", body, true);

            return RedirectToAction("CompanyRegistrationVideo", "Book", new { emailedcode = varinfo.Emailed_code_to_customer, invitationcode = "VAR", quickView = quickView });
        }
        [HttpGet]
        public ActionResult Onboarding(Guid guid)
        {
                using (var db = new DBContext())
            {
                var company = db.company.Where(m => m.guid == guid).FirstOrDefault();
                if (company == null)
                {
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ViewBag.companyId = company.id;
                    ViewBag.companyName = company.company_nm;
                    ViewBag.GuidCompany = guid;
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Onboarding(OnboardingForm form)
        {
            return View("~/Views/Book/OnboardingPayment.cshtml", form);
        }

        [HttpPost]
        public ActionResult OnboardingPayment(OnboardingPaymentForm form, string stripeToken)
        {

            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["ApiPayKey"]);

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = stripeToken; // Using ASP.NET MVC
            var options = new ChargeCreateOptions
            {
                ////    Amount = System.Convert.ToInt64(varinfo.Total_price),
                Currency = "usd",
                Description = "Employee Confidential Onboarding Process",
                SourceId = token,
                Amount = form.Amount
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);
 
            var db = new DBContext();
            string id = charge.Id;
            var company = db.company.Where(t => t.guid == form.CompanyGuid).FirstOrDefault();
            if(company != null)
            {
              int numberOfSessions = form.SessionNumber;
              company.onboard_sessions_paid = company.onboard_sessions_paid + numberOfSessions;
              company.onboard_sessions_expiry_dt = DateTime.Today.AddYears(1);



        var training_payment = new company_payment_training
        {
          company_id = company.id,
          payment_date = DateTime.Today,
          onboard_sessions_expiry_dt = DateTime.Today.AddYears(1),
          auth_code = token,
          amount = form.Amount,
          onboard_sessions_paid = form.SessionNumber,
          user_id = 1,
          payment_code = "ECT-" + company.id.ToString() + "-" + DateTime.Now.ToString("yyMMddHHmmss"),
          training_code = "ECT-" + company.id.ToString() + "-" + DateTime.Now.ToString("yyMMddHHmmss"),
          local_invoice_number = "ECT-" + company.id.ToString() + "-" + DateTime.Now.ToString("yyMMddHHmmss")
        };
        db.company_payment_trainings.Add(training_payment);

              db.SaveChanges();
            }
            #region Purchase Confirmation email

            glb.BookingECOnboardingSessionNotifications(company, id, form.Amount, form.SessionNumber, is_cc, this.Request);
            #endregion

            return Redirect("https://report.employeeconfidential.com/trainer/calendar/");
  
        }
    }
}