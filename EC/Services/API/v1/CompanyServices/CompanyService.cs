using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Common.Interfaces;
using EC.Common.Util;
using EC.Constants;
using EC.Core.Common;
using EC.Errors.CommonExceptions;
using EC.Localization;
using EC.Models.API.v1.Company;
using EC.Models.Database;
using static EC.Constants.LanguageConstants;

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


        public async Task<company> CreateAsync(CreateCompanyModel createCompanyModel, bool isCc)
		{
            List<Exception> errors = await CheckCreateCompanyForErrors(createCompanyModel, isCc);

            if (errors.Count > 0)
                throw new AggregateException(errors);

            company newCompany = GetCompaniesFromCreatedModel(createCompanyModel);

            _appContext.company.Add(newCompany);

            try
            {
                await _appContext
                    .SaveChangesAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

			
            return newCompany;
		}

		public async Task<int> UpdateAsync(UpdateCompanyModel updateCompanyModel, int id)
		{
			company company = await _set
				.UpdateAsync(id, updateCompanyModel)
				.ConfigureAwait(false);

            company.last_update_dt = DateTime.Now;

			await _appContext
				.SaveChangesAsync()
				.ConfigureAwait(false);

			return company.id;
		}

        public async Task<int> DeleteAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            company companyForDelete = await _set.FindAsync(id);
            if (companyForDelete == null)
            {
                throw new ArgumentException("Client not found.", nameof(id));
            }
            companyForDelete.status_id = ECStatusConstants.Inactive_Value;

            company client = await _set
                .UpdateAsync(id, companyForDelete)
                .ConfigureAwait(false);
            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return client.id;
        }

        private async Task<List<Exception>> CheckCreateCompanyForErrors(CreateCompanyModel data, bool isCc)
        {
            List<Exception> errors = new List<Exception>();

            var generateRecordsModel = new Models.GenerateRecordsModel();
            var companiesInDb = await _appContext.company.ToListAsync();
            IEmailAddressHelper emailAddressHelper = new EmailAddressHelper();
            string messageCompanyInUse = LocalizationGetter.GetString("CompanyInUse", isCc);
           
            if (generateRecordsModel.isCompanyInUse(data.CompanyName))
                errors.Add(new AlreadyExistsException(messageCompanyInUse, data.CompanyName));

            string shortName = await generateRecordsModel.GenerateUnusedCompanyShortName(StringUtil.ShortString(data.CompanyName));
            if (string.IsNullOrWhiteSpace(shortName))
                errors.Add(new AlreadyExistsException(messageCompanyInUse, data.CompanyName));

            if (!string.IsNullOrEmpty(data.PartnerCompanyId))
            {
                var partnerInternal = companiesInDb
                    .FirstOrDefault(user => user.partner_api_id != null && user.partner_api_id.Equals(data.PartnerCompanyId));
                if (partnerInternal != null)
                    errors.Add(new Exception($"PartnerInternalID = {data.PartnerCompanyId} already exists"));
            }
            
            return errors;
        }

        public async Task<List<AggregateData>> GetCompanyLocationAnalytic(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            
            var allReportsCount = await _appContext.report.CountAsync(r => r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime);

            var locationsAnalytic = await _appContext.company_location.Where(l => l.company_id == id)
                                .Select(l => new
                                {
                                    Id = l.id,
                                    Name = l.location_en,
                                    ReportsCount = _appContext.report.Count(rep => rep.location_id == l.id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime)
                                })
                                .GroupJoin(_appContext.report.Where(rep => rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                                           l => l.Id,
                                           r => r.location_id,
                                           (l, r) => new AggregateData
                                           {
                                               Name = l.Name,
                                               Quantity = l.ReportsCount,
                                               Percentage = allReportsCount != 0 ? ((decimal)l.ReportsCount / allReportsCount * 100) : 0
                                           }).ToListAsync();

            return locationsAnalytic;
        }

        private company GetCompaniesFromCreatedModel(CreateCompanyModel createCompanyModels)
        {
            return new company()
            {
                //PartnerClientId
                //OptinCaseAnalytics
                //CustomLogoPath
                company_nm = createCompanyModels.CompanyName,
                partner_api_id = createCompanyModels.PartnerCompanyId,
                address_id = 1,
                employee_quantity = createCompanyModels.EmployeeQuantity.ToString(),
                registration_dt = DateTime.Now,
                last_update_dt = DateTime.Now
            };
        }
    }

    public class AggregateData
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Percentage { get; set; }
    }

    public class LocationAnalyticViewModel
    {
        public List<AggregateData> LocationTable { get; set; }
    }
}