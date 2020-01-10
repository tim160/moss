using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Models.API.v1.Company;
using EC.Models.Database;

namespace EC.Services.API.v1.CompanyServices
{
	internal class CompanyService : ServiceBase<company>
	{
		public Task<PagedList<CompanyModel>> GetPagedAsync(
			int page,
			int pageSize,
			Expression<Func<company, bool>> filter = null)
		{
			return GetPagedAsync<string, CompanyModel>(page, pageSize, filter, company => company.company_nm);
		}

		public async Task<int> CreateOrUpdate(ModifyCompanyModel modifyCompanyModel, int id = 0)
		{
			company company;
			if (id == 0)
			{
				company = _set.Add(modifyCompanyModel);
			}
			else
			{
				company = await _set
					.Update(id, modifyCompanyModel)
					.ConfigureAwait(false);
			}

			await _appContext
				.SaveChangesAsync()
				.ConfigureAwait(false);
			return company.id;
		}
	}
}