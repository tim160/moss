using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace EC.Common.Util.Models.API
{
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
          else _modelState = modelState ;
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
}
