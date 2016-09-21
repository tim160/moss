using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.ServiceModel;
using Castle.MicroKernel.Registration;
using Castle.Facilities.WcfIntegration;
using Castle.Core.Logging;
using MarineLMS.Core.Base;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Register components with IoC for the Util project.
    /// </summary>
    /// <remarks>

    /// </remarks>

    public class RegisterToIOC : IComponentInstaller
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
