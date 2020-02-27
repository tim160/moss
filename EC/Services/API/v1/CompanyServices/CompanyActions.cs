using System;
using System.Data.Entity;
using System.Threading.Tasks;
using EC.Models.API.v1.Company;
using EC.Models.Database;

namespace EC.Services.API.v1.CompanyServices
{
	internal static class CompanyActions
	{
		public static Task<company> UpdateAsync(this DbSet<company> companies, int id, UpdateCompanyModel updateCompanyModel)
		{
			return companies.UpdateAsync(id, updateCompanyModel, company =>
            {
                company.company_nm = updateCompanyModel.CompanyName;
                company.employee_quantity = updateCompanyModel.EmployeeQuantity;
                company.partner_api_id = updateCompanyModel.PartnerCompanyId;
                company.last_update_dt = DateTime.Now;
			});
		}
	}
}