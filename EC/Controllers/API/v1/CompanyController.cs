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
using log4net;

namespace EC.Controllers.API.v1
{
	[RoutePrefix("api/v1/companies")]
	[JwtAuthentication]
	[Authorize]
	public class CompanyController : BaseApiController
	{
		private readonly CompanyService _companyService;
		protected readonly ILog _logger;

		public CompanyController()
		{
			_logger = LogManager.GetLogger(GetType());
			_companyService = new CompanyService();
		}

		[HttpGet]
		[Route]
		[ResponseType(typeof(PagedList<CompanyModel>))]
		public async Task<IHttpActionResult> GetList(int page = 1, int pageSize = 10)
		{
			_logger.Debug($"page={page}; pageSize={pageSize}");

			if (!ModelState.IsValid)
			{
				return ApiBadRequest(ModelState);
			}

			PagedList<CompanyModel> result = await _companyService
				.GetPagedAsync(page, pageSize)
				.ConfigureAwait(false);
			return ApiOk(result);
		}

		[HttpPost]
		[Route("new")]
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

		[HttpPut]
		[Route("{id}")]
		public async Task<IHttpActionResult> Update(int id, UpdateCompanyModel updateCompanyModel)
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
        [Route("externalcompany/{id}")]
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


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
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
        [Route("externaldelete/{id}")]
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
    }
}
