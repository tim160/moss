using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EC.Common.Util.Filters;
using EC.Models.Database;

namespace EC.Controllers.API.v1
{
	[RoutePrefix("api/v1/companies")]
	[JwtAuthentication]
	[Authorize]
	public class CompanyController : BaseApiController
	{
		[HttpGet]
		[Route]
		public async Task<IHttpActionResult> GetList()
		{
			if (false)
			{
				return BadRequest();
			}

			//if (!CheckCredentials())
			////if (await CheckCredentials())
			//{
			//	return StatusCode(HttpStatusCode.Unauthorized);
			//}

			List<company> companies = await DB.company
				.Take(10)
				.ToListAsync()
				.ConfigureAwait(false);
			return ApiOk(companies);
		}

		[HttpPost]
		[Route("new")]
		public async Task<IHttpActionResult> Create(/*NewCompanyDto newCompanyDto*/)
		{
			//ResponseModel response = new ResponseModel();

			//if (false)
			//{
			//	return BadRequest();
			//}

			//if (!CheckCredentials())
			////if (await CheckCredentials())
			//{
			//	//response.Code = HttpStatusCode.Unauthorized;
			//	//response.Status = HttpStatusCode.Unauthorized.ToString();
			//	//return response;

			//	return new ApiResponseResult();

			//	//return StatusCode(HttpStatusCode.Unauthorized);
			//}

			//response.Code = HttpStatusCode.Created;
			//response.Status = HttpStatusCode.Created.ToString();
			//return response;

			return ApiCreated();
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IHttpActionResult> Update(int id/*, UpdateCompanyDto updateCompanyDto*/)
		{
			//if (false)
			//{
			//	return BadRequest();
			//}

			//if (!CheckCredentials())
			////if (await CheckCredentials())
			//{
			//	return StatusCode(HttpStatusCode.Unauthorized);
			//}

			return ApiOk();
		}
	}
}
