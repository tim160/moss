using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Common.Interfaces;
using System.Web;
using System.Security.Principal;


namespace EC.Web.ECWebDav
{
    class CustomAuthenticationModule : ECBasic, IHttpModule
    {
        /// <summary>
        /// This is called when the HTTP module is loaded, and allows the module to hook into
        /// IIS request pipeline events.
        /// </summary>

        public void Init(HttpApplication context)
        {
            this.LogInfo("CustomAuthenticationModule.Init() called");

            context.AuthenticateRequest += OnAuthenticateRequest;
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
            var appSettings = this.GetContainer().Resolve<IAppSettings>();
            if (appSettings.WebDavEnableDebugInfoOutput)
            {
                context.PreSendRequestContent += PreSendRequestContent;
                context.BeginRequest += BeginRequest;
            }
        }

        public void BeginRequest(object sender, EventArgs e)
        {
            // This is only called if the WebDav debug info is enabled in web.config.
            string logPrefixBeginRequest = this.LogPrefix + "BeginRequest()";
            this.LogDebug(string.Format("{0}: request Url {1}", logPrefixBeginRequest, HttpContext.Current.Request.RawUrl));
        }

        public void PreSendRequestContent(object sender, EventArgs e)
        {
            // This is only called if the WebDav debug info is enabled in web.config.
            string logPrefixRequestCompleted = this.LogPrefix + "RequestCompleted()";

            HttpApplication app = (HttpApplication)sender;

            this.LogDebug(string.Format("{0}: request Url {1}", logPrefixRequestCompleted, HttpContext.Current.Request.RawUrl));
            if (app.Context != null)
            {
                if (app.Context.User != null && app.Context.User.Identity != null)
                {
                    this.LogInfo(string.Format("{0}: User {1} - IsAuthenticated='{2}', StatusCode={3}.{4}",
                        logPrefixRequestCompleted, app.Context.User.Identity.Name, app.Context.User.Identity.IsAuthenticated, app.Context.Response.StatusCode, app.Context.Response.SubStatusCode));

                }
                else
                {
                    this.LogError(string.Format("{0}: No user has been set - StatusCode={1}.{2}.",
                        logPrefixRequestCompleted, app.Context.Response.StatusCode, app.Context.Response.SubStatusCode));
                }
            }
        }

        /// <summary>
        /// Authenticate user and check capability for the virtual directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void OnAuthenticateRequest(object sender, EventArgs e)
        {
            string logPrefixOnAuthReq = this.LogPrefix + "OnAuthenticationRequest()";
            HttpApplication app = (HttpApplication)sender;
            try
            {
                string authHeader = app.Context.Request.Headers["Authorization"];
                authHeader = authHeader.TrimOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    // Authorization header must be present...
                    this.LogDebug(string.Format("{0}: No basic authorization header found. Request basic authentication with 401 error and WWW-Authenticate header.", logPrefixOnAuthReq));
                    this.AddAuthenticateHeader(app);
                    app.CompleteRequest();
                    return;
                }

                if (authHeader.IndexOf("Basic", 0) != 0)
                {
                    // We don't understand this header if it doesn't start with 'Basic'...request basic authentication..
                    this.LogInfo(string.Format("{0}: No basic authentication. Request basic authentication with 401 error and WWW-Authenticate header.", logPrefixOnAuthReq));
                    this.AddAuthenticateHeader(app);
                    app.CompleteRequest();
                    return;
                }

                string encodedCredentials = authHeader.Substring(6);
                byte[] decodedBytes = Convert.FromBase64String(encodedCredentials);
                string decodedCredentials = new ASCIIEncoding().GetString(decodedBytes);

                string[] userNameAndPwd = decodedCredentials.Split(new char[] { ':' });
                string userName = userNameAndPwd[0];
                string password = userNameAndPwd[1];

                // Decode path from url...
                string path = this.GetPathFromRequest(app.Context.Request);

                if (String.IsNullOrWhiteSpace(path))
                {
                    // Path is null or empty - deny access...
                    this.AccessDenied(app.Response);
                    app.CompleteRequest();
                    this.LogInfo(string.Format("{0}: User '{1}' denied for Path = '{2}'.", logPrefixOnAuthReq, userName, "Null or Empty"));
                    return;
                }

                // Do authentication...
                if (this.AuthenticateUser(userName, password) && this.CheckCapabilityForPathAndUser(path))
                {
                    // The user doesn't have to be set if Anonymous authentication is enabled...
                    app.Context.User = new GenericPrincipal(new GenericIdentity(userName, "CustomBasic"), null);

                    this.LogInfo(string.Format("{0}: User '{1}' authenticated for Path = '{2}', Identity.IsAuthenticated = {3}",
                    logPrefixOnAuthReq, userName, path, app.Context.User.Identity.IsAuthenticated));

                    return;
                }
                else
                {
                    // Invalid credentials - deny access...
                    this.AccessDenied(app.Response);
                    app.CompleteRequest();
                    this.LogInfo(string.Format("{0}: User '{1}' denied for Path = '{2}'.", logPrefixOnAuthReq, userName, path));
                    return;
                }
            }
            catch (Exception ex)
            {
                this.LogError(string.Format("{0} - error: {1}", logPrefixOnAuthReq, ex.ToString()));
                this.AccessDenied(app.Response);
                app.CompleteRequest();
            }
        }

        public void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            string logPrefixOnPreSend = this.LogPrefix + "OnPreSendRequestHeaders()";

            string msg = string.Format("{0}: StatusCode={1}.{2}", logPrefixOnPreSend, app.Context.Response.StatusCode, app.Context.Response.SubStatusCode);
            if ((app.Context.Response.StatusCode == 401) && (app.Context.Response.SubStatusCode == 0))
            {
                // Log 401.0 to info level because it's normal behaviour...
                this.LogInfo(msg);
            }
            else
            {
                this.LogWarn(msg);
            }
        }

        /// <summary>
        /// Deny access (code 401) and add WWW-Authenticate header to response.
        /// Also remove all other WWW-Authenticate header.
        /// </summary>
        /// <param name="app">current application.</param>

        private void AddAuthenticateHeader(HttpApplication app)
        {
            app.Response.StatusCode = 401;
            app.Response.SubStatusCode = 0;
            app.Response.Write("401 Request authentication");

            app.Response.Headers.Remove("WWW-Authenticate");

            // Add single WWw-Authenticate header...
            string val = "Basic Realm=\"Marine LMS\"";
            app.Response.AppendHeader("WWW-Authenticate", val);

        }

        /// <summary>
        /// If user has been denied access, return a 403 error (e.g. in case of a wrong password).
        /// </summary>
        /// <param name="app"></param>

        private void AccessDenied(HttpResponse response)
        {
            response.StatusCode = 403;
            response.SubStatusCode = 0;
            response.Write("403 Access denied");
        }

        /// <summary>
        /// Constructor to set log prefix.
        /// </summary>

        public CustomAuthenticationModule()
            : base("EC.CustomAuthenticationModule.")
        {
        }
    }
}
