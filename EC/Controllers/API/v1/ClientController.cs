using EC.Services.API.v1.ClientService;
using System.Web.Http;
using log4net;
using EC.Models.API.v1.Client;
using EC.Common.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using EC.Errors.CommonExceptions;
using EC.Common.Util.Filters;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/client")]
    //[JwtAuthentication]
    //[Authorize]
    public class ClientController : BaseApiController
    {
        private readonly ClientService _clientService;
        protected readonly ILog _logger;

        public ClientController()
        {
            _logger = LogManager.GetLogger(GetType());
            _clientService = new ClientService();
        }

        [HttpPost]
        [Route("new")]
        public async Task<IHttpActionResult> Create(CreateClientModel createClientModel)
        {
            if (createClientModel == null)
            {
                ModelState.AddModelError(nameof(createClientModel), "Client data required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            bool isCC = DomainUtil.IsCC(Request.RequestUri.AbsoluteUri);
            int id = 0;

            try
            {
                id = await _clientService
                    .CreateAsync(createClientModel, isCC)
                    .ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }

            return ApiCreated(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, UpdateClientModel updateClientModel)
        {
            _logger.Debug($"id={id}");

            if (updateClientModel == null)
            {
                ModelState.AddModelError(nameof(updateClientModel), "Client data required.");
            }
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Client ID required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            try
            {
                await _clientService
                    .UpdateAsync(updateClientModel, id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpPut]
        [Route("externalclient/{id}")]
        public async Task<IHttpActionResult> UpdateExternalUser(string id, UpdateClientModel updateClientModel)
        {
            _logger.Debug($"id={id}");

            if (updateClientModel == null)
            {
                ModelState.AddModelError(nameof(updateClientModel), "Client data required.");
            }
            if (String.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(nameof(id), "Client ID required.");
            }

            int idFromDb = DB.client.Where(client => client.partner_api_id.Equals(id)).Select(client => client.id).FirstOrDefault();
            if (idFromDb == 0) 
            {
                ModelState.AddModelError(nameof(id), "Client not found.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _clientService
                    .UpdateAsync(updateClientModel, idFromDb)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Client ID required.");
            }

            try
            {
                await _clientService
                    .DeleteAsync(id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpDelete]
        [Route("externaldelete/{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(nameof(id), "Client ID required.");
            }
            int idFromDb = DB.client.Where(client => client.partner_api_id.Equals(id)).Select(client => client.id).FirstOrDefault();
            if (idFromDb == 0)
            {
                ModelState.AddModelError(nameof(id), "Client not found.");
            }

            try
            {
                await _clientService
                    .DeleteAsync(idFromDb)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }
    }
}