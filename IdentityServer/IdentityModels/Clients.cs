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
                ClientId = "6e20c12e-c3f5-4694-90e3-034c00e8aef4",
                Flow = Flows.ClientCredentials,

                ClientSecrets = new List<Secret>
                {
                    new Secret("QeThWmZq4t6w9z$C&F)J@NcRfUjXn2r5u8x/A%D*G-KaPdSgVkYp3s6v9y$B&E(H".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    "thinkhr"
                }
            }
        };
        }
    }
}