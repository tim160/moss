using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public class CompanyInvitationModel
    {
        public int Id { get; set; }
        public string Invitation_code { get; set; }
        public int Created_by_company_id { get; set; }
        public int Created_by_user_id { get; set; }
        public DateTime Created_dt { get; set; }
        public int Is_active { get; set; }
        public string Comments { get; set; }
        public int? Reseller_type_id { get; set; }
        public int? Reseller_level { get; set; }
        public int? Reseller_comission { get; set; }
        public int? reseller_discount { get; set; }
        public int? From_quantity { get; set; }
        public int? To_quantity { get; set; }
        public decimal? Employee_price { get; set; }
        public int? Employee_price_type { get; set; }
        public decimal? Contractor_price { get; set; }
        public int? Contractor_price_type { get; set; }
        public decimal? Customer_price { get; set; }
        public int? Customer_price_type { get; set; }
        public decimal? Onboarding_fee { get; set; }
    }
}