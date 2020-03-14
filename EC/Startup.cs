using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("EcConfig", typeof(EC.Startup))]
namespace EC
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
