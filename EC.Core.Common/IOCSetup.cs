using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.Core;
using Castle.Core.Logging;
////timusing Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
////timusing Castle.Facilities.WcfIntegration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EC.Common.Base;
using EC.Common.Interfaces;
using System.Threading.Tasks;
using EC.Errors;
using log4net;
using log4net.Appender;

namespace EC.Core.Common
{
    /// <summary>
    /// This class is just a few global functions which are used to
    /// initialize/finalize the IoC container.
    /// </summary>
    
    public static class IoCSetup
    {
        /// <summary>
        /// Initialize the Castle/Windsor IoC system. This includes setting up common facilities
        /// as well as registering components from all assemblies in the directory the application
        /// is executing from.
        /// </summary>
        /// <remarks>
        /// Originally this was called only once on application startup and a single container 
        /// was used for the lifetime of the application. However, due to the way EF migration
        /// works, it became necessary to allocate new containers that would only be used in the
        /// context of EF migrations. These containers are assumed to be temporary in existence
        /// and it is also assumed that they should not have any background services
        /// associated with them. Thus, the suppressStartable parameter is interpreted to mean
        /// the caller is asking for a temporary-use container which has the effect
        /// both of suppressing the starting of IStartable components, but also of not storing
        /// the new container in the static member variable which is used to track the
        /// applications main container.
        /// </remarks>
        /// <remarks>
        /// RegisterCommonFacilities() must be called immediately on the newly created container
        /// because it sets up things like logging that we use every else. 
        /// </remarks>
        /// <param name="startMsg">A startup message to place in the log</param>
        /// <returns>the initialized Windsor container</returns>
        
        public static IWindsorContainer Run(string startMsg, string containerName, bool suppressStartable=false)
        {
            WindowsEventLog.AddEvent(string.Format("IOCSetup.Run has been called for container name {0} (suppressStartable={1})", containerName, suppressStartable), WindowsEventLog.Info);
            ContainerName = containerName;
            var newContainer = new WindsorContainer();
            newContainer.Kernel.ComponentCreated += Kernel_ComponentCreated;
            RegisterCommonFacilities(newContainer, suppressStartable);
            DBC.Assert(MainContainer == null, String.Format("IoCSetup::Run[{0}] - Duplicate main container", containerName));
            MainContainer = newContainer;
            Logger = newContainer.Resolve<ILogger>();
            Logger.Info(startMsg);
            DeactivatePropertyInjection(newContainer);
            SetupAssemblyLoadHook(); 
            IList<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            loadedAssemblies.ForEach(a => LogAssemblyName(a, "initial"));
            RegisterAssembliesForIOC(newContainer, loadedAssemblies);
            return newContainer;
        }

        /// <summary>
        /// Provide access to the container for those rare situations in which it is not
        /// possible to get the kernel injected. 
        /// </summary>
        /// <remarks>
        /// This entry point was created to handle the way that EF does migrations. It appears
        /// as though EF loads all of the applications assemblies and then directly creates a
        /// DB context. Normally, all of the DBContexts in the application are created via
        /// IoC and rely on injected dependencies. However, these directly created DB contexts
        /// need the IoC container in order to manually resolve the dependencies. 
        /// </remarks>
        /// <returns>the IoC container for the application</returns>
        
        public static IWindsorContainer GetContainer(bool suppressStartable=true)
        {
            if (MainContainer == null) 
            { 
                Run("Creating main container in IoCSetup::GetContainer", "GetContainer", suppressStartable); 
            }
            
            return MainContainer;
        }

        /// <summary>
        /// Shut down the IoC container and exit the application/process. This will also cause all 
        /// component instances to be released and all startable components to be stopped. This call 
        /// should never return because it causes the process to exit with <paramref name="exitCode"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// We call Environment.Exit() here because there are threads running which we don't have
        /// direct control over and which don't seem to get stopped when we dispose the container.
        /// For example, we have observed AsynAppender threads continuing to run after dispose on
        /// the container has returned. This probably shouldn't be the case, but it would be time
        /// consuming to track down why. If we don't exit here, then the process may not exit due
        /// to these running threads.
        /// </para>
        /// <para>
        /// We have to call Exit() in a separate thread because the SCM complains if you call
        /// Exit() on its thread (and when you stop a service via the SCM it is the SCM's thread 
        /// that is calling into this code).
        /// </para>
        /// <para>
        /// There is an interesting deadlock scenario that ShutdownStoppables() needs to avoid.
        /// If the thread calling Shutdown() is the background thread for an object bearing the 
        /// IBackgroundThread interface, and it tries to call Stop() on the instance for which it
        /// is a background thread, then a deadlock results (because it is trying to stop itself).
        /// So ShutdownStoppeables() needs to avoid this scenario.
        /// </para>
        /// <para>
        /// We should try to stop all incoming requests, and ensure that all in-process requests
        /// are complete prior to calling Dispose on the container. This is not implemented currently.
        /// </para>
        /// </remarks>
        /// <param name="shutdownMsg">The shutdown message to log before exiting.</param>
        /// <param name="exitCode">Exit code to stop the system (any code other than 0 for any error occurred).</param>

        public static void ShutdownAndExit(string shutdownMsg, int exitCode)
        {
            try
            {
                ShutdownThreadsAndReleaseContainer(shutdownMsg);

                if (exitCode != 0)
                {
                    string msg = string.Format("IoCSetup::Shutdown[{0}] - {1} {2}", ContainerName, shutdownMsg, string.Format("Stack = {0}", DebugUtils.GetStackTrace()));
                    WindowsEventLog.AddEvent(msg, WindowsEventLog.FatalError);
                }
            }
            catch (Exception ex) 
            {
                var msg = String.Format("IoCSetup::Shutdown[{0}]: Unexpected exception Msg={1} Ex = {2}", ContainerName, shutdownMsg, ex);
                WindowsEventLog.AddEvent(msg, WindowsEventLog.FatalError);
            }

            Task.Factory.StartNew(() => Environment.Exit(exitCode));
        }

        /// <summary>
        /// Shut down the application. This involves shutting down any background threads and then releasing the 
        /// IoC container that was allocated in IoCSetup.Run().
        /// </summary>
        
        public static void ShutdownThreadsAndReleaseContainer(string shutdownMsg)
        {
            if (ShuttingDown && Logger != null) { Logger.InfoFormat("IoCSetup::ShutdownThreadsAndReleaseContainer[{0}] - Re-entrant call. Stack = {1}", ContainerName, DebugUtils.GetStackTrace()); return; }
            ShuttingDown = true;

            if (Logger != null)
            {
                string msg = string.Format("IoCSetup::Shutdown[{0}] - {1}", ContainerName, shutdownMsg);
                Logger.Info(msg);
            }
            
            ShutdownBackgroundThreads();
            MainContainer.Dispose();
            WindowsEventLog.AddEvent(string.Format("IoCSetup::Shutdown[{0}] threads stopped and container disposed.", ContainerName), WindowsEventLog.Info);
        }

        /// <summary>
        /// Register all classes in the given assembly marked with any sort of IoC related
        /// attributes.
        /// </summary>
        /// <remarks>
        /// This code deals with the various IoC related attributes that we use to mark how a class
        /// should be registered with the IoC. There are a number of conventions that we have 
        /// adopted with regard to IoC that are captured in this code: (1) Only classes with IoC
        /// attributes will be registered with the container. (2) Implementation classes must be assignable
        /// to the types they are being registered as
        /// </remarks>
        /// <param name="w">the Windsor container</param>
        /// <param name="a">the assembly</param>

        public static void RegisterAll(IWindsorContainer w, Assembly a)
        {
            try
            {
                var typesInAssembly = a.GetTypes().Where(t => t.IsClass && HasIoCAttribute(t));

                foreach (var implementationClass in typesInAssembly)
                {
                    var registration = GetRegistrationFor(implementationClass);
                    DBC.NonNull(registration, String.Format("IoCSetup [{0}] - No interface to register for class {1}", ContainerName, implementationClass.Name));
                    ValidateRegistrations(registration, implementationClass);
                    var component = Component.For(registration.RegistrationTypes) as ComponentRegistration<object>;
                    DBC.NonNull(component, string.Format("IoCSetup::RegisterAll[{0}] - Could not cast to ComponentRegistration", ContainerName));
                    component = component.ImplementedBy(implementationClass);
                    if (IsSingletonType(implementationClass)) { component = component.LifestyleSingleton(); }
                    if (IsTransientType(implementationClass)) { component = component.LifestyleTransient(); }
                    ////tim         if (IsPerWCFRequestType(implementationClass)) { component = component.LifestylePerWcfOperation(); }
                    if (IsRegisteredAsFactory(implementationClass)) { RegisterFactoryForClass(w, implementationClass); }
                    if (registration.RegistrationName != null) { component = component.Named(registration.RegistrationName); }
                    component = SetupNamedDependencies(component, implementationClass);
                    w.Register(component);
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("IoCSetup::RegisterAll[{0}] - Unexpected Exception in RegisterAll - {1}", ContainerName, ex.ToString());
                Logger.Fatal(msg, ex);
                DBC.Assert(false, msg);
            }
        }

        /// <summary>
        /// Start background threads for any objects registered with the IBackgroundThread interface.
        /// </summary>
        /// <param name="kernel">the IoC container to use</param>
        
        public static void StartBackgroundThreads(IKernel kernel)
        {
            var startables = kernel.ResolveAll<IHaveBackgroundThread>();
            startables.ForEach(s => s.StartBackgroundThread());
        }

        /// <summary>
        /// Start background threads for any shared model objects that bear the 
        /// IBoundBackgroundThread interface (which allows each DB context to have its
        /// own instance of that model object, each of which has its own background
        /// thread).
        /// </summary>
        /// <typeparam name="U">unit of work interface for DB context model objects are bound to</typeparam>
        /// <param name="kernel">the IoC container to user</param>
        
        public static void StartBackgroundThreadsForShared<U>(IKernel kernel)
        {
            var sharedStartables = kernel.ResolveAll<IHaveBoundBackgroundThread<U>>();
            sharedStartables.ForEach(s => s.StartBackgroundThread());
        }

        /// <summary>
        /// Allow the user to force a reference to a type in order to force the loading of
        /// it's assembly. The caller could just reference the type directly, so this
        /// is really intended as a way to document the need to force assembly loading.
        /// </summary>
        /// <param name="t">type in assembly to be loaded</param>

        public static void ForceLoad(Type t)
        {
        }

        /// <summary>
        /// The name used to register the stats log component with.
        /// </summary>

        public static string StatsLogName = "StatsLogger";
        
        /// <summary>
        /// Register common facilities.
        /// 
        /// <para>
        /// The LoggingFacility sets up resolution for the ILogger and ILoggerFactory interfaces
        /// and we configure them to use Log4Net.
        /// </para>
        /// <para>
        /// The WcfFacility close timeout is set to 500ms to give the WCF channel a chance to close properly.
        /// If the close timeout is set to TimeSpan.Zero - both sides - web and service generate a lot of exceptions inside the 
        /// WCF service (they don't surface!) every time a service endpoint is closed.
        /// </para>
        /// <para>
        /// The typed factory facility makes it easy to set up factories for transient objects without actually
        /// having to have any code for the factory implementations.
        /// </para>
        /// <para>
        /// Stats logging is not implemented as a facility, but we want it done very early so that
        /// it is accessible by virtually all other aspects of the system. 
        /// </para>
        /// </summary>
        /// <param name="container">container to set up</param>
        /// <param name="suppressStartable">whether the startable facility should be included</param>
        
        private static void RegisterCommonFacilities(WindsorContainer container, bool suppressStartable)
        {
            try
            {
                ////tim      container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4netConfig.xml"));
                ////tim       container.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.FromMilliseconds(500));
                container.AddFacility<TypedFactoryFacility>();
            }
            catch (Exception ex)
            {
                var msg = String.Format("IOCSetup::RegisterCommonFacilities[{0}] - Unexpected exception - {1}", ContainerName, ex.ToString());
                ShutdownAndExit("Unexpected Exception - " + msg, 1);
            }
        }

        /// <summary>
        /// Locate and call into the IoC setup entry points in each of the given assemblies. An
        /// IoC setup entry point is any class that bears the IComponentInstaller interface.
        /// </summary>
        /// <param name="container">the container into which component definitions should go</param>

        private static void RegisterAssembliesForIOC(WindsorContainer container, IList<Assembly> assemblies)
        {
            IList<IComponentInstaller> allIocInstallers = GetIoCInstallersFromAssemblies(assemblies);
            ExecuteIoCInstallers(container, allIocInstallers);
            ExecuteIoCPostInstallers(container, allIocInstallers);
        }

        /// <summary>
        /// Get all types of IComponentInstaller from the list of <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">Assemblies to look for types of IComponentInstaller.</param>
        /// <returns>Return list of IComponentInstaller found in the assemblies.</returns>

        private static IList<IComponentInstaller> GetIoCInstallersFromAssemblies(IList<Assembly> assemblies)
        {
            List<IComponentInstaller> iocInstallers = new List<IComponentInstaller>();

            foreach (var a in assemblies)
            {
                Logger.DebugFormat("GetIoCInstallersFromAssemblies[{0}] processing {1}", ContainerName, a.FullName);
                if (IsDynamicAssembly(a)) { continue; }
                var types = GetInstallerTypesFromAssembly(a);
                if (types == null) { continue; }

                foreach (var t in types)
                {
                    try
                    {
                        Object o = Activator.CreateInstance(t);
                        IComponentInstaller installer = o as IComponentInstaller;
                        Logger.InfoFormat("IoCsetup::GetIoCInstallers[{0}] - Found IoC Installer [type = {1}] from assembly [{2}]", ContainerName, t.FullName, a.Location);
                        iocInstallers.Add(installer);
                    }
                    catch (Exception ex)
                    {
                        Logger.InfoFormat("IoCsetup::GetIoCInstallers[{0}] - Unexpected error [type = {1}] and assembly [{2}]: {3}", ContainerName, t.Name, a.Location, ex.ToString());
                    }
                }
            }

            return iocInstallers;
        }

        /// <summary>
        /// Execute the InstallComponents to register types for IOC.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="iocInstallers"></param>

        private static void ExecuteIoCInstallers(IWindsorContainer container, IList<IComponentInstaller> iocInstallers)
        {
            foreach (var installer in iocInstallers)
            {
                try
                {
                    installer.InstallComponents(container);
                    Logger.DebugFormat("IoCSetup::ExecuteIoCInstallers[{0}] - InstallComponents for {1}", ContainerName, installer.GetType().Assembly.GetName());
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("IoCSetup::ExecuteIoCInstallers[{0}] - Unexpected exception wile processing {1} - {2}: {3}", ContainerName, installer.GetType().Assembly.GetName(), ex.GetType().ToString(), ex.ToString());
                }
            }
        }

        /// <summary>
        /// Execute the PostInstallComponents to register types for IOC after all initial registrations have been finished.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="iocInstallers"></param>

        private static void ExecuteIoCPostInstallers(IWindsorContainer container, IList<IComponentInstaller> iocInstallers)
        {
            foreach (var installer in iocInstallers)
            {
                try
                {
                    installer.PostInstallComponents(container);
                    Logger.DebugFormat("IoCSetup::ExecutePostIoCInstallers[{0}] - PostInstallComponents for {1}", ContainerName, installer.GetType().Assembly.GetName());
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("IoCSetup::ExecutePostIoCInstallers[{0}] - Unexpected exception wile processing {1} - {2}: {3}", ContainerName, installer.GetType().Assembly.GetName(), ex.GetType().ToString(), ex.ToString());
                }
            }
        }

        // NOTE: these constants correspond to the appender names in the log4netConfig.xml files
        private const string Log4NetTraceLogAppenderName = "TraceLog";
        private const string Log4NetEventLogAppenderName = "Event";
        private const string Log4NetEmailLogAppenderName = "Email";
        private const string Log4NetDebuggerLogAppenderName = "Debugger";
        private const string Log4NetStatsLogAppenderName = "StatsLog";

        /// <summary>
        /// Configure the log4net logging infrastructure at runtime from app settings
        /// </summary>


        /// <summary>
        /// Configure a specific appender to use the specified logging level as the threshold
        /// </summary>
        /// <param name="appenderName">Appender name to use when indexing <paramref name="appenderMap"/></param>
        /// <param name="level">Level to set the appender threshold to</param>
        
        private static void ConfigureLog4NetAppender(
            IAppSettings settings,
            log4net.Repository.Hierarchy.Hierarchy hierarchy, 
            Dictionary<string, AppenderSkeleton> appenderMap, 
            string appenderName, 
            string level, 
            ILogger logger)
        {
            AppenderSkeleton appender = null;

            if (appenderMap.ContainsKey(appenderName))
            {
                appender = appenderMap[appenderName];
            }

            if (appender != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(level) == false)
                    {
                        appender.Threshold = hierarchy.LevelMap[level];
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("IoCSetup::ConfigureLog4NetAppender[{0}] - Unable to Set {1} Level: {2} - {3}", ContainerName, appenderName, level, ex.Message);
                }

                var fileAppender = appender as FileAppender;

                if (fileAppender != null)
                {
                    string path = null;

                    switch (appenderName)
                    {
                        case Log4NetTraceLogAppenderName:
                            path = settings.TraceLogPath;
                            break;
                        case Log4NetStatsLogAppenderName:
                            path = settings.StatsLogPath;
                            break;
                    }

                    try
                    {
                        if (string.IsNullOrWhiteSpace(path) == false)
                        {
                            fileAppender.File = path;

                            fileAppender.ActivateOptions();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("IoCSetup::ConfigureLog4NetAppender[{0}] - Unable to Set {1} File Path: {2} - {3}", ContainerName, appenderName, level, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Set up the stats logging infrastructure.
        /// </summary>
        /// <remarks>
        /// The stats log is a separately named log instance that is used only for
        /// recording statistical data. The name used for the Stats log must match 
        /// up with logger name used in the log4net.xml config file (in order to
        /// have the output for this log separated into its own file).
        /// </remarks>
        /// <param name="container">IoC container to register with</param>

        private static void SetupStatsLogging(WindsorContainer container)
        {
            try
            {
                var logFactory = container.Resolve<ILoggerFactory>();
                var statsLog = logFactory.Create("Stats");
                statsLog.InfoFormat("IoCSetup::SetupStatsLogging[{0}] - Stats Log Started", ContainerName); 
                container.Register(Component.For<ILogger>().Named(StatsLogName).Instance(statsLog));
            }         
            catch (Exception ex)
            {
                Logger.ErrorFormat("IoCSetup::SetupStatsLogging[{0}] - Unexpected exception - {1}", ContainerName, ex.Message);
            }
        }

        /// <summary>
        /// Check to see if the implementation classes being bound to registration types are compatible with 
        /// those types. For example, if an implementation class indicates that it inherits from IFoo and
        /// nothing else, but the implementation class is being registered for type IBar, that does not make sense
        /// because one could resolve IBar and get an object back that did not support that interface. This
        /// code checks to see if the implementation type can be assigned to the registration type. 
        /// <para>
        /// NOTE: The assignment check cannot be used when the registration type is an unbound generic because
        /// the IsAssignableFrom() call fails in that case. There is no easy way to check the consistency of
        /// the types in this case, so we just forgo checking in that case.
        /// </para>
        /// </summary>
        /// <param name="registration">the type be registered for the implementation</param>
        /// <param name="implementationClass">the implementation type</param>

        private static void ValidateRegistrations(RegisterAsType registration, Type implementationClass)
        {
            foreach (var registrationType in registration.RegistrationTypes)
            {
                if (registrationType.IsGenericTypeDefinition) { continue; }
                var msg = String.Format("IoCSetup::RegisterAll[{0}] - Cannot assign implementation class ({1}) to registration type ({2})", ContainerName, implementationClass.Name, registrationType.Name);
                DBC.Assert(registrationType.IsAssignableFrom(implementationClass), msg);
            }
        }

        /// <summary>
        /// Call stop on all IoC components that have the IBackgrounfThread interface.
        /// </summary>

        private static void ShutdownBackgroundThreads()
        {
            if (StoppableInstances.Count() == 0) { return; }
            var msg = StoppableInstances.Select(s => s.GetType().ToString()).Aggregate((c, a) => a + ", " + c);
            Logger.InfoFormat("IoCSetup::ShutdownStoppables - Stopping {0}.", msg);
            StoppableInstances.ForEach(s => StopIfNotCurrent(s));
        }

        /// <summary>
        /// Stops the background thread on an object as long as that background thread is
        /// not the current thread (because that way lies deadlock).
        /// </summary>

        private static void StopIfNotCurrent(IHaveBackgroundThread s)
        {
            if (s.GetBackgroundThread().IsCurrentThread()) { return; }
            s.StopBackgroundThread();
        }

        /// <summary>
        /// Register a handler in the app domain that will get called whenever an assembly is loaded.
        /// </summary>

        private static void SetupAssemblyLoadHook()
        {
            AppDomain d = AppDomain.CurrentDomain;
            d.AssemblyLoad += new AssemblyLoadEventHandler(IOCAssemblyLoadHandler);
        }

        // Event handler called when an assembly is loaded. We need to check and see whether is has
        // and IOC registration entry point (which is done in LoadAssemblyForIOC).

        private static void IOCAssemblyLoadHandler(object sender, AssemblyLoadEventArgs args)
        {
            DBC.NonNull(args.LoadedAssembly, "args.LoadedAssembly was null");
            LogAssemblyName(args.LoadedAssembly, "via hook");
            RegisterAssemblyForIOC(MainContainer, args.LoadedAssembly);
        }

        /// <summary>
        /// This method is called for every component that is created via Castle/Windsor.
        /// </summary>
        /// <remarks>
        /// Currently the only instances we're interested in are those bearing the IBackgroundThread interface,
        /// because we need to shut them down when the application terminates. The reason that we 
        /// need to record the stoppable instances, rather than just doing a ResovleAll, is that 
        /// ResolveAll can actually create instances of components, whereas we really only want to
        /// get those components which have been created elsewhere.
        /// </remarks>

        private static void Kernel_ComponentCreated(ComponentModel model, object instance)
        {
            if (instance is IHaveBackgroundThread) { StoppableInstances.Add((instance as IHaveBackgroundThread)); }
        }

        /// <summary>
        /// Deactivate automatic property injection in Castle Windsor container.
        /// </summary>
        /// <remarks>
        /// Code from: http://docs.castleproject.org/Windsor.How-properties-are-injected.ashx?HL=property,injection
        /// </remarks>

        private static void DeactivatePropertyInjection(WindsorContainer container)
        {
            try
            {
                var propInjector = container.Kernel
                   .ComponentModelBuilder
                   .Contributors
                   .OfType<Castle.MicroKernel.ModelBuilder.Inspectors.PropertiesDependenciesModelInspector>()
                   .Single();

                container.Kernel.ComponentModelBuilder.RemoveContributor(propInjector);
            }
            catch (Exception ex)
            {
                var msg = String.Format("IoCSetup::DeactivatePropertyInjection[{0}] - Unexpected exception {1}", ContainerName, ex.ToString());
                Logger.Error(msg, ex);
                ShutdownAndExit("Fatal Exception -" + msg, 1);
            }
        }

        /// <summary>
        /// Register a factory for the given implementaton class. This assumes that the class
        /// has been annotated with attributes indicating that a factory is required.
        /// </summary>
        /// <param name="w">windsor container</param>
        /// <param name="c">implementation class</param>

        private static void RegisterFactoryForClass(IWindsorContainer w, Type c)
        {
            var factoryType = GetFactoryTypeFromAttributes(c);
            w.Register(Component.For(factoryType).AsFactory().LifestyleTransient());
        }

        /// <summary>
        /// Check if the given assembly is a "dynamic" assembly (that is, an assembly that is generated
        /// on the fly by .NET).
        /// </summary>
        /// <remarks>
        /// The way we figure out if the assembly is dynamic is to try to access the Location property, which
        /// is not supported by dynamic assemblies.
        /// </remarks>
        /// <param name="a">the assembly to check</param>
        /// <returns>true if it is a dynamic assembly</returns>

        private static bool IsDynamicAssembly(Assembly a)
        {
            return a.IsDynamic;
        }

        /// <summary>
        /// Extract from an assembly the set of types that support the IComponentInstaller 
        /// interface. Normally there would only be one per assembly, but we don't enforce that.
        /// </summary>
        /// <param name="a">assembly to process</param>
        /// <returns>the list of component installer types; null indicates an error</returns>

        private static IEnumerable<Type> GetInstallerTypesFromAssembly(Assembly a)
        {
            Type componentInstallerType = typeof(IComponentInstaller);

            try
            {
                var types = a.GetTypes().Where(t => componentInstallerType.IsAssignableFrom(t) && t.IsClass);
                return types;
            }
            catch (ReflectionTypeLoadException r)
            {
                var msg = String.Format("IoCSetup::GetinstallerTypesFromAssembly[{0}] - Exception while getting types from assembly {1}", ContainerName, a.GetName());
                var loaderExceptionStrings = r.LoaderExceptions.Select(e => e.ToString());
                msg += "\n" + "LOADER EXCEPTIONS" + "\n";
                msg += loaderExceptionStrings.Aggregate((curr, next) => curr + "\nLoaderException: " + next);
                Logger.Error(msg, r);
                return null;
            }
        }

        /// <summary>
        /// Load out the assembly name/location. Dynamic assemblies don't have a location, but the only
        /// way to find that out is to try and access the location and catch the exception.
        /// </summary>
        /// <param name="a">assembly to log</param>
        
        private static void LogAssemblyName(Assembly a, string prefix)
        {
            if (IsDynamicAssembly(a))
            {
                Logger.DebugFormat("IoCSetup[{0},{1},App Domain Id:{2}] - Assembly [{3}] loaded (dynamic).", ContainerName, prefix, AppDomain.CurrentDomain.Id, a.GetName());
            }
            else
            {
                Logger.DebugFormat("IoCSetup[{0},{1},App Domain Id:{2}] - Assembly [{3}] loaded from {4}.", ContainerName, prefix, AppDomain.CurrentDomain.Id, a.GetName(), a.Location);
            }
        }

        /// <summary>
        /// Look for an IOC registration entry point in the given assembly, and call InstallComponents and PostInstallComponents.
        /// </summary>
        /// <remarks>
        /// This code looks for a class which inherits from IComponentInstaller. If it finds such
        /// a class, it instantiates an instance, and calls InstallComponents and PostInstallComponents methods, passing 
        /// it the Windsor container (which will then allow the method to register components with
        /// Windsor).
        /// </remarks>
        /// <param name="container">Container to register the components.</param>
        /// <param name="a">the assembly to look in</param>

        private static void RegisterAssemblyForIOC(WindsorContainer container, Assembly a)
        {
            IList<Assembly> assemblies = new List<Assembly>() { a };
            RegisterAssembliesForIOC(container, assemblies);
        }

        /// <summary>
        /// Return the set of types that a given type should be registered as in the IoC container. Intuitively,
        /// each type should only be registered as a single type (e.g. Foo will be registered as the implementation
        /// for IFoo). However, due to history, we actually register a given implementation class with
        /// all the interfaces it declares.
        /// </summary>
        /// <remarks>
        /// NOTE: If the given type is an interface, we return an empty set because we can't register
        /// an interface with the container without know which class implements it. Similarly, if the
        /// given type is a concrete class with no interfaces, and which does not have attributes
        /// forcing registration as a specific type, then there is nothing to register.
        /// </remarks>
        /// <param name="implementationClass">type to get registration types for</param>
        /// <returns>the set of types that the given type should be registered as</returns>
        
        private static RegisterAsType GetRegistrationFor(Type implementationClass)
        {
            if (implementationClass.IsInterface) { return null; }
            var explicitRegistration = GetRegistrationFromAttributes(implementationClass);
            if (explicitRegistration != null) { return explicitRegistration; }
            var allInterfaces = implementationClass.GetInterfaces();
            if (!allInterfaces.Any()) { return null; }
            DBC.Assert(allInterfaces.Count() <= 2, String.Format("Can't handle more than 2 implicit interface registrations ({0})", implementationClass.Name));
            if (allInterfaces.Count() == 1) { return new RegisterAsType(allInterfaces.ElementAt(0)); }
            if (allInterfaces.Count() == 2) { return new RegisterAsType(allInterfaces.ElementAt(0), allInterfaces.ElementAt(1)); }
            DBC.Assert(false, "Unreachable code");
            return null;
        }

        /// <summary>
        /// Extend the given component registration with service dependency information from attributes.
        /// </summary>

        private static ComponentRegistration<object> SetupNamedDependencies(ComponentRegistration<object> registration,Type c)
        {
            var attrs = c.GetCustomAttributes(typeof(DependsOn), true);
            var namedDependencyAttrs = attrs.Select(o => (DependsOn) o);
            namedDependencyAttrs.ForEach(d => registration = registration.DependsOn(ServiceOverride.ForKey(d.ServiceType).Eq(d.Name)));
            return registration;
        }

        /// <summary>
        /// Predicate indicate whether the given type has any IoC attributes attached to it.
        /// </summary>
        /// <param name="t">type to test</param>
        /// <returns>true if there are IoC attributes on the type</returns>
        
        private static bool HasIoCAttribute(Type t)
        {
            Object[] attrs = t.GetCustomAttributes(true);
            Type IoCMarkerInterface = typeof(IoCAttribute);
            return attrs.Any(a => (IoCMarkerInterface.IsAssignableFrom(a.GetType())));        
        }

        /// <summary>
        /// Predicate indicating whether the give type is marked with the SingletonType attribute.
        /// </summary>
        /// <param name="t">type to check</param>
        /// <returns><c>true</c> if type has SingletonType attribute AND has the value <c>true</c></returns>
        
        private static bool IsSingletonType(Type t)
        {
            Object[] attrs = t.GetCustomAttributes(true);
            return attrs.Where(a => (a.GetType() == typeof(SingletonType)) && (((SingletonType)a).IsSingleton == true)).Count() >= 1;
        }

        /// <summary>
        /// Predicate indicating whether the give type is marked with the TransientType attribute.
        /// </summary>
        /// <param name="t">type to check</param>
        /// <returns><c>true</c> if type has TransientType attribute AND has the value <c>true</c></returns>
        
        private static bool IsTransientType(Type t)
        {
            Object[] attrs = t.GetCustomAttributes(true);
            return attrs.Where(a => (a.GetType() == typeof(TransientType)) && (((TransientType)a).IsTransient == true)).Count() >= 1;
        }

        /// <summary>
        /// Predicate indicating whether the give type is marked with the PerWCFRequestType attribute.
        /// </summary>
        /// <param name="t">type to check</param>
        /// <returns><c>true</c> if type has PerWCFRequestType attribute AND has the value <c>true</c></returns>

        private static bool IsPerWCFRequestType(Type t)
        {
            Object[] attrs = t.GetCustomAttributes(true);
            return attrs.Where(a => (a.GetType() == typeof(PerWCFRequestType)) && (((PerWCFRequestType)a).IsPerWCFRequest == true)).Count() >= 1;
        }

        /// <summary>
        /// Predicate indicating whether the given type is marked with the RegisterAsFactory attribute
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool IsRegisteredAsFactory(Type t)
        {
            Object[] attrs = t.GetCustomAttributes(true);
            return attrs.Where(a => (a.GetType() == typeof(RegisterFactoryFor))).Count() >= 1;
        }

        /// <summary>
        /// Get the IoC registration for the given type.
        /// </summary>
        /// <param name="t">Type to check</param>
        /// <returns>Return the type for the registration. Return <c>null</c> if no <c>RegisterAsType</c> attribute exists.</returns>

        private static RegisterAsType GetRegistrationFromAttributes(Type t)
        {
            var attrs = t.GetCustomAttributes(typeof(RegisterAsType), true);
            var registrationAttrs = attrs.Select(o => (RegisterAsType) o);
            DBC.Assert(registrationAttrs.Count() <= 1, "Multiple RegisterAsType attributes not supported");
            return registrationAttrs.FirstOrDefault();
        }

        /// <summary>
        /// Get the parameter to the RegisterFactoryFor attribute. This should be the
        /// one of the interface types of the class to which it is applied.
        /// </summary>

        private static Type GetFactoryTypeFromAttributes(Type t)
        {
            var attrs = t.GetCustomAttributes(typeof(RegisterFactoryFor), true);
            var registrationAttrs = attrs.Select(o => (RegisterFactoryFor)o);
            DBC.Assert(registrationAttrs.Count() <= 1, "Multiple RegisterAsFactory attributes not supported");
            var factoryAttribute = registrationAttrs.FirstOrDefault() as RegisterFactoryFor;
            return factoryAttribute == null ? null : factoryAttribute._factoryType;
        }
 
        // ================================== Member Variables ==================================

        private static ILogger Logger = null;
        private static WindsorContainer MainContainer = null;
        private static IList<IHaveBackgroundThread> StoppableInstances = new List<IHaveBackgroundThread>();
        private static string ContainerName = "Default";
        private static bool ShuttingDown = false;
    }

    /// <summary>
    /// Marker interface for all attributes used by the IoC code.
    /// </summary>
    
    public interface IoCAttribute { }
    
    /// <summary>
    /// Class attribute to indicate to IOC that there should only ever be one instance of this type of object.
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Class, Inherited=false)]

    public class SingletonType : System.Attribute, IoCAttribute
    {
        public bool IsSingleton { get; set; }

        public SingletonType()
        {
            this.IsSingleton = true;  // Default value.
        }

        public SingletonType(bool isSingleton)
        {
            this.IsSingleton = isSingleton;
        }
    }

    /// <summary>
    /// Class attribute to indicate to IOC that a new object should be created each time an instance of 
    /// this class is needed.
    /// </summary>

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]

    public class TransientType : System.Attribute, IoCAttribute
    {
        public bool IsTransient { get; set; }

        public TransientType()
        {
            this.IsTransient = true;
        }

        public TransientType(bool isTransient)
        {
            this.IsTransient = isTransient;
        }
    }

    /// <summary>
    /// Class attribute to indicate to IOC that objects lifestyle will be pre WCF request
    /// </summary>

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]

    public class PerWCFRequestType : System.Attribute, IoCAttribute
    {
        public bool IsPerWCFRequest { get; set; }

        public PerWCFRequestType()
        {
            this.IsPerWCFRequest = true;
        }

        public PerWCFRequestType(bool PerWCFRequest)
        {
            this.IsPerWCFRequest = PerWCFRequest;
        }
    }

    /// <summary>
    /// Class attribute used to register class as factory 
    /// </summary>

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]

    public class RegisterFactoryFor: System.Attribute, IoCAttribute
    {
        public Type _factoryType { get; set; }

        public RegisterFactoryFor(Type factoryType)
        {
            this._factoryType = factoryType;
        }
    }
  
    /// <summary>
    /// Class attribute to indicate that the IOC should register this class as
    /// a specific type.
    /// </summary>
    /// <remarks>
    /// NOTE: Castle does not like to have the same implementation class registered more than once,
    /// even if it is being registered for different service types. However, it doesn't complain 
    /// as long as the registration has a unique name. So, if the registration attribute doesn't supply
    /// a name, we create a unique one (a GUID).
    /// </remarks>

    [AttributeUsage(AttributeTargets.Class, Inherited=false)]

    public class RegisterAsType : System.Attribute, IoCAttribute
    {
        public IList<Type> RegistrationTypes { get; private set; }
        public String RegistrationName { get; private set; }

        /// <summary>
        /// Must set a type name.
        /// </summary>
        /// <param name="registrationType">Type which is used to register the class/interface in the IOC.</param>
        /// <exception cref="FatalException">In case the parameter <c>registrationType</c> is <c>null</c>.</exception>

        public RegisterAsType(Type registrationType)
        {
            this.RegistrationTypes = new List<Type>() { registrationType };
            this.RegistrationName = null;
        }

        /// <summary>
        /// Register the given service type for the implementation class to which this attribute
        /// is attached. Also, name the registered component.
        /// </summary>
        /// <param name="registrationType">service type for implementation class</param>
        /// <param name="regName">component name for registration</param>
        
        public RegisterAsType(Type registrationType, string regName)
        {
            this.RegistrationTypes = new List<Type>() { registrationType };   
            this.RegistrationName = regName;
        }

        /// <summary>
        /// Register the given service types for the implementation class to which this attribute 
        /// is attached.
        /// </summary>
        /// <param name="registrationType1">first service type</param>
        /// <param name="registrationType2">second service type</param>

        public RegisterAsType(Type registrationType1, Type registrationType2)
        {
            this.RegistrationTypes = new List<Type>() { registrationType1, registrationType2 };
            this.RegistrationName = null;
        }

        /// <summary>
        /// Register the given service types for the implementation class to which this attribute
        /// is attached. Also, name the registered component.
        /// </summary>
        /// <param name="registrationType">type to register implementation as</param>
        /// <param name="regName">name for this particular implementation</param>

        public RegisterAsType(Type registrationType1, Type registrationType2, String regName)
        {
            this.RegistrationTypes = new List<Type>() { registrationType1, registrationType2 };
            this.RegistrationName = regName;
        }
    }

    /// <summary>
    /// Attribute for defining which implementation to use when a service type has multiple
    /// implementations. This assumes that the implementations are named. The type 
    /// specified for a dependency should be the type of a constructor argument for the 
    /// class to which the attribute is attached. The string is the component name that
    /// should be used to satisfy this dependency.
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]

    public class DependsOn : System.Attribute, IoCAttribute
    {
        public Type ServiceType {get; private set;}
        public String Name {get; private set;}

        public DependsOn(Type serviceType, string name)
        {
            ServiceType = serviceType;
            Name = name;
        }
    }
}
