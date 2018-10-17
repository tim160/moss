using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EC.Controllers.API
{
    public class NewCompanyController : BaseApiController
    {
        public GlobalFunctions glb = new GlobalFunctions();

        public class NewCompanyModel
        {
            public string Data { get; set; }
            public string InvitationCode { get; set; }
            public string PrimaryLocationUp { get; set; }
            public int NumberEmployees { get; set; }
            public int NumberOfNonEmployees { get; set; }
            public int NumberOfClients { get; set; }
            public int Language { get; set; }
            public int Role { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public string CompanyName { get; set; }
            public string Phone { get; set; }
            public string Department { get; set; }
            public decimal Amount { get; set; }
            public string CreditCardNumber { get; set; }
            public string NameonCard { get; set; }
            public string ExpiryDate { get; set; }
            public string SelectedYear { get; set; }
            public string CSV { get; set; }
        }

        [HttpPost]
        [Route("api/NewCompany/Calc")]
        public object Calc(NewCompanyModel model)
        {
            model.Data = System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(model.Data));
            var strs = model.Data.Split('|');
            //var data = $"{model.InvitationCode}|{model.FirstName}|{model.LastName}|{model.CompanyName}|{model.Phone}|{model.Email}|{model.NumberOfEmployees}|{model.NumberOfNonEmployees}|{model.NumberOfClients}";
            model.InvitationCode = String.IsNullOrEmpty(model.InvitationCode) ? strs[0] : model.InvitationCode;
            model.FirstName = String.IsNullOrEmpty(model.FirstName) ? strs[1] : model.FirstName;
            model.LastName = String.IsNullOrEmpty(model.LastName) ? strs[2] : model.LastName;
            model.CompanyName = String.IsNullOrEmpty(model.Title) ? strs[3] : model.CompanyName;
            model.Phone = String.IsNullOrEmpty(model.Phone) ? strs[4] : model.Phone;
            model.Email = String.IsNullOrEmpty(model.Email) ? strs[5] : model.Email;
            model.NumberEmployees = model.NumberEmployees == 0 ? int.Parse(strs[6]) : model.NumberEmployees;
            model.NumberOfNonEmployees = model.NumberOfNonEmployees == 0 ? int.Parse(strs[7]) : model.NumberOfNonEmployees;
            model.NumberOfClients = model.NumberOfClients == 0 ? int.Parse(strs[8]) : model.NumberOfClients;

            var priceNE = 0m;
            var priceNNE = 0m;
            var priceC = 0m;
            var priceR = 0m;
            var items = DB.company_invitation
                .Where(x => x.invitation_code == model.InvitationCode)
                .ToList();

            var ne = items.FirstOrDefault(x => model.NumberEmployees >= x.from_quantity && model.NumberEmployees <= x.to_quantity);
            if ((ne != null) && (ne.employee_price.HasValue) && (ne.employee_price_type.HasValue))
            {
                priceNE = ne.employee_price_type.Value == 1 ? ne.employee_price.Value : ne.employee_price.Value * model.NumberEmployees;
                priceR = ne.onboarding_fee.Value;
            }
            var nne = items.FirstOrDefault(x => model.NumberOfNonEmployees >= x.from_quantity && model.NumberOfNonEmployees <= x.to_quantity);
            if ((nne != null) && (nne.contractor_price.HasValue) && (nne.contractor_price_type.HasValue))
            {
                priceNNE = nne.contractor_price_type.Value == 1 ? nne.contractor_price.Value : nne.contractor_price.Value * model.NumberOfNonEmployees;
            }
            var c = items.FirstOrDefault(x => model.NumberOfClients >= x.from_quantity && model.NumberOfClients <= x.to_quantity);
            if ((c != null) && (c.customer_price.HasValue) && (c.customer_price_type.HasValue))
            {
                priceC = c.customer_price_type.Value == 1 ? c.customer_price.Value : c.customer_price.Value * model.NumberOfClients;
            }

            model.Amount = priceNE + priceNNE + priceC + priceR;

            return new
            {
                Model = model,

                priceNE = priceNE,
                priceNNE = priceNNE,
                priceC = priceC,
                priceR = priceR,
            };
        }

        [HttpPost]
        [Route("api/NewCompany/Create")]
        public object Create(NewCompanyModel model)
        {
            bool result = false;
            var message = "";
            if (!String.IsNullOrEmpty(model.CompanyName))
            {
                result = !glb.isCompanyInUse(model.CompanyName);
                message = !result ? "Organization name is already in use" : message;
            }
            if (result)
            {

            }

            return new
            {
                Result = result,
                Message = message,
            };
        }
    }
}
