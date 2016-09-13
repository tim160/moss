using System;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Component which supports methods for registering Castle WCF client proxies and service implementations
    /// </summary>
    
    public interface IWCFHelper : IDisposable
    {
        /// <summary>
        /// Register a WCF client proxy for the given service interface with the IOC. The service
        /// will be contacted at the given URI.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To register different endpoint behaviours the behaviour must be added to the clientModel (not the endpoint directly like the service registration).
        /// Adding the endpoint behaviours directly to the endpoint will not invoke the endpoint behaviours.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">service interface</typeparam>
        /// <param name="k">IoC kernel to register to</param>
        /// <param name="uri">service URI</param>
        /// <param name="streamed">true if streaming must be supported</param>
        /// <param name="maxMsg">maximum message size supported</param>
        /// <param name="sendTimeout">WCF proxy timeout to send a message</param>
        /// <param name="receiveTimeout">WCF proxy timeout to receive a message</param>
        /// <param name="useExternalEndpointBehaviour">Optional: Flag to add external endpoint behaviour (<c>true</c>) or the default endpoint behaviour. Default: false</param>

        void RegisterWCFClientProxy<T>(string uri, bool streamed, int maxMsg, TimeSpan sendTimeout, TimeSpan receiveTimeout, bool useExternalEndpointBehaviour = false) where T : class;

        /// <summary>
        /// Register a WCF service and implementation with the IoC
        /// </summary>
        /// <para>
        /// WCF configuration is finicky. If you want metadata published, you need to use BaseAddresses rather 
        /// than configuring the endpoint with .At() -- which is the interface that most examples use. Also,
        /// metadata cannot be published over the Net.Tcp transport; it only works over Http. So if you are
        /// using Net.Tcp and want metadata published then you have to have an Http base address in addition
        /// to the Net.Tcp address. And in this case, the Http base address and the Net.Tcp address must
        /// have different port numbers (because you can't have two different transports bound to the
        /// same port). 
        /// </para>
        /// <para>
        /// For self-hosted services, you must *not* use .Hosted() on the service model, as that indicates 
        /// the services lives in IIS/WAS. Also, the service implementation must *not* have a ServiceBehaviour 
        /// attribute, because that will cause .NET to try and create the service host and it demands a 
        /// parameter-less constructor (which we don't have since we're trying to do dependency injection 
        /// via constructor parameters).
        /// </para>
        /// <para>
        /// To register different endpoint behaviours the behaviour must be added to the endpoint instance as an extension (AddExtension).
        /// Adding the endpoint behaviours to the serviceModel directly fails if the same endpoint behaviour is registered for another service.
        /// </para>
        /// <para>
        /// Every service will be registered with a unique random name because registering 2 services with different (I) interfaces but same implementation (C)
        /// conflicts with Castle Windsor - it needs a unique name. 
        /// The unique name is never been used to resolve the registered service. It is only needed to generate a unique key for Castle Windsor.
        /// Resolve the service with the proper interface (I).
        /// </para>
        /// </remarks>
        /// <typeparam name="I">interface for service</typeparam>
        /// <typeparam name="C">implementation class for service</typeparam>
        /// <param name="serviceName">service name (used in endpoint URL)</param>
        /// <param name="http">indicates whether to use the HTTP transport or Net.Tcp</param>
        /// <param name="streamed">whether the service needs a streamed protocol</param>
        /// <param name="port">the port number to use for the service</param>
        /// <param name="mexPort">the port for the mex endpoint (only used if http == false)</param>
        /// <param name="maxMsg">the max message size configuration parameter for WCF</param>
        /// <param name="sendTimeout">WCF service timeout to send a message</param>
        /// <param name="receiveTimeout">WCF service timeout to receive a message</param>
        /// <param name="useExternalEndpointBehaviour">Optional: Flag to add the authorization endpoint behaviour (<c>true</c>) or the default endpoint behaviour. Default: false</param>

        void RegisterWCFServiceImpl<I, C>(string serviceName, bool http, bool streamed, int port,int mexPort, int maxMsg, TimeSpan sendTimeout, TimeSpan receiveTimeout, bool useAuthorizationEndpointBehaviour = false) where I : class where C : I;
    }
}
