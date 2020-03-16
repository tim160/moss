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
                ClientId = "e3c0cb7f-6c8c-47ae-a39a-1b32023308ac",
                Flow = Flows.ClientCredentials,

                ClientSecrets = new List<Secret>
                {
                    new Secret("hWmZq4t7w9z$C&F)J@NcRfUjXn2r5u8x/A%D*G-KaPdSgVkYp3s6v9y$B&E(H+MbQeThWmZq4t7w!z%C*F-J@NcRfUjXn2r5u8x/".Sha256())
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