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
    [RoutePrefix("api/v1/clients")]
    [CustomAuthorization]
    public class ClientController : BaseApiController
    {
        private readonly ClientService _clientService;
        private readonly GlobalSettingsService _globalSettingsService;
        private readonly CompanyService _companyService;


        public ClientController()
        {
            _clientService = new ClientService();
            _globalSettingsService = new GlobalSettingsService();
            _companyService = new CompanyService();
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(ClientModel))]
        public async Task<IHttpActionResult> GetClient(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ApiBadRequest(ModelState);

            var idFromDb = await _clientService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Client not found.");
            }
 
            var client = await _clientService.GetClientById(idFromDb);
 
            if (client != null)
            {
                var clientViewModel = new ClientViewModel()
                {
                    Total = 1,
                    Items = new List<ClientModel>() {client}
                };
                return ApiOk(clientViewModel);

            }

            return ApiNotFound();
        }

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Create(List<CreateClientModel> createClientModel)
        {
            if (createClientModel == null || !createClientModel.Any())
                ModelState.AddModelError(nameof(createClientModel), "Client data required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            try
            {

                foreach (var t in createClientModel)
                {
                  if(string.IsNullOrWhiteSpace(t.PartnerClientId))
                    return ApiBadRequest("PartnerClientId required.");
                }

                foreach (var t in createClientModel)
                {
                    t.Id = (await _clientService
                        .CreateAsync(t)
                        .ConfigureAwait(false)).id;
                }
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }


            try
            {
                await _globalSettingsService
                    .CreateAsync(createClientModel)
                    .ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }

            return ApiCreated(createClientModel);
        }


    [HttpPut]
    [Route("{id}")]
    public async Task<IHttpActionResult> Update(string id, UpdateClientModel updateClientModel)
    {
      if (updateClientModel == null)
        ModelState.AddModelError(nameof(updateClientModel), "Client data required.");
 

      if (!ModelState.IsValid)
        return ApiBadRequest(ModelState);

      if (string.IsNullOrEmpty(id))
        return ApiBadRequest(ModelState);

      var idFromDb = await _clientService.GetInternalIDfromExternal(id);
      if (idFromDb == 0)
      {
        return ApiNotFound("Client not found.");
      }


      try
      {
        var client = DB.client.FirstOrDefault(c => c.id == idFromDb);
        var globalSettings = DB.global_settings.FirstOrDefault(gs => gs.client_id == idFromDb);

        if (client != null)
        {
          client.client_nm = updateClientModel.ClientName;
          // do not update this field, it is their id
    //      client.partner_api_id = updateClientModel.PartnerClientId;
          globalSettings.custom_logo_path = updateClientModel.GlobalSettings.CustomLogoPath;
          globalSettings.header_color_code = updateClientModel.GlobalSettings.HeaderColorCode;
          globalSettings.header_links_color_code = updateClientModel.GlobalSettings.HeaderLinksColorCode;

          await DB.SaveChangesAsync();


          return ApiOk();
        }

        return ApiNotFound();

      }
      catch (NotFoundException exception)
      {
        return ApiNotFound(exception.Message);
      }
              ///
              ////try
              ////{
              ////  await _clientService
              ////      .UpdateAsync(updateClientModel, idFromDb)
              ////      .ConfigureAwait(false);
              ////}
              ////catch (NotFoundException exception)
              ////{
              ////  return ApiNotFound(exception.Message);
              ////}

      try
      {
        await _globalSettingsService
            .UpdateAsync(updateClientModel.GlobalSettings, idFromDb)
            .ConfigureAwait(false);
      }
      catch (NotFoundException exception)
      {
        return ApiNotFound(exception.Message);
      }

      return ApiOk();
    }



    ////[HttpPut]
    ////    [Route("{id}")]
    ////    public async Task<IHttpActionResult> UpdateInternal(int id, UpdateClientModel updateClientModel)
    ////    {
    ////        if (updateClientModel == null)
    ////            ModelState.AddModelError(nameof(updateClientModel), "Client data required.");

    ////        if (id == 0)
    ////            ModelState.AddModelError(nameof(id), "Client ID required.");

    ////        if (!ModelState.IsValid)
    ////            return ApiBadRequest(ModelState);

    ////        try
    ////        {
    ////            await _clientService
    ////                .UpdateAsync(updateClientModel, id)
    ////                .ConfigureAwait(false);
    ////        }
    ////        catch (NotFoundException exception)
    ////        {
    ////            return ApiNotFound(exception.Message);
    ////        }

    ////        try
    ////        {
    ////            await _globalSettingsService
    ////                .UpdateAsync(updateClientModel.GlobalSettings, id)
    ////                .ConfigureAwait(false);
    ////        }
    ////        catch (NotFoundException exception)
    ////        {
    ////            return ApiNotFound(exception.Message);
    ////        }

    ////        return ApiOk();
    ////    }

        //[HttpDelete]
        //[Route("internal/{id}")]
        //private async Task<IHttpActionResult> DeleteInternal(int id)
        //{
        //    if (id == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "Client ID required.");
        //    }

        //    try
        //    {
        //        await _clientService
        //            .DeleteAsync(id)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IHttpActionResult> Delete(string id)
        //{
        //    if (String.IsNullOrEmpty(id))
        //    {
        //        ModelState.AddModelError(nameof(id), "Client ID required.");
        //    }
        //    int idFromDb = DB.client.Where(client => client.partner_api_id.Equals(id)).Select(client => client.id).FirstOrDefault();
        //    if (idFromDb == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "Client not found.");
        //    }

        //    try
        //    {
        //        await _clientService
        //            .DeleteAsync(idFromDb)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> ClientActivate(string id)
        {
            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _clientService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Client not found.");
            }
  
            try
            {
              var client = DB.client.FirstOrDefault(c => c.id == idFromDb);
              if (client != null)
              {
                client.status_id = ECStatusConstants.Active_Value;
                await DB.SaveChangesAsync();
                return ApiOk();
              }

              return ApiNotFound();

            }
            catch (NotFoundException exception)
            {
              return ApiNotFound(exception.Message);
            }


 
        }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> ClientDeactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _clientService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Client not found.");
            }

            try
            {
              var client = DB.client.FirstOrDefault(c => c.id == idFromDb);
              if (client != null)
              {
                client.status_id = ECStatusConstants.Inactive_Value;
                await DB.SaveChangesAsync();
                return ApiOk();
              }

              return ApiNotFound();
 
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }
 
        }

        //filter by client_id ( note, it is external _client_id )
        [HttpGet]
        [Route("{id}/companies")]
        [ResponseType(typeof(List<CompanyModel>))]
        public async Task<IHttpActionResult> GetCompaniesList(string id)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _clientService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Client not found.");
            }

          //Expression<Func<EC.Models.Database.company, bool>> filterApp = c => idFromDb == c.client_id;


          List<CompanyModel> result = await _companyService.GetCompaniesByClientId(idFromDb, id);
 

          //PagedList<CompanyModel> result1 = await _companyService
          //        .GetPagedAsync(1, 1, c=> c.client_id == idFromDb)
          //        .ConfigureAwait(false);

          //Expression<Func<EC.Models.Database.company, bool>> filterApp = c => idFromDb == c.client_id;
 
          return ApiOk(result);
        }

        #region Aggregate Data

        [HttpGet]
        [Route("{id}/aggregateData/departments")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateDepartments(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/locations")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateLocations(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/incidentTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateIncidentTypes(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/reporterTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateReporterTypes(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/behavioralFactors")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateBehavioralFactors(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/externalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateExternalInfluences(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/aggregateData/organizationalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AggregateoOrganizationalInfluences(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }
        #endregion

        #region Analytics

        [HttpGet]
        [Route("{id}/analytics/departments")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsDepartments(int id, string startDate = "", string endDate = "")
        {
            
            var clientDepartmentsAnalytics = await _clientService.GetClientDepartmentsAnalytics(id, startDate, endDate);

            return ApiOk(clientDepartmentsAnalytics);
        }

        [HttpGet]
        [Route("{id}/analytics/locations")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsLocations(int id, string startDate = "", string endDate = "")
        {
            var clientLocationsAnalytics = await _clientService.GetClientLocationsAnalytics(id, startDate, endDate);

            return ApiOk(clientLocationsAnalytics);
        }

        [HttpGet]
        [Route("{id}/analytics/incidentTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsIncidentTypes(int id, string startDate = "", string endDate = "")
        {
            var clientIncidentsAnalytics = await _clientService.GetClientIncidentsAnalytics(id, startDate, endDate);

            return ApiOk(clientIncidentsAnalytics);
        }

        [HttpGet]
        [Route("{id}/analytics/reporterTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsReporterTypes(int id, string startDate = "", string endDate = "")
        {
            var clientReporterTypeAnalytics = await _clientService.GetClientReporterTypeAnalytics(id, startDate, endDate);

            return ApiOk(clientReporterTypeAnalytics);
        }

        [HttpGet]
        [Route("{id}/analytics/behavioralFactors")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsBehavioralFactors(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/externalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsExternalInfluences(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/organizationalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsOrganizationalInfluences(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }
        #endregion
 

  }
}