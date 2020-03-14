
using IdentityServer3.Core.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.Code;
using IdentityServer.IdentityModels;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(IdentityServer.Startup))]
namespace IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var isUrl = ConfigurationManager.AppSettings.Get("IdentityServerUrl").TrimEnd('/');
            var siteUrl = ConfigurationManager.AppSettings.Get("SiteUrl").TrimEnd('/');

            IdentityModelEventSource.ShowPII = true;

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),
                    
                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnableLocalLogin = false
                    }
                });
                
            });
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = isUrl + "/identity",
                ClientId = "mvc",
                Scope = "openid profile roles sampleApi",
                RedirectUri = isUrl,
                ResponseType = "id_token token",

                SignInAsAuthenticationType = "Cookies"
            });
            app.UseResourceAuthorization(new AuthorizationManager());
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "313117514498-o0qv7m84jc623mr13p9h04thi34vqbap.apps.googleusercontent.com",
                ClientSecret = "G5I0n74Wxk_Apm24sWBMdq8d"
            });
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        X509Certificate2 LoadCertificate()
        {
            var certPath = $"{ AppDomain.CurrentDomain.BaseDirectory }\\localhost.pfx";
            return new X509Certificate2(certPath, "12345678");
        }
    }
}