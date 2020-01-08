using System.Net;

namespace EC.Models.API
{
	public class ApiUnauthorizedResult : ApiResponseResult
	{
		public ApiUnauthorizedResult(string message = null)
			: base(HttpStatusCode.Unauthorized, message)
		{
		}
	}
}