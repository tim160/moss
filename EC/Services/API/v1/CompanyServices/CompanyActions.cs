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
				company.last_update_dt = DateTime.Now;
			});
		}
	}
}