using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.Client;
using EC.Models.API.v1.Company;
using EC.Services.API.v1.ClientService;
using EC.Services.API.v1.CompanyServices;
using EC.Services.API.v1.GlobalSettingsService;
using TestApi.Utils;
using EC.Constants;
using System.Net.Http;
using System.Linq.Expressions;


namespace TestApi.Controllers
{
    [RoutePrefix("api/v1/partners")]
    [CustomAuthorization]
    public class PartnerController : BaseApiController
    {
        private readonly ClientService _clientService;


        public PartnerController()
        {
            _clientService = new ClientService();
        }



    //filter by client_id ( note, it is external _client_id )
    [HttpGet]
    [Route("clients")]
    [ResponseType(typeof(List<CompanyModel>))]
    public async Task<IHttpActionResult> GetClientsList()
    {

      //PagedList<CompanyModel> result1 = await _companyService
      //        .GetPagedAsync(1, 1, c=> c.client_id == idFromDb)
      //        .ConfigureAwait(false);

      List<ClientModel> result = await _clientService.GetAllClients();
      if (result != null)
      {
        var clientViewModel = new ClientViewModel()
        {
          Total = result.Count(),
          Items = result  
        };
        return ApiOk(clientViewModel);

      }

      return ApiNotFound("Clients not found.");
    }

  }
}