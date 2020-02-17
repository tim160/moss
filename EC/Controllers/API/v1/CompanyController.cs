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


namespace EC.Controllers.API.v1
{
	[RoutePrefix("api/v1/companies")]
	[JwtAuthentication]
	[Authorize]
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
		public async Task<IHttpActionResult> GetCompany()
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
      public int Name { get; set; }
      public int Quantity { get; set; }

      public decimal Percentage { get; set; }

    }


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
