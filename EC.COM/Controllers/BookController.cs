using EC.COM.Data;
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
            var data = $"{model.InvitationCode}|{model.FirstName}|{model.LastName}|{model.CompanyName}|{model.Phone}|{model.Email}|{model.NumberOfEmployees}|{model.NumberOfNonEmployees}|{model.NumberOfClients}";
            data = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data));
            return Redirect($"{System.Configuration.ConfigurationManager.AppSettings["MainSite"]}new/company?data={data}");
            //return View();
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
        }

        [HttpPost]
        public JsonResult Calculate(CalculateModel model)
        {
            var priceNE = 0m;
            var priceNNE = 0m;
            var priceC = 0m;
            var priceR = 0m;
            using (var db = new DBContext())
            {
                model.InvitationCode = String.IsNullOrEmpty(model.InvitationCode) ? "EC" : model.InvitationCode;
                var items = db.CompanyInvitations
                    .Where(x => x.Invitation_code == model.InvitationCode)
                    .ToList();

                var ne = items.FirstOrDefault(x => model.NumberOfEmployees >= x.From_quantity && model.NumberOfEmployees <= x.To_quantity);
                if ((ne != null) && (ne.Employee_price.HasValue) && (ne.Employee_price_type.HasValue))
                {
                    priceNE = ne.Employee_price_type.Value == 1 ? ne.Employee_price.Value : ne.Employee_price.Value * model.NumberOfEmployees;
                    priceR = ne.Onboarding_fee.Value;
                }
                var nne = items.FirstOrDefault(x => model.NumberOfNonEmployees >= x.From_quantity && model.NumberOfNonEmployees <= x.To_quantity);
                if ((nne != null) && (nne.Contractor_price.HasValue) && (nne.Contractor_price_type.HasValue))
                {
                    priceNNE = nne.Contractor_price_type.Value == 1 ? nne.Contractor_price.Value : nne.Contractor_price.Value * model.NumberOfNonEmployees;
                }
                var c = items.FirstOrDefault(x => model.NumberOfClients >= x.From_quantity && model.NumberOfClients <= x.To_quantity);
                if ((c != null) && (c.Employee_price.HasValue) && (c.Employee_price_type.HasValue))
                {
                    priceC = c.Customer_price_type.Value == 1 ? c.Customer_price.Value : c.Customer_price.Value * model.NumberOfClients;
                }
            }

            NumberFormatInfo nfi = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";

            var data = new
            {
                priceNE = priceNE.ToString("N0", nfi),
                priceNNE = priceNNE.ToString("N0", nfi),
                priceC = priceC.ToString("N0", nfi),
                priceT = (priceNE + priceNNE + priceC).ToString("N0", nfi),
                priceR = priceR.ToString("N0", nfi),
            };

            return new JsonResult
            {
                Data = data,
            };
        }
    }
}