﻿using EC.Services.API.v1.ClientService;
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
using EC.Models.API.v1.Company;
using EC.Services.API.v1.CompanyServices;


namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/client")]
    [JwtAuthentication]
    [Authorize]
    public class ClientController : BaseApiController
    {
        private readonly ClientService _clientService;
        private readonly GlobalSettingsService _globalSettingsService;
        private readonly CompanyService _companyService;

        protected readonly ILog _logger;

        public ClientController()
        {
            _logger = LogManager.GetLogger(GetType());
            _clientService = new ClientService();
            _globalSettingsService = new GlobalSettingsService();
        }

        #region CRUD
    // should be 1 item, not list
    [HttpGet]
    [Route("{id}")]
    [ResponseType(typeof(ClientModel))]
    public async Task<IHttpActionResult> GetClient()
    {


      if (!ModelState.IsValid)
      {
        return ApiBadRequest(ModelState);
      }

      PagedList<ClientModel> result = await _clientService
          .GetPagedAsync(1, 1)
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

      if (createClientModel.globalSettings != null)
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

    // do not do it now
    [HttpPut]
    [Route("internal/{id}")]
    private async Task<IHttpActionResult> UpdateInternal(int id, UpdateClientModel updateClientModel)
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


    // do not do it now
    [HttpDelete]
    [Route("internal/{id}")]
    private async Task<IHttpActionResult> DeleteInternal(int id)
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
    #endregion


        #region Companies List by external clientID
    //filter by client_id ( note, it is external _client_id )
    [HttpGet]
    [Route("{id}/companies")]
    [ResponseType(typeof(PagedList<CompanyModel>))]
    public async Task<IHttpActionResult> GetCompaniesList()
    {


      if (!ModelState.IsValid)
      {
        return ApiBadRequest(ModelState);
      }

      PagedList<CompanyModel> result = await _companyService
        .GetPagedAsync(1, 1)
        .ConfigureAwait(false);
      return ApiOk(result);
    }
    #endregion



        // to do  - move to common area
        public class AggregateData
    {
      public int Name { get; set; }
      public int Quantity { get; set; }

      public decimal Percentage { get; set; }

    }

    #region Aggregate Data

    [HttpGet]
    [Route("{id}/aggregatedata/Departments")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateDepartments(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/aggregatedata/Locations")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateLocations(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/aggregatedata/IncidentTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateIncidentTypes(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    [HttpGet]
    [Route("{id}/aggregatedata/ReporterTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateReporterTypes(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/aggregatedata/behavioralFactors")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateBehavioralFactors(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/aggregatedata/externalInfluences")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateExternalInfluences(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    [HttpGet]
    [Route("{id}/aggregatedata/organizationalInfluences")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AggregateoOrganizationalInfluences(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    #endregion


    #region Analytics

    [HttpGet]
    [Route("{id}/analytics/Departments")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsDepartments(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/analytics/Locations")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsLocations(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/analytics/IncidentTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsIncidentTypes(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    [HttpGet]
    [Route("{id}/analytics/ReporterTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsReporterTypes(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/analytics/behavioralFactors")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsBehavioralFactors(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("{id}/analytics/externalInfluences")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsExternalInfluences(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    [HttpGet]
    [Route("{id}/analytics/organizationalInfluences")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> AnalyticsOrganizationalInfluences(string start_dt, string end_dt)
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    #endregion
  }
}