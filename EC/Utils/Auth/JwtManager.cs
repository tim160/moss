﻿using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using log4net;
using Microsoft.IdentityModel.Tokens;

namespace EC.Utils.Auth
{
	public static class JwtManager
	{
		private static readonly string _secret;
		private static readonly byte[] _symmetricKey;
		private static readonly int _defualtExpireTime;
		private static readonly ILog _logger = LogManager.GetLogger(typeof(JwtManager));

		static JwtManager()
		{
			_defualtExpireTime = int.Parse(ConfigurationManager.AppSettings["JwtManager.DefualtExpireTime"]);

			const string settingName = "JwtManager.Secret";
			_secret = ConfigurationManager.AppSettings[settingName];
			if (string.IsNullOrWhiteSpace(_secret))
			{
				throw new ApplicationException($"The setting '{settingName}' is not set.");
			}

			_symmetricKey = Encoding.UTF8.GetBytes(_secret);
		}

		public static string GenerateToken(string username, int expireTime = 0)
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = GetIdentity(username),
				Expires = DateTime.UtcNow.AddHours(expireTime == 0 ? _defualtExpireTime : expireTime),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(_symmetricKey),
					SecurityAlgorithms.HmacSha256Signature
					)
			};

			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public static ClaimsPrincipal GetPrincipal(string token)
		{
			try
			{
				JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

				if (!(tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken))
				{
					return null;
				}

				TokenValidationParameters validationParameters = new TokenValidationParameters()
				{
					RequireExpirationTime = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(_symmetricKey)
				};

				return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
			}
			catch (Exception exception)
			{
				_logger.Error("Failed to get principal.", exception);
				return null;
			}
		}

		public static ClaimsIdentity GetIdentity(string username) => new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Name, username),
			//new Claim(ClaimTypes.NameIdentifier, "CompanyId")
		}, "JWT");
	}
}