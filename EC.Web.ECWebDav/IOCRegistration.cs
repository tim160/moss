using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using EC.Core.Common;


namespace EC.Web.ECWebDav
{
    public class IOCRegistration : IComponentInstaller
    {
        public void InstallComponents(Castle.Windsor.IWindsorContainer w)
        {
            IoCSetup.RegisterAll(w, Assembly.GetExecutingAssembly());
        }


        public void PostInstallComponents(Castle.Windsor.IWindsorContainer w)
        {
        }
    }
}
