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

namespace EC.Services.API.v1.ClientService
{
    public class ClientService : ServiceBase<client>
    {
        public Task<PagedList<ClientModel>> GetPagedAsync(int page, int pageSize, Expression<Func<client, bool>> filter = null)
        {
            return GetPagedAsync<string, ClientModel>(page, pageSize, filter, null);
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
    }
}