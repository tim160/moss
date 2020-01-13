using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Company
{
	public class UpdateCompanyModel
	{
		[Required]
		[StringLength(250)]
		public string EmployeeQuantity { get; set; }
	}
}