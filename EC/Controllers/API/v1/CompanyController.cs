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
using EC.Utils.Auth;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/companies")]
    [CustomAuthorize]
    public class CompanyController : BaseApiController
    {
        private readonly CompanyService _companyService;
        private readonly UserService _userService;
        protected readonly ILog _logger;

        public CompanyController()
        {
            _logger = LogManager.GetLogger(GetType());
            _companyService = new CompanyService();
            _userService = new UserService();
        }

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
        public async Task<IHttpActionResult> Create(List<CreateCompanyModel> createCompanyModel)
        {
            if (createCompanyModel == null || !createCompanyModel.Any())
                ModelState.AddModelError(nameof(createCompanyModel), "Company data required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            try
            {
                foreach (var c in createCompanyModel)
                {
                    var companyId = (await _companyService
                        .CreateAsync(c, DomainUtil.IsCC(Request.RequestUri.AbsoluteUri))
                        .ConfigureAwait(false)).id;
                    c.Users.ForEach(u => u.PartnerCompanyId = companyId.ToString());
                }

                foreach (var c in createCompanyModel)
                {
                    await _userService
                        .CreateAsync(c.Users)
                        .ConfigureAwait(false);
                }

                return ApiCreated(createCompanyModel);

            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, UpdateCompanyModel updateCompanyModel)
        {

            if (updateCompanyModel == null)
                ModelState.AddModelError(nameof(updateCompanyModel), "Company data required.");

            if (id == 0)
                ModelState.AddModelError(nameof(id), "Company ID required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

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

//        [HttpPut]
//        [Route("{id}")]
//        public async Task<IHttpActionResult> UpdateExternalCompany(string id, UpdateCompanyModel updateCompanyModel)
//        {
//            _logger.Debug($"id={id}");
//
//            if (updateCompanyModel == null)
//            {
//                ModelState.AddModelError(nameof(updateCompanyModel), "Company data required.");
//            }
//            if (String.IsNullOrEmpty(id))
//            {
//                ModelState.AddModelError(nameof(id), "Company ID required.");
//            }
//
//            int idFromDb = DB.company.Where(company => company.partner_api_id.Equals(id)).Select(company => company.id).FirstOrDefault();
//            if (idFromDb == 0)
//            {
//                ModelState.AddModelError(nameof(id), "Company not found.");
//            }
//
//            if (!ModelState.IsValid)
//            {
//                return ApiBadRequest(ModelState);
//            }
//
//            try
//            {
//                await _companyService
//                    .UpdateAsync(updateCompanyModel, idFromDb)
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

        #region Analytics

        [HttpGet]
        [Route("{id}/analytics/departments")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsDepartments(int id, string startDate = "", string endDate = "")
        {
            var departmentsAnalytics = await _companyService.GetCompanyDepartmentsAnalytics(id, startDate, endDate);

            var result = new DepartmentAnalyticViewModel()
            {
                DepartmentTable = departmentsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/locations")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsLocations(int id, string startDate = "", string endDate = "")
        {
            var locationsAnalytics = await _companyService.GetCompanyLocationsAnalytics(id, startDate, endDate);

            var result = new LocationAnalyticViewModel()
            {
                LocationTable = locationsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/incidentTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsIncidentTypes(int id, string startDate = "", string endDate = "")
        {
            var incidentsAnalytics = await _companyService.GetCompanyIncidentsAnalytics(id, startDate, endDate);

            var result = new IncidentAnalyticViewModel()
            {
                SecondaryTypeTable = incidentsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/reporterTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsReporterTypes(int id, string startDate = "", string endDate = "")
        {
            var reporterTypesAnalytics = await _companyService.GetCompanyReporterTypeAnalytics(id, startDate, endDate);

            var result = new ReporterTypeAnalyticViewModel()
            {
                RelationTable = reporterTypesAnalytics
            };

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
