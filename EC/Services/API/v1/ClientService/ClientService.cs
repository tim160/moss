using EC.Common.Base;
using EC.Constants;
using EC.Models.API.v1.Client;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using EC.Models.API.v1.GlobalSettings;
using EC.Services.API.v1.CompanyServices;

namespace EC.Services.API.v1.ClientService
{
    public class ClientService : ServiceBase<client>
    {
        private readonly CompanyService _companyService;

        public ClientService()
        {
            _companyService = new CompanyService();
        }

        public Task<PagedList<ClientModel>> GetPagedAsync(int page, int pageSize, Expression<Func<client, bool>> filter = null)
        {
            return GetPagedAsync<string, ClientModel>(page, pageSize, filter, null);
        }

        public async Task<ClientModel> GetClientById(string id)
        {
            var client = await _appContext.client.FirstOrDefaultAsync(c => c.partner_api_id == id);
            if (client != null)
            {
                var globalSettings = await _appContext.global_settings.FirstOrDefaultAsync(g => g.client_id == client.id);
                return new ClientModel()
                {
                    clientName = client.client_nm,
                    partnerClientId = client.partner_api_id,
                    globalSettings = new GlobalSettingsModel()
                    {
                        customLogoPath = globalSettings?.custom_logo_path,
                        headerColorCode = globalSettings?.header_color_code,
                        headerLinksColorCode = globalSettings?.header_links_color_code
                    }
                };
            }

            return null;
        }

        public async Task<client> CreateAsync(CreateClientModel createClientModel)
        {
            List<Exception> errors = await CheckPartnerUserId(createClientModel);

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var client = GetClientsFromCreatedModel(createClientModel);

            _appContext.client.Add(client);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return client;
        }

        public async Task<client> UpdateAsync(UpdateClientModel updateClientModel, int id)
        {
            updateClientModel.Id = id;
            client client = await _set
                .UpdateAsync(id, updateClientModel)
                .ConfigureAwait(false);

            client.last_update_dt = DateTime.Now;

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return client;
        }

        public async Task<int> DeleteAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            client clientForDelete = await _set.FindAsync(id);
            if (clientForDelete == null)
            {
                throw new ArgumentException("Client not found.", nameof(id));
            }

            clientForDelete.status_id = ECStatusConstants.Inactive_Value;
            client client = await _set
                .UpdateAsync(id, clientForDelete)
                .ConfigureAwait(false);
            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return client.id;
        }

        private async Task<List<Exception>> CheckPartnerUserId(CreateClientModel data)
        {
            List<Exception> errors = new List<Exception>();
            var clientsInDb = await _appContext.client.ToListAsync();
            var partnerInternal = clientsInDb
                    .FirstOrDefault(client => client.partner_api_id != null && client.partner_api_id.Equals(data.PartnerClientId));
                if (partnerInternal != null)
                    errors.Add(new Exception($"PartnerInternalID = {data.PartnerClientId} already exists"));

            return errors;
        }

        private client GetClientsFromCreatedModel(CreateClientModel clientToCreate)
        {
            return new client()
            {
                client_nm = clientToCreate.ClientName,
                last_update_dt = DateTime.Now,
                registration_dt = DateTime.Now,
                is_api = true,
                api_source_id = null,
                partner_api_id = clientToCreate.PartnerClientId
            };
        }

        public async Task<List<ClientCompanyDepartmentAggregateData>> GetClientDepartmentsAnalytics(int id, string startDate, string endDate)
        {
            var companies = await _appContext.company.Where(c => c.client_id == id).ToListAsync();
            List<ClientCompanyDepartmentAggregateData> result = new List<ClientCompanyDepartmentAggregateData>();
            foreach (var company in companies)
            {
                var departmentsAnalytics = await _companyService.GetCompanyDepartmentsAnalytics(company.id, startDate, endDate);
                result.Add(new ClientCompanyDepartmentAggregateData()
                {
                    CompanyName = company.company_nm,
                    DepartmentTable = departmentsAnalytics
                });
            }

            return result;
        }


        public async Task<List<ClientCompanyLocationAggregateData>> GetClientLocationsAnalytics(int id, string startDate, string endDate)
        {
            var companies = await _appContext.company.Where(c => c.client_id == id).ToListAsync();
            List<ClientCompanyLocationAggregateData> result = new List<ClientCompanyLocationAggregateData>();
            foreach (var company in companies)
            {
                var departmentsAnalytics = await _companyService.GetCompanyLocationsAnalytics(company.id, startDate, endDate);
                result.Add(new ClientCompanyLocationAggregateData()
                {
                    CompanyName = company.company_nm,
                    LocationTable = departmentsAnalytics
                });
            }

            return result;
        }

        public async Task<List<ClientCompanyIncidentAggregateData>> GetClientIncidentsAnalytics(int id, string startDate, string endDate)
        {
            var companies = await _appContext.company.Where(c => c.client_id == id).ToListAsync();
            List<ClientCompanyIncidentAggregateData> result = new List<ClientCompanyIncidentAggregateData>();
            foreach (var company in companies)
            {
                var departmentsAnalytics = await _companyService.GetCompanyLocationsAnalytics(company.id, startDate, endDate);
                result.Add(new ClientCompanyIncidentAggregateData()
                {
                    CompanyName = company.company_nm,
                    SecondaryTypeTable = departmentsAnalytics
                });
            }

            return result;
        }

        public async Task<List<ClientCompanyReporterTypeAggregateData>> GetClientReporterTypeAnalytics(int id, string startDate, string endDate)
        {
            var companies = await _appContext.company.Where(c => c.client_id == id).ToListAsync();
            List<ClientCompanyReporterTypeAggregateData> result = new List<ClientCompanyReporterTypeAggregateData>();
            foreach (var company in companies)
            {
                var departmentsAnalytics = await _companyService.GetCompanyLocationsAnalytics(company.id, startDate, endDate);
                result.Add(new ClientCompanyReporterTypeAggregateData()
                {
                    CompanyName = company.company_nm,
                    RelationTable = departmentsAnalytics
                });
            }

            return result;
        }
    }
}