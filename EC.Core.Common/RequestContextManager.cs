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
using Castle.MicroKernel;
using EC.Errors;

namespace EC.Core.Common
{
    /// <summary>
    /// Provide access to the RequestContext. The RequestContext carries information both 
    /// across and within processes that is convenient to have globally available, e.g.
    /// the ID of the user associated with the running thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation uses a combination of Castle lifestyles and explicit thread local
    /// storage to ensure that a caller receives the 'right' context. There are basically 
    /// 3 situations that the code has to handle. (1) The code is executing in a thread
    /// which is controlled by IIS/WCF. (2) The code is executing in a stand-alone application.
    /// (3) The code is executing in a application spawned thread. The first two cases
    /// are handled by having IRequestContext registered in Castle with the appropriate
    /// lifestyle (e.g. PerWebReqeust, PerWCFOperation, PerThread). The third case is 
    /// handled using thread local storage - which raises the question of why one couldn't
    /// just use the PerThread lifestyle in that case as well. The reason is primarily for
    /// error checking. Using Kernel.Resolve() its not easy to distinguish the first creation
    /// of an object from subsequent lookups. But here we want to distinguish those cases
    /// in order to do error checking. Explicitly allocating the RequestContext and
    /// storing it in thread local storage lets us do this by checking to see if the
    /// thread local storage reference is null or not. It could possibly have been done
    /// by adding a flag to the RequestContext (to indicate first allocation) and then
    /// using the PerThread lifestyle.
    /// </para>
    /// <para>
    /// Historical Notes: Initially, access to the request context was simply done by 
    /// resolving IRequestContext. That interface was set up with either the PerWebRequest or 
    /// PerWCFOperation lifestyles (depending on whether you are in the front end or back end code). 
    /// However, in order to support spawning of C# Tasks and allowing those tasks to have access to
    /// the request context (needed, for example, to know who the user making the request
    /// is), we needed a more sophisticated mechanism (because if you attempt to 
    /// resolve IRequestContext in a thread that is not directly handling a request, you
    /// get NULL back).
    /// </para>
    /// </remarks>
    
    [SingletonType]
    [RegisterAsType(typeof(IManageRequestContexts))]

    public class RequestContextManager : IManageRequestContexts
    {
        /// <summary>
        /// Get the RequestContext for the current thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the thread was initialized to use the default request context, then we can simply
        /// resolve IRequestContext -- whose lifestyle is defined by an app.config setting. E.g.
        /// in a 'server' app we'll get a request context bound to the WCF operation. If the
        /// thread was spawned by the application, then it must call InitializeThreadRequestContext()
        /// prior to trying to obtain the context, in which case the PerThreadRequestContext will be
        /// set and can be returned. Otherwise, the application is in an erroneous state and we
        /// throw an exception.
        /// </para>
        /// <para>
        /// It might seem that when the default context is available we should also check to see
        /// whether the per thread context is set as well (which would be an error). However, the
        /// only way the per thread context can be set is via InitializeThreadRequestContext() and
        /// it will fail if the default context is available.
        /// </para>
        /// <para>
        /// The availability of the default context is determined using a feature of Castle. For
        /// example, if IRequestContext has the PerWCFOperation lifestyle and you attempt to
        /// resolve it when there is no active WCF operation, then Castle throws an exception. An
        /// application spawned thread will not have an associated active WCF operation and so
        /// we can detect when we are in an application spawned thread.
        /// </para>
        /// </remarks>
        /// <returns>the request context</returns>
        /// <exception cref="FatalException">if the context has not been correctly initialized</exception>
        
        public IRequestContext GetContext()
        {
            if (DefaultContextAvailable()) { return Kernel.Resolve<IRequestContext>(); }
            if (PerThreadRequestContext != null) { return PerThreadRequestContext; }
            DBC.Assert(false, "GetContext - no context available");
            return null;
        }

        /// <summary>
        /// Predicate indicating whether the request context has been initialized. If this 
        /// returns false, then either InitializeDefaultRequestContext() or
        /// InitializeThreadRequestContext() should be called to establish a request context
        /// for the current thread.
        /// </summary>

        public bool IsContextInitialized()
        {
            if (DefaultContextAvailable())
            {
                var context = Kernel.Resolve<IRequestContext>();
                var contextImpl = context as RequestContext;
                DBC.NonNull(contextImpl, "Could not down cast IRequestContext to implementation class");
                return contextImpl.IsInitialized;
            }

            if (PerThreadRequestContext != null)
            {
                return PerThreadRequestContext.IsInitialized;
            }

            return false;
        }

        /// <summary>
        /// Initialize the current thread to use the 'default' request context. The default context
        /// is defined by the Core.RequestContext app config setting. The setting must be consistent
        /// with the type of application. That is, a WCF server must use the setting 'server', while 
        /// a Web app must use 'web'. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the per thread context has been set, then the application is in an invalid state. This
        /// really should not be possible, because the only way to set the per thread context is
        /// via InitializeThreadRequestContext(), and it will fail if the default context is available.
        /// </para>
        /// </remarks>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        public void InitializeDefaultRequestContext()
        {
            DBC.Assert(PerThreadRequestContext == null, "Initialize context (default): PerThread context exists");
            DBC.Assert(DefaultContextAvailable(), "Initialize context (default): No default context available");
            var context = Kernel.Resolve<IRequestContext>();
            var contextImpl = context as RequestContext;
            DBC.NonNull(contextImpl, "Could not down cast IRequestContext to implementation class");
            DBC.Assert(!contextImpl.IsInitialized, "Initialize context (default): Context already initialized");
            contextImpl.IsInitialized = true;
        }

        /// <summary>
        /// <para>
        /// Initialize the current thread to use the 'thread' context.
        /// </para>
        /// <para>
        /// The 'thread' context is only used for threads explicitly spawned within the application.
        /// These threads generally cannot use the 'default' thread context because the default context
        /// may refer to a lifestyle that does not exist for them.  For example, a thread spawned in a 
        /// web application has no associated web request, and therefore cannot use the PerWebRequest 
        /// lifestyle (which is what the default request context will be set up to use).
        /// </para>
        /// <para>
        /// NOTE: When a thread uses InitializeThreadRequestContext() it must call ClearThreadContext()
        /// before returning from that thread. The .NET infrastructure uses thread pooling, so returning
        /// from a thread does not necessarily terminate that thread. This can lead to accidental re-use
        /// of a request context. The request manager checks for this, so failure to call
        /// ClearThreadContext() may cause failures in subsequent calls to initialize the request context.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This call explicitly allocates a request context object and assigns it to thread local storage.
        /// This gives us a request context bound to that specific thread. If the default thread context
        /// is available, that is considered an error because the request context should be bound to
        /// the scope associated with that default lifestyle. Also, if the PerThreadRequest context
        /// is not null, InitializeThreadRequestContext() has been called without a subsequent 
        /// ClearThreadContext(). This is an error because it would lead to erroneous sharing of the
        /// request context.
        /// </para>
        /// </remarks>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        public void InitializeThreadRequestContext()
        {
            DBC.Assert(!DefaultContextAvailable(), "Initialize context (thread): Default context is available");
            DBC.Assert(PerThreadRequestContext == null, "Initialize context (thread): Thread context already set");
            PerThreadRequestContext = new RequestContext();
            PerThreadRequestContext.IsInitialized = true;
        }

        /// <summary>
        /// Clear the per thread request context for this thread. This call must be made when an application
        /// spawned thread is about to return control out of the main thread body. This call is required in
        /// order to prevent inadvertent sharing of request contexts due to thread pooling.
        /// </summary>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        public void ClearThreadContext()
        {
            DBC.Assert(!DefaultContextAvailable(), "Clear: Default context is available when clearing thread context");
            PerThreadRequestContext = null;
        }

        /// <summary>
        /// Check to see if the 'default' implementation of IRequestContext is available.
        /// </summary>
        /// <returns>true if context is available</returns>
        
        private bool DefaultContextAvailable()
        {
            try
            {
                Kernel.Resolve<IRequestContext>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public RequestContextManager(IKernel k, ILogger l)
        {
            Kernel = k;
            Logger = l;
        }
        
        /// <summary>
        /// Thread local storage for the RequestContext. NOTE: The property *must* be declared
        /// as static. Using the ThreadStatic attribute on a non-static member has unpredictable
        /// results -- that is, sometimes the value in the member seems to change without having
        /// been modified by any of our code.
        /// </summary>
        
        [ThreadStatic]
        private static RequestContext PerThreadRequestContext;

        // ---------------------------- Injected Components ---------------------------------------

        private readonly IKernel Kernel = null;
        private readonly ILogger Logger = null;
    }

    /// <summary>
    /// Implementation for IRequestContext.
    /// </summary>
    /// <remarks>
    /// This class is explicitly registered in IoC in the code below.
    /// </remarks>

    
    internal class RequestContext : IRequestContext
    {
        /// <summary>
        /// Initialize this request context from another.
        /// </summary>
        /// <param name="src">context to initialize from</param>
        
        public void CopyFrom(IRequestContext src)
        {
            var rcImpl = src as RequestContext;
            DBC.Assert(rcImpl.IsInitialized, "Cannot copy an uninitialized RequestContext");
            CurrentUserId = src.CurrentUserId;
            CurrentUserName = src.CurrentUserName;
            ConversationId = src.ConversationId;
            AuthenticationId = src.AuthenticationId;  
            UserTimeZone = src.UserTimeZone;
            UserLanguage = src.UserLanguage;
            URLHost = src.URLHost;
            DisplayName = src.DisplayName;  
            IsInitialized = true;
        }

        // --------------------------------- Exposed State ----------------------------------------

        public Guid? CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? AuthenticationId { get; set; }
        public string UserTimeZone { get; set; }
        public string UserLanguage { get; set; }
        public string URLHost { get; set; }
        public string DisplayName { get; set; }

        // --------------------------------- Internal State ------------------------------------------

        public bool IsInitialized { get; set; }
    }

    /// <para>
    /// IRequestContext needs to be registered with specific lifestyles depending
    /// on the context of the application that we are running in. An app.config
    /// setting controls which mode is chosen. 
    /// </para>

    public class RequestContextComponentInstaller : IComponentInstaller
    {
        public void InstallComponents(Castle.Windsor.IWindsorContainer w)
        {
            var logger = w.Resolve<ILogger>();
            var context = w.Resolve<IAppSettings>().RequestContextStyle;
            
            switch (context)
            {
                case AppSettingsConstants.CodeContext.server:

                    w.Register(Component
                        .For<IRequestContext>()
                        .ImplementedBy<RequestContext>()
                        .LifestylePerWcfOperation()
                    );
                    logger.Debug("RequestContext set up with PerWCFOperation lifestyle");
                    break;

                case AppSettingsConstants.CodeContext.web:

                    w.Register(Component
                        .For<IRequestContext>()
                        .ImplementedBy<RequestContext>()
                        .LifestylePerWebRequest()
                    );
             
                    logger.Debug("RequestContext set up with PerWebRequest lifestyle ");
                    break;

                case AppSettingsConstants.CodeContext.client:

                    w.Register(Component
                        .For<IRequestContext>()
                        .ImplementedBy<RequestContext>()
                        .LifestylePerThread()
                    );
                    logger.Debug("RequestContext set up with PerThread lifestyle");
                    break;

                default:
                    DBC.Assert(false, "Unrecognized code context setting");
                    break;
            }
           
        }

        /// <summary>
        /// No post-installer required for request contexts/
        /// </summary>

        public void PostInstallComponents(Castle.Windsor.IWindsorContainer w)
        {
        }
    }
}
