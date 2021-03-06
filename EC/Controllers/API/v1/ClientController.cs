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
using System.Collections.Generic;
using EC.Models.Database;
using EC.Utils.Auth;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/clients")]
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
          if (string.IsNullOrEmpty(id))
            ModelState.AddModelError(nameof(id), "Client ID required.");

           int idFromDb = DB.client.Where(client => client.partner_api_id.Equals(id)).Select(client => client.id).FirstOrDefault();
           if (idFromDb == 0)
           {
               ModelState.AddModelError(nameof(id), "Client not found.");
           }
           if (updateClientModel == null)
             ModelState.AddModelError(nameof(updateClientModel), "Client data required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

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

            try
            {
                await _globalSettingsService
                    .UpdateAsync(updateClientModel.GlobalSettings, Int32.Parse(id))
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

//        [HttpPut]
//        [Route("{id}")]
//        public async Task<IHttpActionResult> Update(string id, UpdateClientModel updateClientModel)
//        {
//            _logger.Debug($"id={id}");
//
//            if (updateClientModel == null)
//            {
//                ModelState.AddModelError(nameof(updateClientModel), "Client data required.");
//            }
//            if (String.IsNullOrEmpty(id))
//            {
//                ModelState.AddModelError(nameof(id), "Client ID required.");
//            }
//
//            int idFromDb = DB.client.Where(client => client.partner_api_id.Equals(id)).Select(client => client.id).FirstOrDefault();
//            if (idFromDb == 0)
//            {
//                ModelState.AddModelError(nameof(id), "Client not found.");
//            }
//
//            if (!ModelState.IsValid)
//            {
//                return ApiBadRequest(ModelState);
//            }
//
//            try
//            {
//                await _clientService
//                    .UpdateAsync(updateClientModel, idFromDb)
//                    .ConfigureAwait(false);
//            }
//            catch (NotFoundException exception)
//            {
//                return ApiNotFound(exception.Message);
//            }
//
//            return ApiOk();
//        }

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

        #region Client Activate/Deactive

        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> ClientActivate(string id)
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
                ///   await _clientService
                ///      .DeleteAsync(idFromDb)
                //      .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> ClientDeactivate(string id)
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
                /// await _clientService
                ///    .DeleteAsync(idFromDb)
                ///     .ConfigureAwait(false);
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
            public string name { get; set; }
            public int quantity { get; set; }
            public decimal percentage { get; set; }

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
        public async Task<IHttpActionResult> AnalyticsDepartments(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/locations")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsLocations(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            List<AggregateData> result = new List<AggregateData>();
            //result.Add(new AggregateData() { name = "Seattle", quantity = 1, percentage =50});
            //result.Add(new AggregateData() { name = "New York", quantity = 1, percentage = 50 });
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/incidentTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsIncidentTypes(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/reporterTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsReporterTypes(string startDate, string endDate)
        {
            // _logger.Debug($"page={page}; pageSize={pageSize}");

            AggregateData result = new AggregateData();
            return ApiOk(result);
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