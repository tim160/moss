using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EC.Models.API.v1.Token;
using EC.Utils.Auth;

namespace EC.Controllers.API.v1
{
	[RoutePrefix("api/v1/token")]
	public class TokenController : BaseApiController
	{
		[AllowAnonymous]
		[HttpPost]
		[Route]
		public async Task<IHttpActionResult> Get([FromBody]TokenRequestModel tokenRequest)
		{
			if (CheckCompany(tokenRequest.CompanyId, tokenRequest.SecretKey))
			{
				return ApiOk<string>(JwtManager.GenerateToken(tokenRequest.CompanyId));
			}

			throw new HttpResponseException(HttpStatusCode.Unauthorized);
		}

		private bool CheckCompany(string companyId, string secretKey)
		{
			// TODO: should check in the database.
			return true;
		}
	}
}
