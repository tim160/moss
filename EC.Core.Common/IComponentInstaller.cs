using Castle.Windsor;
using Castle.Core;

namespace EC.Core.Common
{
    /// <summary>
    /// This interface is used to mark classes that will be called to participate in 
    /// initialization of the IoC container.
    /// </summary>
    /// <remarks>
    /// Util.IOC.Setup.Run processes loaded assemblies and sets up a hook that is
    /// called when any new assemblies are loaded so that we can inspect each
    /// assembly to see if it exposes an implementation of IComponentInstaller. 
    /// For each such implementation it finds (implementation == class), it creates an
    /// instance of that class and calls its InstallComponents() and PostInstallComponents() methods. The intent is
    /// that each "module" will have an IComponentInstaller which knows which components
    /// should be registered as well as any specific registration options (such as OnCreate
    /// delegates).
    /// </remarks>
    
    public interface IComponentInstaller
    {
        /// <summary>
        /// Only register components which don't need w.Resolve()!! 
        /// If you need w.Resolve(), put it in <c>PostInstallComponents</c>.
        /// </summary>
        /// <param name="w"></param>

        void InstallComponents(IWindsorContainer w);

        /// <summary>
        /// This is executed after all InstallComponents are executed during assembly load.
        /// Use this to execute code which needs to resolve an IOC object from another assembly and which might not been loaded during 
        /// the InstallComponents call. 
        /// This is also executed after a single assembly is loaded (in case an assembly is loaded later on).
        /// </summary>
        /// <param name="w"></param>

        void PostInstallComponents(IWindsorContainer w);
    }
}