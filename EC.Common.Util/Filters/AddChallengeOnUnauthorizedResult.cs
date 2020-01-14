using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace EC.Common.Util.Filters
{
	public class AddChallengeOnUnauthorizedResult : IHttpActionResult
	{
		public AuthenticationHeaderValue _challenge;
		public IHttpActionResult _innerResult;

		public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
		{
			_challenge = challenge ?? throw new ArgumentNullException(nameof(challenge));
			_innerResult = innerResult ?? throw new ArgumentNullException(nameof(innerResult));
		}

		public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			HttpResponseMessage response = await _innerResult.ExecuteAsync(cancellationToken);

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Only add one challenge per authentication scheme.
				if (response.Headers.WwwAuthenticate.All(headerValue => headerValue.Scheme != _challenge.Scheme))
				{
					response.Headers.WwwAuthenticate.Add(_challenge);
				}
			}

			return response;
		}
	}
}