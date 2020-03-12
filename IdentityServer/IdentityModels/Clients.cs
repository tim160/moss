using IdentityServer3.Core.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace IdentityServer.IdentityModels
{
    public static class Clients
    {
        static string isUrl = ConfigurationManager.AppSettings.Get("IdentityServerUrl").TrimEnd('/');
        static string siteUrl = ConfigurationManager.AppSettings.Get("SiteUrl").TrimEnd('/');
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
            new Client
            {
                Enabled = true,
                ClientName = "MVC Client",
                ClientId = "mvc",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    isUrl
                },

                AllowAccessToAllScopes = true
            },
            new Client
            {
                Enabled = true,
                ClientName = "EC Client",
                ClientId = "ec_client",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    siteUrl + "/Service/SignInGoogle"
                },

                AllowAccessToAllScopes = true
            },
            new Client
            {
                ClientName = "MVC Client (service communication)",
                ClientId = "mvc_service",
                Flow = Flows.ClientCredentials,

                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    "sampleApi"
                }
            }
        };
        }
    }
}