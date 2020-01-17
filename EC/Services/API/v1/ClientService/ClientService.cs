﻿using EC.Models.API.v1.Client;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EC.Services.API.v1.ClientService
{
    internal class ClientService : ServiceBase<client>
    {
        public async Task<int> CreateAsync(CreateClientModel createClientModel, bool isCC)
        {
            if (createClientModel == null)
            {
                throw new ArgumentNullException(nameof(createClientModel));
            }

            List<Exception> errors = new List<Exception>();

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }

            client newClient = _set.Add(createClientModel, client =>
            { 
                client.address_id = createClientModel.address_id;
                client.status_id = createClientModel.status_id;
                client.client_nm = createClientModel.client_nm;
                client.client_ds = createClientModel.client_ds;
                client.notepad_tx = createClientModel.notepad_tx;
                client.user_id = createClientModel.user_id;
            });

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return newClient.id;
        }

        public async Task<int> UpdateAsync(UpdateClientModel updateClientModel, int id)
        {
            if (updateClientModel == null)
            {
                throw new ArgumentNullException(nameof(updateClientModel));
            }
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            client client = await _set
                .UpdateAsync(id, updateClientModel)
                .ConfigureAwait(false);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return client.id;
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

            clientForDelete.status_id = 2;
            client client = await _set
                .UpdateAsync(id, clientForDelete)
                .ConfigureAwait(false);
            return client.id;
        }
    }
}