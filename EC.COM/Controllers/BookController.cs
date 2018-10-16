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
            ViewBag.FirstName = data[0];
            ViewBag.LastName = data[1];
            ViewBag.CompanyName = data[2];
            ViewBag.Phone = data[3];
            ViewBag.Email = data[4];
            ViewBag.NumberOfEmployees = data[5];
            ViewBag.InvitationCode = data[6];

            return View();
        }

        public ActionResult Buy(CalculateModel model)
        {
            return View();
        }

        public class CalculateModel
        {
            public string Id { get; set; }
            public string InvitationCode { get; set; }
            public int NumberOfEmployees { get; set; }
            public int NumberOfNonEmployees { get; set; }
            public int NumberOfClients { get; set; }
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
                if ((nne != null) && (nne.Employee_price.HasValue) && (nne.Employee_price_type.HasValue))
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