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
	public class CompanyService : ServiceBase<company>
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

        public async Task<List<AggregateData>> GetCompanyLocationsAnalytics(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            var allReportsQuery = _appContext.report.Where(r =>
                r.location_id != null && r.company_id == id && r.reported_dt > startDateTime &&
                r.reported_dt < endDateTime);

            var allReportsCount = await allReportsQuery.CountAsync();

            var locationsAnalytics = await allReportsQuery
                .Join(_appContext.company_location,
                    r => r.location_id,
                    cl => cl.id,
                    (r, cl) => new
                    {
                        Name = cl.location_en
                    })
                .GroupBy(g => g.Name).Select(g => new AggregateData
                {
                    Name = g.Key,
                    Quantity = g.Count(),
                    Percentage = allReportsCount != 0 ? ((decimal)g.Count() / allReportsCount * 100) : 0
                }).AsNoTracking().ToListAsync();

            locationsAnalytics.ForEach(l => l.Percentage = decimal.Parse(l.Percentage.ToString("0.00")) );

            return locationsAnalytics;
        }

        public async Task<List<AggregateData>> GetCompanyDepartmentsAnalytics(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            var departmentsAnalyticsQuery = _appContext.report_department
                .Join(_appContext.company_department.Where(cd => cd.company_id == id),
                    rd => rd.department_id,
                    cd => cd.id,
                    (rd, cd) => new
                    {
                        rd.report_id,
                        Name = cd.department_en
                    })
                .Join(
                    _appContext.report.Where(r =>
                        r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime),
                    rd => rd.report_id,
                    r => r.id,
                    (rd, r) => new
                    {
                        rd.Name
                    });

            var allReportsCount = await departmentsAnalyticsQuery.CountAsync();

            var departmentsAnalytics = await departmentsAnalyticsQuery
                .GroupBy(g => g.Name).Select(g => new AggregateData
                {
                    Name = g.Key,
                    Quantity = g.Count(),
                    Percentage = allReportsCount != 0 ? ((decimal)g.Count() / allReportsCount * 100) : 0
                }).AsNoTracking().ToListAsync();

            departmentsAnalytics = FormatPercentage(departmentsAnalytics);

            return departmentsAnalytics;
        }

        public async Task<List<AggregateData>> GetCompanyIncidentsAnalytics(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            var incidentsAnalyticsQuery = _appContext.report_secondary_type
                .Join(_appContext.company_secondary_type.Where(cd => cd.company_id == id),
                    rs => rs.secondary_type_id,
                    cst => cst.id,
                    (rs, cst) => new
                    {
                        rs.report_id,
                        Name = cst.secondary_type_en
                    })
                .Join(
                    _appContext.report.Where(r =>
                        r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime),
                    rd => rd.report_id,
                    r => r.id,
                    (rd, r) => new
                    {
                        rd.Name
                    });

            var allReportsCount = await incidentsAnalyticsQuery.CountAsync();

            var incidentsAnalytics = await incidentsAnalyticsQuery
                .GroupBy(g => g.Name).Select(g => new AggregateData
                {
                    Name = g.Key,
                    Quantity = g.Count(),
                    Percentage = allReportsCount != 0 ? ((decimal)g.Count() / allReportsCount * 100) : 0
                }).AsNoTracking().ToListAsync();

            incidentsAnalytics = FormatPercentage(incidentsAnalytics);

            return incidentsAnalytics;
        }

        public async Task<List<AggregateData>> GetCompanyReporterTypeAnalytics(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            var reporterTypesAnalyticsQuery = _appContext.report_relationship
                .Join(_appContext.company_relationship.Where(cd => cd.company_id == id),
                    rr => rr.company_relationship_id,
                    cr => cr.id,
                    (rr, cr) => new
                    {
                        rr.report_id,
                        Name = cr.relationship_en
                    })
                .Join(
                    _appContext.report.Where(r =>
                        r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime),
                    rd => rd.report_id,
                    r => r.id,
                    (rd, r) => new
                    {
                        rd.Name
                    });

            var allReportsCount = await reporterTypesAnalyticsQuery.CountAsync();

            var reporterTypesAnalytics = await reporterTypesAnalyticsQuery
                .GroupBy(g => g.Name).Select(g => new AggregateData
                {
                    Name = g.Key,
                    Quantity = g.Count(),
                    Percentage = allReportsCount != 0 ? ((decimal)g.Count() / allReportsCount * 100) : 0
                }).AsNoTracking().ToListAsync();

            reporterTypesAnalytics = FormatPercentage(reporterTypesAnalytics);

            return reporterTypesAnalytics;
        }

        private List<AggregateData> FormatPercentage(List<AggregateData> data)
        {
            data.ForEach(d => d.Percentage = decimal.Parse(d.Percentage.ToString("0.00")));

            return data;
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

    //TODO: move to common area
    public class ReportForEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ReportsCount { get; set; }
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

    public class DepartmentAnalyticViewModel
    {
        public List<AggregateData> DepartmentTable { get; set; }
    }

    public class IncidentAnalyticViewModel
    {
        public List<AggregateData> SecondaryTypeTable { get; set; }
    }

    public class ReporterTypeAnalyticViewModel
    {
        public List<AggregateData> RelationTable { get; set; }
    }
}