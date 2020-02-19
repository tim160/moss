using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
using EC.Common.Util;
using EC.Common.Util.Filters;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.Company;
using EC.Services.API.v1.CompanyServices;
using EC.Models.API.v1.User;
using EC.Services.API.v1.UserService;
using System.Linq.Expressions;
using EC.Models.ViewModels;
using log4net;
using EC.Models.Database;
using EC.Constants;
using System.Collections.Generic;
using System.Data.Entity;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/companies")]

    public class CompanyController : BaseApiController
    {
        private readonly CompanyService _companyService;
        private readonly UserService _userService;
        protected readonly ILog _logger;

        public CompanyController()
        {
            _logger = LogManager.GetLogger(GetType());
            _companyService = new CompanyService();
        }
        // should be 1 item, not list
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CompanyModel))]
        public async Task<IHttpActionResult> GetCompany(int id)
        {
            var result = await DB.company.FirstOrDefaultAsync(u => u.id == id);

            if (result != null)
                return ApiOk(result);

            return ApiNotFound();
        }

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Create(CreateCompanyModel createCompanyModel)
        {
            if (createCompanyModel == null)
            {
                ModelState.AddModelError(nameof(createCompanyModel), "Company data required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            bool isCC = DomainUtil.IsCC(Request.RequestUri.AbsoluteUri);
            int id;
            try
            {
                id = await _companyService
                    .CreateAsync(createCompanyModel, isCC)
                    .ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }

            return ApiCreated(id);
        }
        // do not do it now
        [HttpPut]
        [Route("internal/{id}")]
        private async Task<IHttpActionResult> Update(int id, UpdateCompanyModel updateCompanyModel)
        {
            _logger.Debug($"id={id}");

            if (updateCompanyModel == null)
            {
                ModelState.AddModelError(nameof(updateCompanyModel), "Company data required.");
            }
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Company ID required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _companyService
                    .UpdateAsync(updateCompanyModel, id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateExternalCompany(string id, UpdateCompanyModel updateCompanyModel)
        {
            _logger.Debug($"id={id}");

            if (updateCompanyModel == null)
            {
                ModelState.AddModelError(nameof(updateCompanyModel), "Company data required.");
            }
            if (String.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(nameof(id), "Company ID required.");
            }

            int idFromDb = DB.company.Where(company => company.partner_api_id.Equals(id)).Select(company => company.id).FirstOrDefault();
            if (idFromDb == 0)
            {
                ModelState.AddModelError(nameof(id), "Company not found.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _companyService
                    .UpdateAsync(updateCompanyModel, idFromDb)
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
        private async Task<IHttpActionResult> Delete(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Company ID required.");
            }

            try
            {
                await _companyService
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
                ModelState.AddModelError(nameof(id), "Company ID required.");
            }

            int idFromDb = DB.company.Where(company => company.partner_api_id.Equals(id)).Select(company => company.id).FirstOrDefault();
            if (idFromDb == 0)
            {
                ModelState.AddModelError(nameof(id), "Company not found.");
            }

            try
            {
                await _companyService
                    .DeleteAsync(idFromDb)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }


        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> CompanyActivate(int id)
        {
            var company = await DB.company.FirstOrDefaultAsync(u => u.id == id);
            if (company != null)
            {
                company.status_id = 2;
                await DB.SaveChangesAsync();
                return ApiOk();
            }

            return ApiNotFound();
        }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> CompanyDeactivate(int id)
        {
            var company = await DB.company.FirstOrDefaultAsync(u => u.id == id);
            if (company != null)
            {
                company.status_id = 3;
                await DB.SaveChangesAsync();
                return ApiOk();
            }

            return ApiNotFound();
        }

        [HttpGet]
        [Route("{id}/users")]
        [ResponseType(typeof(PagedList<UserModel>))]
        public async Task<IHttpActionResult> GetUsersList(int page = 1, int pageSize = 10)
        {
            _logger.Debug($"page={page}; pageSize={pageSize}");

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            Expression<Func<user, bool>> filterApp = u => new[] { ECLevelConstants.level_ec_mediator, ECLevelConstants.level_escalation_mediator, ECLevelConstants.level_supervising_mediator }.Contains(u.role_id);
            PagedList<UserModel> result = await _userService
                .GetPagedAsync(page, pageSize, filterApp)
                .ConfigureAwait(false);
            var statusModel = new Models.ReadStatusModel();
            result.Items.ForEach(entity =>
            {
                entity.usersUnreadEntities = statusModel.GetUserUnreadEntitiesNumbers(entity.id);
            });

            return ApiOk(result);
        }

        // to do  - move to common area
        public class AggregateData
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public decimal percentage { get; set; }
        }

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

            AggregateData result = new AggregateData();
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
