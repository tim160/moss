using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Company
{
	public class UpdateCompanyModel
	{
        public string CompanyName { get; set; }
        public string PartnerCompanyId { get; set; }
        public string PartnerClientId { get; set; }
        public string OptinCaseAnalytics { get; set; }
        public string EmployeeQuantity { get; set; }
        public string CustomLogoPath { get; set; }
    }
}