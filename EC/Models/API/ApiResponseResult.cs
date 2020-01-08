using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace EC.Models.API
{
	public class ApiResponseResult : IHttpActionResult
	{
		private readonly MediaTypeFormatter _mediaTypeFormatter = new JsonMediaTypeFormatter();

		public ApiResponseResult(
			HttpStatusCode code,
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			Code = code;
			Message = message;
			Status = string.IsNullOrWhiteSpace(status) ? code.ToString() : status;
			ExceptionDetails = exceptionDetails;
		}

		public Guid ApiRequestReferenceId { get; } = Guid.NewGuid();
		public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
		public string Status { get; }
		public HttpStatusCode Code { get; }
		public string Message { get; }
		public string ExceptionDetails { get; }

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			HttpResponseMessage responseMessage = new HttpResponseMessage(Code)
			{
				Content = new ObjectContent<ApiResponseResult>(this, _mediaTypeFormatter),
				ReasonPhrase = Status
			};

			return Task.FromResult(responseMessage);
		}
	}

	public class ApiResponseResult<T> : ApiResponseResult
	{
		public ApiResponseResult(
			HttpStatusCode code,
			T data,
			string message = null,
			string status = null,
			string exceptionDetails = null)
			: base(code, message, status, exceptionDetails)
		{
			Data = data;
		}

		public T Data { get; }
	}
}