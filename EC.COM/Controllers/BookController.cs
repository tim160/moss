using EC.COM.Data;
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
using Newtonsoft.Json;
using EC.Constants;

namespace EC.COM.Controllers
{

    public class BookController : Controller
    {
        public GlobalFunctions glb = new GlobalFunctions();
        private bool is_cc = false;
        private string INVITATIONCODE = "VAR";
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
                string invitation_code = String.IsNullOrEmpty(model.InvitationCode) ? VarConstants.DefaultVARCode : model.InvitationCode;
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
                if (!is_invitation_complete)
                    items = db.CompanyInvitations.Where(x => x.Invitation_code.ToLower() == VarConstants.DefaultVARCode.ToLower()).ToList();
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


            int t = Convert.ToInt32(mFraction * 100);
            if (t == 0)
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

            VarInfoModelString varInfoString = new VarInfoModelString();

            varInfoString.Annual_plan_priceS = CutTheZeroos(model.Annual_plan_price);
            varInfoString.CallCenterHotlineS = CutTheZeroos(model.CallCenterHotline);
            varInfoString.Customers_priceS = CutTheZeroos(model.Customers_price);
            varInfoString.Non_employee_priceS = CutTheZeroos(model.Non_employee_price);
            varInfoString.Onboarding_priceS = CutTheZeroos((model.Onboarding_price));
            varInfoString.Total_priceS = CutTheZeroos(model.Total_price);

            varInfoString.TotalAnnualPriceS = CutTheZeroos(model.Annual_plan_price + model.Customers_price + model.Non_employee_price + model.CallCenterHotline);
            varInfoString.TwoYearPriceS = CutTheZeroos((model.Annual_plan_price + model.Customers_price + model.Non_employee_price + model.CallCenterHotline) * model.Year);


            return View(new OrderViewModel
            {
                VarInfo = model,
                VarInfoString = varInfoString,

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

            var company_payment = db.company_paymentss.Where(t => t.auth_code == varinfo.Emailed_code_to_customer).FirstOrDefault();
            string local_inv_num = varinfo.Emailed_code_to_customer;
            if (company_payment != null)
                local_inv_num = company_payment.local_invoice_number;

            varinfo.Registered_dt = DateTime.Now;
            db.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.OrderConfirmation_Email(
                local_inv_num,
                String.IsNullOrEmpty(varinfo.First_nm) && String.IsNullOrEmpty(varinfo.Last_nm) ? "Customer" : varinfo.First_nm,
                varinfo.Last_nm,
                varinfo.Annual_plan_price.ToString(),
                varinfo.Onboarding_price.ToString(),
                (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
                varinfo.Last_nm,
                varinfo.Company_nm,
                model.NameOnCard,
                varinfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode={varinfo.Emailed_code_to_customer}&invitationcode=VAR&quickview={model.QuickView}",
                $"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}new/registration/{varinfo.Emailed_code_to_customer}",
                varinfo.Year.ToString(),
                varinfo.Total_price.ToString()

                );
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(varinfo.Email.Trim());
      ///      em.Send(to, cc, "Employee Confidential Registration", body, true);

            glb.SaveEmailBeforeSend(2, 2, 2, varinfo.Email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential", eb.Body, false, 67);

      #region   Purchase was made, need to send email to Partner / EC
      string invitationCode = VarConstants.DefaultVARCode;
      if (!string.IsNullOrWhiteSpace(varinfo.Invitation_code))
        invitationCode = varinfo.Invitation_code;

      var company_invitation = db.CompanyInvitations.Where(t => t.Invitation_code == invitationCode).FirstOrDefault();
      if (company_invitation != null)
      {
        /// need to add partner_id and replace company_id with it. 
        var partner = db.partners.Where(t => t.id == company_invitation.Created_by_company_id).FirstOrDefault();

        if (partner != null)
        {
          EC.Business.Actions.Email.EmailBody eb_partner = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
          eb_partner.GetConfirmationTextPartner(

                 String.IsNullOrEmpty(varinfo.First_nm) && String.IsNullOrEmpty(varinfo.Last_nm) ? "Customer" : varinfo.First_nm,
                 varinfo.Last_nm,
                 varinfo.Annual_plan_price.ToString(),
                 varinfo.Onboarding_price.ToString(),
                 (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
                 local_inv_num,
                 varinfo.Company_nm,
                  invitationCode,
                  varinfo.Year.ToString(),
                  varinfo.Total_price.ToString()
                 );

 
 
          glb.SaveEmailBeforeSend(2, 2, 2, partner.email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential: #" + local_inv_num, eb.Body, false, 77);

        }

      }
      var partnerEC = db.partners.Where(t => t.partner_code == "EC").FirstOrDefault();
      if (partnerEC != null)
      {
        EC.Business.Actions.Email.EmailBody eb_EC = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
        eb_EC.GetConfirmationTextEC(

               String.IsNullOrEmpty(varinfo.First_nm) && String.IsNullOrEmpty(varinfo.Last_nm) ? "Customer" : varinfo.First_nm,
               varinfo.Last_nm,
               varinfo.Annual_plan_price.ToString(),
               varinfo.Onboarding_price.ToString(),
               (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
               local_inv_num,
               varinfo.Company_nm,
                invitationCode,
               varinfo.Year.ToString(),
               varinfo.Total_price.ToString()
               );

 
 
        glb.SaveEmailBeforeSend(2, 2, 2, partnerEC.email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential: #" +local_inv_num, eb_EC.Body, false, 78);

      }

      #endregion

      //var resultErrorMessage = await em.QuickSendEmailAsync(varinfo.Email.Trim(), "copy", "Employee Confidential Registration", body, true);
      //if (resultErrorMessage.exception != null)
      //{
      //    logger.Info("ReportController / New" + resultErrorMessage.exception.Message);
      //}

      return RedirectToAction("CompanyRegistrationVideo", "Book", new { emailedcode = varinfo.Emailed_code_to_customer, quickview = model.QuickView, invitationcode = "VAR" });
        }

        public ActionResult CompanyRegistrationVideo(string emailedcode, string quickview, string invitationcode)
        {
            ViewBag.EmailedCode = emailedcode;

            bool quickView = false;
            if (!string.IsNullOrWhiteSpace(quickview) && (quickview == "1" || quickview == "true"))
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
        public ActionResult Payment(OrderViewModel orderViewModel)
        {
            string quickView = "";
            if (!string.IsNullOrWhiteSpace(orderViewModel.QuickView) && orderViewModel.QuickView == "1")
                quickView = "1";
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["ApiPayKey"]);

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = orderViewModel.StripeToken; // Using ASP.NET MVC

            var db = new DBContext();
            orderViewModel.VarInfo = db.VarInfoes.FirstOrDefault(x => x.Id == orderViewModel.VarInfo.Id && x.Email == orderViewModel.VarInfo.Email);

            var options = new ChargeCreateOptions
            {
                Amount = System.Convert.ToInt64(orderViewModel.VarInfo.Total_price * 100),
                Currency = "usd",
                Description = "Employee Confidential Subscription Services",
                SourceId = token,
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            orderViewModel.VarInfo.Emailed_code_to_customer = charge.Id;
            orderViewModel.VarInfo.Registered_dt = orderViewModel.VarInfo.Registered_dt ?? DateTime.Now;

            var company_payment = new company_payments
            {
                id = Guid.NewGuid(),
                company_id = 1,
                payment_date = DateTime.Today,

                auth_code = token,     /// Emailed_code_to_customer
                amount = orderViewModel.VarInfo.Total_price,
                local_invoice_number = "EC-" + DateTime.Now.ToString("yyMMddHHmmssfff"),
                stripe_receipt_url = "",
                cc_name = "",
                cc_number = "",
                cc_month = 1,
                cc_year = 1,
                cc_csv = 1,
                user_id = 1
            };
            string receipt_url = "";
            try
            {
                var stripe_response = charge.StripeResponse.ResponseJson;
                dynamic data = JsonConvert.DeserializeObject(stripe_response);
                receipt_url = data.receipt_url;
                company_payment.stripe_receipt_url = receipt_url;
            }
            catch
            {
                // json failed
            }

            db.company_paymentss.Add(company_payment);

            /*   if (varinfo.Onboarding_session_numbers > 0)
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
               }*/

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb   = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.OrderConfirmation_Email(
                company_payment.local_invoice_number,
                String.IsNullOrEmpty(orderViewModel.VarInfo.First_nm) && String.IsNullOrEmpty(orderViewModel.VarInfo.Last_nm) ? "Customer" : orderViewModel.VarInfo.First_nm,
                orderViewModel.VarInfo.Last_nm,
                orderViewModel.VarInfo.Annual_plan_price.ToString(),
                orderViewModel.VarInfo.Onboarding_price.ToString(),
                (orderViewModel.VarInfo.Registered_dt.Value.AddYears(orderViewModel.VarInfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
                orderViewModel.VarInfo.Last_nm,
                orderViewModel.VarInfo.Company_nm,
                "",
                orderViewModel.VarInfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Video/Index",
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode={orderViewModel.VarInfo.Emailed_code_to_customer}&invitationcode=VAR&quickview={quickView}",
                orderViewModel.VarInfo.Year.ToString(),
                orderViewModel.VarInfo.Total_price.ToString()


                );
 

            ////            em.Send(to, cc, "Employee Confidential Registration", body, true);
            glb.SaveEmailBeforeSend(2, 2, 2, orderViewModel.VarInfo.Email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential", eb.Body, false, 67);

      #region   Purchase was made, need to send email to Partner / EC
      string invitationCode = VarConstants.DefaultVARCode;
      if (!string.IsNullOrWhiteSpace(orderViewModel.VarInfo.Invitation_code))
        invitationCode = orderViewModel.VarInfo.Invitation_code;

      var company_invitation = db.CompanyInvitations.Where(t => t.Invitation_code == invitationCode).FirstOrDefault();
      if (company_invitation != null)
      {
        /// need to add partner_id and replace company_id with it. 
        var partner = db.partners.Where(t => t.id == company_invitation.Created_by_company_id).FirstOrDefault();

        if (partner != null)
        {
          EC.Business.Actions.Email.EmailBody eb_partner = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
          eb_partner.GetConfirmationTextPartner(

                 String.IsNullOrEmpty(orderViewModel.VarInfo.First_nm) && String.IsNullOrEmpty(orderViewModel.VarInfo.Last_nm) ? "Customer" : orderViewModel.VarInfo.First_nm,
                 orderViewModel.VarInfo.Last_nm,
                 orderViewModel.VarInfo.Annual_plan_price.ToString(),
                 orderViewModel.VarInfo.Onboarding_price.ToString(),
                 (orderViewModel.VarInfo.Registered_dt.Value.AddYears(orderViewModel.VarInfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
                 company_payment.local_invoice_number,
                 orderViewModel.VarInfo.Company_nm,
                  invitationCode,
                  orderViewModel.VarInfo.Year.ToString(),
                  orderViewModel.VarInfo.Total_price.ToString()
                 );

 
          glb.SaveEmailBeforeSend(2, 2, 2, partner.email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential: #" + company_payment.local_invoice_number, eb_partner.Body, false, 77);

        }

      }
      var partnerEC = db.partners.Where(t => t.partner_code == "EC").FirstOrDefault();
      if (partnerEC != null)
      {
        EC.Business.Actions.Email.EmailBody eb_EC = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
        eb_EC.GetConfirmationTextEC(

               String.IsNullOrEmpty(orderViewModel.VarInfo.First_nm) && String.IsNullOrEmpty(orderViewModel.VarInfo.Last_nm) ? "Customer" : orderViewModel.VarInfo.First_nm,
               orderViewModel.VarInfo.Last_nm,
               orderViewModel.VarInfo.Annual_plan_price.ToString(),
               orderViewModel.VarInfo.Onboarding_price.ToString(),
               (orderViewModel.VarInfo.Registered_dt.Value.AddYears(orderViewModel.VarInfo.Year)).ToString("MMMM dd, yyyy", new CultureInfo("en-US")),
               company_payment.local_invoice_number,
               orderViewModel.VarInfo.Company_nm,
                invitationCode,
                  orderViewModel.VarInfo.Year.ToString(),
                  orderViewModel.VarInfo.Total_price.ToString()
               );

 
        glb.SaveEmailBeforeSend(2, 2, 2, partnerEC.email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Order Confirmation from Employee Confidential: #" + company_payment.local_invoice_number, eb_EC.Body, false, 78);

      }

      #endregion


      ViewBag.Emailed_code_to_customer = orderViewModel.VarInfo.Emailed_code_to_customer;
            ViewBag.ReceiptUrl = receipt_url;
            orderViewModel.receiptUrl = receipt_url;
            ViewBag.RedirectLink = "/Book/CompanyRegistrationVideo?emailedcode=" + orderViewModel.VarInfo.Emailed_code_to_customer +
                "&invitationCode=" + INVITATIONCODE + "&quickView=" + quickView;
            return View("~/Views/Book/OrderPaymentReceipt.cshtml", orderViewModel.VarInfo);
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
                }
                else
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
                Description = "Employee Confidential Onboarding Training",
                SourceId = token,
                Amount = form.Amount * 100
            };
            string receipt_url = "";
            var service = new ChargeService();
            Charge charge = service.Create(options);

            var db = new DBContext();
            string id = charge.Id;
            var company = db.company.Where(t => t.guid == form.CompanyGuid).FirstOrDefault();
            if (company != null)
            {
                int numberOfSessions = form.SessionNumber;
                company.onboard_sessions_paid = company.onboard_sessions_paid + numberOfSessions;
                company.onboard_sessions_expiry_dt = DateTime.Today.AddYears(1);


                string time_now = DateTime.Now.ToString("yyMMddHHmmss");

                var training_payment = new company_payment_training
                {
                    company_id = company.id,
                    payment_date = DateTime.Today,
                    onboard_sessions_expiry_dt = DateTime.Today.AddYears(1),
                    auth_code = token,
                    amount = form.Amount,
                    onboard_sessions_paid = form.SessionNumber,
                    user_id = 1,
                    payment_code = "ECT-" + company.id.ToString() + "-" + time_now,
                    training_code = "ECT-" + company.id.ToString() + "-" + time_now,
                    local_invoice_number = "ECT-" + company.id.ToString() + "-" + time_now
                };


                try
                {
                    var stripe_response = charge.StripeResponse.ResponseJson;
                    dynamic data = JsonConvert.DeserializeObject(stripe_response);
                    receipt_url = data.receipt_url;
                    training_payment.stripe_receipt_url = receipt_url;
                }
                catch
                {
                    // json failed
                }



                db.company_payment_trainings.Add(training_payment);

                db.SaveChanges();
            }
            #region Purchase Confirmation email

            glb.BookingECOnboardingSessionNotifications(company, id, form.Amount, form.SessionNumber, is_cc, this.Request);
            #endregion
            ViewBag.ReceiptUrl = receipt_url;
            form.receiptUrl = receipt_url;

            return View("~/Views/Book/OnboardingPaymentReceipt.cshtml", form);
            //return Redirect("https://report.employeeconfidential.com/trainer/calendar/");

        }

        public ActionResult OrderPaymentReceipt(VarInfoModel VarInfo)
        {

            var db = new DBContext();
            string receiptUrl = "";

            var payment = db.company_paymentss.Where(t => t.auth_code == VarInfo.Emailed_code_to_customer).FirstOrDefault();
            if (payment != null)
                receiptUrl = payment.stripe_receipt_url;
            ViewBag.receiptUrl = receiptUrl;
            return View();
        }
    }
}