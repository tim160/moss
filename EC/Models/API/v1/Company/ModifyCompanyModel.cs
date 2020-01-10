using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Company
{
	public class ModifyCompanyModel
	{
		[Required]
		[StringLength(500)]
		public string Name { get; set; }
	}
}