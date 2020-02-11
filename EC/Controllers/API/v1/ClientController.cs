using EC.Services.API.v1.ClientService;
using System.Web.Http;
using log4net;
using EC.Models.API.v1.Client;
using EC.Common.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using EC.Errors.CommonExceptions;
using EC.Services.API.v1.GlobalSettingsService;
using EC.Common.Util.Filters;
using System.Web.Http.Description;
using EC.Common.Base;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/client")]
    [JwtAuthentication]
    [Authorize]
    public class ClientController : BaseApiController
    {
        private readonly ClientService _clientService;
        private readonly GlobalSettingsService _globalSettingsService;

        protected readonly ILog _logger;

        public ClientController()
        {
            _logger = LogManager.GetLogger(GetType());
            _clientService = new ClientService();
            _globalSettingsService = new GlobalSettingsService();
        }

        [HttpGet]
        [Route]
        [ResponseType(typeof(PagedList<ClientModel>))]
        public async Task<IHttpActionResult> GetList(int page = 1, int pageSize = 10)
        {
            _logger.Debug($"page={page}; pageSize={pageSize}");

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            PagedList<ClientModel> result = await _clientService
                .GetPagedAsync(page, pageSize)
                .ConfigureAwait(false);

            result.Items.ForEach(entity =>
            {
                entity.globalSettings = _globalSettingsService.getByClientId(entity.id);
            });
            return ApiOk(result);
        }

        [HttpPost]
        [Route()]
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

            if (createClientModel.globalSettings!=null)
            {
                try
                {
                    createClientModel.globalSettings.client_id = id;
                    await _globalSettingsService
                        .CreateAsync(createClientModel.globalSettings, isCC)
                        .ConfigureAwait(false);
                }
                catch (AggregateException exception)
                {
                    return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
                }
            }

            return ApiCreated(id);
        }

        [HttpPut]
        [Route("internal/{id}")]
        public async Task<IHttpActionResult> UpdateInternal(int id, UpdateClientModel updateClientModel)
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

            if (updateClientModel.globalSettings != null)
            {
                try
                {
                    await _globalSettingsService
                        .UpdateAsync(updateClientModel.globalSettings, id)
                        .ConfigureAwait(false);
                }
                catch (NotFoundException exception)
                {
                    return ApiNotFound(exception.Message);
                }
            }

            return ApiOk();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(string id, UpdateClientModel updateClientModel)
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
        [Route("internal/{id}")]
        public async Task<IHttpActionResult> DeleteInternal(int id)
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
        [Route("{id}")]
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