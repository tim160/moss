using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Provide access to the RequestContext.
    /// <para>
    /// The request context provides a small set of information that needs to be available
    /// through the application. Rather than passing this information as parameters everywhere,
    /// IManageRequestContext provides a means to retrieve the current request context from
    /// any code running within the application. In addition, the MLSEndpointBehavior class
    /// ensures that this information is transparently carried along with all WCF requests.
    /// </para>
    /// </summary>
    
    public interface IManageRequestContexts
    {
        /// <summary>
        /// Get the RequestContext for the current thread.
        /// </summary>
        /// <returns>the request context</returns>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        IRequestContext GetContext();

        /// <summary>
        /// Predicate indicating whether the request context has been initialized. If this 
        /// returns false, then either InitializeDefaultRequestContext() or
        /// InitializeThreadRequestContext() should be called to establish a request context
        /// for the current thread.
        /// </summary>

        bool IsContextInitialized();

        /// <summary>
        /// Initialize the current thread to use the 'default' request context. The default context
        /// is defined by the Core.RequestContext app config setting. The setting must be consistent
        /// with the type of application. That is, a WCF server must use the setting 'server', while 
        /// a Web app must use 'web'. 
        /// </summary>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        void InitializeDefaultRequestContext();

        /// <summary>
        /// <para>
        /// Initialize the current thread to use the 'thread' context. The 'thread' context will be used
        /// by threads that are spawned within the application. These threads generally cannot use the
        /// 'default' thread context because they are not bound to requests in the same way. For 
        /// example, a thread spawned in a web application has no associated web request, and cannot
        /// use the PerWebRequest lifestyle for the request context.
        /// </para>
        /// <para>
        /// NOTE: When a thread uses InitializeThreadRequestContext() it must call ClearThreadContext()
        /// before returning from that thread. The .NET infrastructure uses thread pooling, so returning
        /// from a thread does not necessarily terminate that thread. This can lead to accidental re-use
        /// of a request context. The request manager checks for this, so failure to call
        /// ClearThreadContext() may cause failures in subsequent calls to initialize the request context.
        /// </para>
        /// </summary>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>
        
        void InitializeThreadRequestContext();

        /// <summary>
        /// Clear the per thread request context for this thread. This call must be made when an application
        /// spawned thread is about to return control out of the main thread body. This call is required in
        /// order to prevent inadvertent sharing of request contexts due to thread pooling.
        /// </summary>
        /// <exception cref="FatalException">if the application is using the context inconsistently</exception>

        void ClearThreadContext();
    }
}
