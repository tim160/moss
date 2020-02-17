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
    [RoutePrefix("api/v1/partners")]
  [JwtAuthentication]
  [Authorize]
  public class PartnerController : BaseApiController
    {
        private readonly ClientService _clientService;
        private readonly GlobalSettingsService _globalSettingsService;

        protected readonly ILog _logger;

        public PartnerController()
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

 
    }
}