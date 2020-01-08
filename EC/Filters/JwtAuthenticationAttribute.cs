using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using EC.Models.API;
using EC.Utils.Auth;

namespace EC.Filters
{
	public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		private const string _authorizationScheme = "Bearer";

		public bool AllowMultiple => false;

		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			HttpRequestMessage request = context.Request;
			AuthenticationHeaderValue authorization = request.Headers.Authorization;

			// If there are no credentials or if there are credentials but the filter does not recognize the authentication scheme, do nothing.
			if (authorization == null
				|| authorization.Scheme != _authorizationScheme)
			{
				return;
			}

			string token = authorization.Parameter;
			if (string.IsNullOrWhiteSpace(token))
			{
				context.ErrorResult = new ApiUnauthorizedResult("Missing token.");
				return;
			}

			IPrincipal principal = await AuthenticateJwtAsync(token);
			if (principal == null)
			{
				context.ErrorResult = new ApiUnauthorizedResult("Invalid token.");
			}
			else
			{
				context.Principal = principal;
			}
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			AuthenticationHeaderValue challenge = new AuthenticationHeaderValue(_authorizationScheme);
			context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);

			return Task.FromResult<object>(null);
		}

		protected Task<IPrincipal> AuthenticateJwtAsync(string token)
		{
			if (ValidateToken(token, out string username))
			{
				ClaimsIdentity identity = JwtManager.GetIdentity(username);
				IPrincipal user = new ClaimsPrincipal(identity);

				return Task.FromResult(user);
			}

			return Task.FromResult<IPrincipal>(null);
		}

		private static bool ValidateToken(string token, out string username)
		{
			username = null;

			ClaimsPrincipal simplePrinciple = JwtManager.GetPrincipal(token);
			ClaimsIdentity identity = simplePrinciple?.Identity as ClaimsIdentity;

			if (identity == null
				|| !identity.IsAuthenticated)
			{
				return false;
			}

			Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
			username = usernameClaim?.Value;

			if (string.IsNullOrWhiteSpace(username))
			{
				return false;
			}

			// TODO: More validate to check whether username exists in system

			return true;
		}
	}
}