using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
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
		public async Task<IHttpActionResult> Create(ModifyCompanyModel modifyCompanyModel)
		{
			if (modifyCompanyModel is null)
			{
				ModelState.AddModelError(nameof(modifyCompanyModel), "Company data required.");
			}

			if (!ModelState.IsValid)
			{
				return ApiBadRequest(ModelState);
			}

			int id = await _companyService
				.CreateOrUpdate(modifyCompanyModel)
				.ConfigureAwait(false);

			return ApiCreated(id);
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IHttpActionResult> Update(int id, ModifyCompanyModel modifyCompanyModel)
		{
			_logger.Debug($"id={id}");

			if (modifyCompanyModel is null)
			{
				ModelState.AddModelError(nameof(modifyCompanyModel), "Company data required.");
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
					.CreateOrUpdate(modifyCompanyModel, id)
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
