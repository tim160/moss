using EC.COM.Data;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC.COM.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index(string id = "")
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

            return View(model);
        }

        public ActionResult Buy(CalculateModel model)
        {
            model = DoCalculate(model);

            var db = new DBContext();
            var varinfo = new Data.VarInfoModel
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
            };
            db.VarInfoes.Add(varinfo);
            db.SaveChanges();

            //var data = $"{model.InvitationCode}|{model.FirstName}|{model.LastName}|{model.CompanyName}|{model.Phone}|{model.Email}|{model.NumberOfEmployees}|{model.NumberOfNonEmployees}|{model.NumberOfClients}|{(model.PriceNE + model.PriceNNE + model.PriceC)}";
            //data = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data));
            //return Redirect($"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}new/company?data={data}");
            //return View();
            return RedirectToAction("Order", new { id = varinfo.Id, email = model.Email, company = model.CompanyName });
        }

        public class CalculateModel
        {
            public string InvitationCode { get; set; }
            public int NumberOfEmployees { get; set; }
            public int NumberOfNonEmployees { get; set; }
            public int NumberOfClients { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CompanyName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public decimal PriceNE { get; set; }
            public decimal PriceNNE { get; set; }
            public decimal PriceC { get; set; }
            public decimal PriceR { get; set; }
            public int Year { get; set; }
            public decimal AnnualyTotal { get; set; }
            public int sessionNumber { get; set; }
            public string sessionN { get; set; }

            public decimal GrandTotal { get; set; }
        }

        public CalculateModel DoCalculate(CalculateModel model)
        {
            model.PriceNE = 0m;
            model.PriceNNE = 0m;
            model.PriceC = 0m;
            model.PriceR = 0m;
            model.sessionNumber = 0;

            using (var db = new DBContext())
            {
                model.InvitationCode = String.IsNullOrEmpty(model.InvitationCode) ? "EC" : model.InvitationCode;
                var items = db.CompanyInvitations
                    .Where(x => x.Invitation_code == model.InvitationCode)
                    .ToList();

                var ne = items.FirstOrDefault(x => model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity);
                if ((ne != null) && (ne.Employee_price.HasValue) && (ne.Employee_price_type.HasValue))
                {
                    model.PriceNE = ne.Employee_price_type.Value == 1 ? ne.Employee_price.Value : ne.Employee_price.Value * model.NumberOfEmployees;
                    if (model.Year == 2)
                    {
                        model.PriceNE = ne.Employee_price_type.Value == 1 ? ne.TwoYearPerYear_employee_price.Value : ne.TwoYearPerYear_employee_price.Value * model.NumberOfEmployees;
                    }
                    model.PriceR = ne.Onboarding_fee.Value;
                    if(ne.Onboarding_session_numbers.HasValue)
                        model.sessionNumber = ne.Onboarding_session_numbers.Value;
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
            model.AnnualyTotal = model.PriceNE + model.PriceNNE + model.PriceC;
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
                priceNE = model.PriceNE.ToString("N0", nfi),
                priceNNE = model.PriceNNE.ToString("N0", nfi),
                priceC = model.PriceC.ToString("N0", nfi),
                priceT = (model.PriceNE + model.PriceNNE + model.PriceC).ToString("N0", nfi),
                priceR = model.PriceR.ToString("N0", nfi),
                sessionN = model.sessionN,
            };

            return new JsonResult
            {
                Data = data,
            };
        }

        public class OrderViewModel
        {
            public VarInfoModel VarInfo { get; set; }
            public string CardNo { get; set; }
            public string NameOnCard { get; set; }
            public int ExpirationMonth { get; set; }
            public int ExpirationYear { get; set; }
            public string CSVCode { get; set; }
        }

        public ActionResult Order(int id, string email, string company)
        {
            var db = new DBContext();
            var model = db.VarInfoes.FirstOrDefault(x => x.Id == id && x.Email == email && x.Company_nm == company);

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
            varinfo.Registered_dt = varinfo.Registered_dt ?? DateTime.Now;
            db.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(false);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
            eb.OrderConfirmation_Email(
                varinfo.Emailed_code_to_customer,
                varinfo.First_nm,
                varinfo.Last_nm,
                varinfo.Annual_plan_price.ToString(),
                varinfo.Onboarding_price.ToString(),
                (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd'st', yyyy", new CultureInfo("en-US")),
                varinfo.Last_nm,
                varinfo.Company_nm,
                model.NameOnCard,
                varinfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Video/Index",
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode{varinfo.Emailed_code_to_customer}&invitationcode=VAR");
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(varinfo.Email.Trim());
            em.Send(to, cc, "Employee Confidential Registration", body, true);

            return RedirectToAction("CompanyRegistrationVideo", "Book", new { emailedcode = varinfo.Emailed_code_to_customer, invitationcode = "VAR" });
        }

        public ActionResult CompanyRegistrationVideo(string emailedcode, string invitationcode)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(VarInfoModel varInfo, string stripeToken)
        {
            StripeConfiguration.SetApiKey("sk_test_4eC39HqLyjWDarjtT1zdp7dc");

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = stripeToken; // Using ASP.NET MVC

            var db = new DBContext();
            var varinfo = db.VarInfoes.FirstOrDefault(x => x.Id == varInfo.Id && x.Email == varInfo.Email);

            var options = new ChargeCreateOptions
            {
                Amount = System.Convert.ToInt64(varinfo.Total_price),
                Currency = "usd",
                Description = "Example charge",
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
                varinfo.First_nm,
                varinfo.Last_nm,
                varinfo.Annual_plan_price.ToString(),
                varinfo.Onboarding_price.ToString(),
                (varinfo.Registered_dt.Value.AddYears(varinfo.Year)).ToString("MMMM dd'st', yyyy", new CultureInfo("en-US")),
                varinfo.Last_nm,
                varinfo.Company_nm,
                //model.NameOnCard,
                "",
                varinfo.Last_nm,
                Request.Url.AbsoluteUri.ToLower(),
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Video/Index",
                $"{Request.Url.Scheme}://{Request.Url.Host}{(Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString())}/Book/CompanyRegistrationVideo?emailedcode{varinfo.Emailed_code_to_customer}&invitationcode=VAR");
            string body = eb.Body;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            to.Add(varinfo.Email.Trim());
            em.Send(to, cc, "Employee Confidential Registration", body, true);

            return RedirectToAction("CompanyRegistrationVideo", "Book", new { emailedcode = varinfo.Emailed_code_to_customer, invitationcode = "VAR" });
        }
    }
}