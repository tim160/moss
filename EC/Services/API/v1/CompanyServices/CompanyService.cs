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

        public async Task<List<AggregateData>> GetCompanyLocationsAnalytics(int id, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateTime))
                startDateTime = DateTime.MinValue;

            if (!DateTime.TryParse(endDate, out DateTime endDateTime))
                endDateTime = DateTime.MaxValue;

            var allReportsCount = await _appContext.report.CountAsync(r => r.location_id != null && r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime);

            var locationsAnalytics = await _appContext.report
                .Where(r => r.location_id != null && r.company_id == id && r.reported_dt > startDateTime && r.reported_dt < endDateTime)
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

            var allReportsCount = await _appContext.company_department.Where(cd => cd.company_id == id)
                .Join(_appContext.report_department,
                    cd => cd.id,
                    rd => rd.department_id,
                    (cd, rd) => new {rd.report_id})
                .Join(
                    _appContext.report.Where(rep =>
                        rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                    sr => sr.report_id,
                    r => r.id,
                    (sr, r) => new { }).CountAsync();

            var departmentsAnalytics = await _appContext.company_department.Where(i => i.company_id == id)
                .Select(d => new ReportForEntity
                {
                    Id = d.id,
                    Name = d.department_en,
                    ReportsCount = _appContext.report_department.Where(rd => rd.department_id == d.id)
                        .Join(_appContext.report.Where(rep => rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                            type => type.report_id,
                            rep => rep.id,
                            (dep, rep) => new { }).Count()
                }).Select(e => new AggregateData()
                {
                    Name = e.Name,
                    Quantity = e.ReportsCount,
                    Percentage = allReportsCount != 0 ? ((decimal)e.ReportsCount / allReportsCount * 100) : 0
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

            var allReportsCount = await _appContext.company_secondary_type.Where(cst => cst.company_id == id)
                .Join(_appContext.report_secondary_type,
                    cst => cst.id,
                    rst => rst.secondary_type_id,
                    (cst, rst) => new { report_id = rst.report_id })
                .Join(
                    _appContext.report.Where(rep => rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                    type => type.report_id,
                    rep => rep.id,
                    (dep, rep) => new { }).CountAsync();

            var incidentsAnalytics = await  _appContext.company_secondary_type.Where(i => i.company_id == id)
                .Select(t => new ReportForEntity
                {
                    Id = t.id,
                    Name = t.secondary_type_en,
                    ReportsCount = _appContext.report_secondary_type.Where(rd => rd.secondary_type_id == t.id)
                        .Join(_appContext.report.Where(rep => rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                            type => type.report_id,
                            rep => rep.id,
                            (dep, rep) => new {}).Count()
                }).Select(e => new AggregateData()
                {
                    Name = e.Name,
                    Quantity = e.ReportsCount,
                    Percentage = allReportsCount != 0 ? ((decimal)e.ReportsCount / allReportsCount * 100) : 0
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

            var allReportsCount = await _appContext.company_relationship.Where(cr => cr.company_id == id)
                .Join(_appContext.report_relationship,
                    cr => cr.id,
                    rr => rr.company_relationship_id,
                    (cr, rr) => new {report_id = rr.report_id})
                .Join(
                    _appContext.report.Where(rep =>
                        rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                    rel => rel.report_id,
                    rep => rep.id,
                    (rel, rep) => new { }).CountAsync();

            var reporterTypesAnalytics = await _appContext.company_relationship.Where(c => c.company_id == id)
                .Select(r => new ReportForEntity
                {
                    Id = r.id,
                    Name = r.relationship_en,
                    ReportsCount = _appContext.report_relationship.Where(rd => rd.company_relationship_id == r.id)
                        .Join(_appContext.report.Where(rep => rep.company_id == id && rep.reported_dt > startDateTime && rep.reported_dt < endDateTime),
                            type => type.report_id,
                            rep => rep.id,
                            (dep, rep) => new { }).Count()
                }).Select( e => new AggregateData()
                {
                    Name = e.Name,
                    Quantity = e.ReportsCount,
                    Percentage = allReportsCount != 0 ? ((decimal)e.ReportsCount / allReportsCount * 100) : 0
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