using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using EC.Models;
using EC.Models.Database;
using Newtonsoft.Json;

namespace TestApi.Controllers
{
	public class BaseApiController : ApiController
	{
		private ECEntities db = null;
		internal ECEntities DB
		{
			get
			{
				if (db == null)
				{
					db = new ECEntities();
					db.Configuration.ProxyCreationEnabled = false;
					db.Configuration.LazyLoadingEnabled = false;
				}

				return db;
			}
		}

		internal HttpResponseMessage ResponseObject2Json(object obj)
		{
			var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
				new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					PreserveReferencesHandling = PreserveReferencesHandling.None
				});

			return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
		}

		public EmailNotificationModel emailNotificationModel = new EmailNotificationModel();

		#region Response results

		protected ApiResponseResult ApiOk(
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult(HttpStatusCode.OK, message, status, exceptionDetails);
		}

		protected ApiResponseResult<T> ApiOk<T>(
			T data,
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult<T>(HttpStatusCode.OK, data, message, status, exceptionDetails);
		}

		protected ApiResponseResult<T> ApiCreated<T>(
			T data,
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult<T>(HttpStatusCode.Created, data, message, status, exceptionDetails);
		}

		protected ApiBadRequestResult ApiBadRequest(string message) => new ApiBadRequestResult(message);

		protected ApiBadRequestResult ApiBadRequest(ModelStateDictionary modelState) => new ApiBadRequestResult(modelState, this);

		protected ApiResponseResult ApiNotFound(string message = null) => new ApiResponseResult(HttpStatusCode.NotFound, message);

		#endregion
	}

    public class ApiBadRequestResult : ApiResponseResult<Dictionary<string, string[]>>, IHttpActionResult
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApiController _controller;

        public ApiBadRequestResult(string message = null)
            : base(HttpStatusCode.BadRequest, null, message)
        {
        }

        public ApiBadRequestResult(ModelStateDictionary modelState, ApiController controller)
            : base(HttpStatusCode.BadRequest, modelState
                .Where(item => item.Value.Errors.Any())
                .ToDictionary(
                    item => item.Key,
                    item => item.Value.Errors.Select(e => e.ErrorMessage).ToArray()))
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));
            else _modelState = modelState;
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            else _controller = controller;
        }

        async Task<HttpResponseMessage> IHttpActionResult.ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage;

            if (_modelState == null)
            {
                responseMessage = await base.ExecuteAsync(cancellationToken);
            }
            else
            {
                InvalidModelStateResult result = new InvalidModelStateResult(_modelState, _controller);
                responseMessage = await result.ExecuteAsync(cancellationToken);
                responseMessage.Content = new ObjectContent<ApiBadRequestResult>(this, _mediaTypeFormatter);
            }

            return await Task.FromResult(responseMessage);
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

    public class ApiResponseResult : IHttpActionResult
    {
        protected readonly MediaTypeFormatter _mediaTypeFormatter = new JsonMediaTypeFormatter();

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
}
