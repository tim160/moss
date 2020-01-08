using System.Net;

namespace EC.Common.Util.Models.API
{
	public class ApiUnauthorizedResult : ApiResponseResult
	{
		public ApiUnauthorizedResult(string message = null)
			: base(HttpStatusCode.Unauthorized, message)
		{
		}
	}
}