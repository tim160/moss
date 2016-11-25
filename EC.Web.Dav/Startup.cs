using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EC.Web.Dav.Startup))]
namespace EC.Web.Dav
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
