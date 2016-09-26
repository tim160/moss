using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;
using Castle.MicroKernel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Core.Logging;
using EC.Common.Interfaces;
using System.ServiceModel.Description;

namespace EC.Core.Common
{
    /// <summary>
    /// Miscellaneous routines to help with setting up WCF and the Castle WCF facility.
    /// </summary>
   
    [TransientType]
    [RegisterAsType(typeof(IWCFHelper))]
    
    public class WCFHelper : IWCFHelper
    {
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
        /// <param name="useExternalEndpointBehaviour">Optional: Flag to add external endpoint behaviour (<c>true</c>) or the default endpoint behaviour. Default: false</param>
        
        public void RegisterWCFServiceImpl<I, C>(string serviceName, bool http, bool streamed, int port, int mexPort, int maxMsg, TimeSpan sendTimeout, TimeSpan receiveTimeout, 
            bool useExternalEndpointBehaviour) where I : class where C : I
        {
            Func<int, string, string> GenAddr = GetServiceAddressNetTCP;
            if (http) { GenAddr = GetServiceAddressHTTP; }
            var serviceUri = GenAddr(port, serviceName);
            List<string> baseAddresses = new List<string>();

            var endPoint = CreateEndpoint(
                GetEndpoint(serviceUri, streamed, maxMsg, sendTimeout, receiveTimeout),
                this.CreateEndpointBehaviour(useExternalEndpointBehaviour),
                baseAddresses,
                serviceUri);

            if (!http)
            {
                var mexUri = GetServiceAddressHTTP(mexPort, serviceName);
                baseAddresses.Add(mexUri);
            }

            try
            {
                var serviceModel = new DefaultServiceModel()
                    .PublishMetadata(o => o.EnableHttpGet())
                    .AddBaseAddresses(baseAddresses.ToArray());

                serviceModel.AddEndpoints(endPoint);
                
                string uniqueRegistrationName = Guid.NewGuid().ToString();
                Kernel.Register(Component
                    .For<I>()
                    .ImplementedBy<C>()
                    .AsWcfService(serviceModel)
                    .Named(uniqueRegistrationName));

                Logger.InfoFormat("START and REGISTER service '{0}' - listening at [{1}]: Endpoint-Behaviour-Count: {2}, Use-External-EndpointBehaviour = '{3}'.", serviceName, serviceUri, serviceModel.Extensions.Count(), useExternalEndpointBehaviour.ToString());
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error: Service registration failed for address = [{0}], Use-External-EndpointBehaviour = '{1}', Ex='{2}'", serviceUri,useExternalEndpointBehaviour.ToString(), ex.ToString());
                Logger.Fatal(msg);
                IoCSetup.ShutdownAndExit(msg, 1);
            }
        }

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
        
        public void RegisterWCFClientProxy<T>(string uri, bool streamed, int maxMsg, TimeSpan sendTimeout, TimeSpan receiveTimeout, bool useExternalEndpointBehaviour) where T:class
        {

            var endPoint = GetEndpoint(uri, streamed, maxMsg, sendTimeout, receiveTimeout);

            try
            {
                var clientModel = new DefaultClientModel() { Endpoint = endPoint.At(uri) };
                clientModel = clientModel.WithoutAsyncCapability();
                var endpointBehaviour = this.CreateEndpointBehaviour(useExternalEndpointBehaviour);
                clientModel.AddExtensions(endpointBehaviour);
                
                Kernel.Register(Component
                    .For<T>()
                    .LifeStyle.Transient
                    .AsWcfClient(clientModel)
                );
                
                Logger.InfoFormat("WCF CLIENT proxy registered at [{0}]: Endpoint-Behaviour-Count: {1}, Use-External-EndpointBehaviour = '{2}.'", uri, clientModel.Extensions.Count(), useExternalEndpointBehaviour.ToString());
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error: WCF client proxy registration failed at address = [{0}], useExternalEndpointBehaviour = '{1}', Ex='{2}'", uri, useExternalEndpointBehaviour.ToString(), ex.ToString());
                Logger.Fatal(msg);
                IoCSetup.ShutdownAndExit(msg, 1);
            }
        }

        /// <summary>
        /// Creates the WCF endpoint and configures port sharing depending on the 
        /// Core.WCFEnablePortSharing setting.
        /// </summary>
        /// <returns>The Castle endpoint facility contract</returns>
        
        private IWcfEndpoint CreateEndpoint(
            BindingEndpointModel baseEndpoint,
            IEndpointBehavior endpointBehaviour,
            List<string> baseAddresses,
            string serviceUri)
        {
            IWcfEndpoint endpoint = baseEndpoint;

            if (Settings.WCFEnablePortSharing)
            {
                baseAddresses.Add(serviceUri);
                baseEndpoint.AddExtensions(endpointBehaviour);
            }
            else
            {
                // add ACL's to allow http service registrations without account elevation (for mex endpoints)
                // run these commands in an elevated console (only need to run this once)
                // NOTE: this is only required when not running as an administrator
                // netsh http add urlacl url = http://+:8002/ user=\Everyone
                // netsh http add urlacl url = http://+:8003/ user=\Everyone
                // netsh http add urlacl url = http://+:8502/ user=\Everyone

                endpoint = baseEndpoint
                    // The .At(uri) enables a fully qualified endpoint Uri which avoids using a base address configuration
                    // By limiting base addresses to only HTTP (mex) endpoints we eliminate the requirement of port sharing
                    .At(serviceUri)
                    .AddExtensions(endpointBehaviour);
            }

            return endpoint;
        }

        /// <summary>
        /// Create correspondent endpoint behaviour.
        /// </summary>
        /// <param name="useExternalBehaviour">Flag which endpoint behaviour to return</param>
        /// <returns>Return new end point behaviour instance</returns>

        private IEndpointBehavior CreateEndpointBehaviour(bool useExternalBehaviour)
        {
            if (useExternalBehaviour)
            {
                return new AuthenticationEndpointBehavior(Kernel);
            }
            return new MLSEndpointBehavior(Kernel);
        }

        /// <summary>
        /// Return the WCF-style URI for service operating over NET TCP.
        /// </summary>
        /// <param name="port">The port the service is running on</param>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>URI for service</returns>
        
        private string GetServiceAddressNetTCP(int port, string serviceName)
        {
            return string.Format("net.tcp://localhost:{0}/Services/{1}", port.ToString(), serviceName);
        }

        /// <summary>
        /// Return the WCF-style URI for a service operating over HTTP.
        /// </summary>
        /// <param name="port">The port the service is running on</param>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>URI for service</returns>
        
        private string GetServiceAddressHTTP(int port, string serviceName)
        {
            return string.Format("http://localhost:{0}/Services/{1}", port.ToString(), serviceName);
        }

        /// <summary>
        /// Create a Castle WCF endpoint model for Basic HTTP
        /// </summary>
        /// <param name="maxReceivedMessageSize">maximum received message size allowed</param>
        /// <param name="sendTimeout">WCF timeout to send a message</param>
        /// <param name="receiveTimeout">WCF timeout to receive a message</param>
        /// <returns>a binding endpoint model</returns>

        private BindingEndpointModel CreateBasicHttpEndpoint(int maxReceivedMessageSize, TimeSpan sendTimeout, TimeSpan receiveTimeout)
        {
            return WcfEndpoint.BoundTo(new System.ServiceModel.BasicHttpBinding()
            {
                SendTimeout = sendTimeout,
                ReceiveTimeout = receiveTimeout,
                MaxReceivedMessageSize = maxReceivedMessageSize,
                ReaderQuotas = this.CreateReaderQuotas()
            });
        }

        /// <summary>
        /// Return an endpoint model for streamed basic http.
        /// </summary>
        /// <remarks>
        /// The high values for some of the parameters have the potential for a DOS attack 
        /// http://stackoverflow.com/questions/2117447/what-is-the-point-of-wcf-maxreceivedmessagesize 
        /// If you change these BasicHttpBinding settings please change the binding on 
        /// TestClient/ConfigureWCF() and in the Web project Global.asax
        /// </remarks>
        /// <param name="sendTimeout">WCF timeout to send a message</param>
        /// <param name="receiveTimeout">WCF timeout to receive a message</param>
        /// <returns>a binding endpoint model</returns>

        private BindingEndpointModel CreatedStreamedBasicHttpEndpoint(TimeSpan sendTimeout, TimeSpan receiveTimeout)
        {
            return WcfEndpoint.BoundTo(new System.ServiceModel.BasicHttpBinding
            {
                SendTimeout = sendTimeout,
                ReceiveTimeout = receiveTimeout,
                MaxReceivedMessageSize = long.MaxValue,
                MaxBufferSize = 5242880,
                MaxBufferPoolSize = 524288,
                TransferMode = TransferMode.Streamed,
                MessageEncoding = WSMessageEncoding.Mtom,
                ReaderQuotas = this.CreateReaderQuotas()
            });
        }

        /// <summary>
        /// Create a Castle WCF endpoint model for Net TCP
        /// </summary>
        /// <param name="maxReceivedMessageSize">maximum received message size allowed</param>
        /// <param name="sendTimeout">WCF timeout to send a message</param>
        /// <param name="receiveTimeout">WCF timeout to receive a message</param>
        /// <returns>a binding endpoint model</returns>
        
        private BindingEndpointModel CreateNetTCPEndpoint(int maxReceivedMessageSize, TimeSpan sendTimeout, TimeSpan receiveTimeout)
        {
            return WcfEndpoint.BoundTo(new System.ServiceModel.NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = sendTimeout,
                ReceiveTimeout = receiveTimeout,
                MaxReceivedMessageSize = maxReceivedMessageSize,
                PortSharingEnabled = Settings.WCFEnablePortSharing,
                ReaderQuotas = this.CreateReaderQuotas()
            });
        }

        /// <summary>
        /// Create a Castle WCF endpoint model for Net TCP that supports streaming.
        /// </summary>
        /// <param name="sendTimeout">WCF timeout to send a message</param>
        /// <param name="receiveTimeout">WCF timeout to receive a message</param>
        /// <returns>a binding endpoint model</returns>
        
        private BindingEndpointModel CreateStreamedNetTCPEndpoint(TimeSpan sendTimeout, TimeSpan receiveTimeout)
        {
            return WcfEndpoint.BoundTo(new System.ServiceModel.NetTcpBinding
            {
                Security = new NetTcpSecurity() { Mode = SecurityMode.None },
                PortSharingEnabled = Settings.WCFEnablePortSharing,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxReceivedMessageSize = long.MaxValue,
                MaxBufferSize = 5242880,
                MaxBufferPoolSize = 524288,
                TransferMode = TransferMode.Streamed,
                CloseTimeout = new TimeSpan(0, 10, 0, 0, 0),
                OpenTimeout = new TimeSpan(0, 0, 1, 0, 0),
                SendTimeout = sendTimeout,
                ReceiveTimeout = receiveTimeout,
                ReaderQuotas = this.CreateReaderQuotas()
            });
        }

        /// <summary>
        /// Return the appropriate endpoint based on the endpoint URI and whether streaming 
        /// should be supported.
        /// </summary>
        /// <remarks>If streaming is true maxMsg will not be used, instead all streams us long.MaxValue</remarks>
        /// <param name="uri">service endpoint URI</param>
        /// <param name="streamed">whether streaming must be supported</param>
        /// <returns>the endpoint</returns>

        private BindingEndpointModel GetEndpoint(string uri, bool streamed, int maxMsg, TimeSpan sendTimeout, TimeSpan receiveTimeout)
        {
            var http = !uri.Contains("net.tcp");
            if (http && streamed) { return CreatedStreamedBasicHttpEndpoint(sendTimeout, receiveTimeout); }
            if (http && !streamed) { return CreateBasicHttpEndpoint(maxMsg, sendTimeout, receiveTimeout); }
            if (!http && streamed) { return CreateStreamedNetTCPEndpoint(sendTimeout, receiveTimeout); }
            if (!http && !streamed) { return CreateNetTCPEndpoint(maxMsg, sendTimeout, receiveTimeout); }
            DBC.Assert(false, "Unreachable code");
            return null;
        }

        /// <summary>
        /// Create reader quotas for the endpoint.
        /// </summary>
        /// <returns></returns>

        private XmlDictionaryReaderQuotas CreateReaderQuotas()
        {
            return new XmlDictionaryReaderQuotas
                {
                    MaxDepth = 256,
                    MaxArrayLength = 1048576,   // in bytes
                    MaxStringContentLength = 4194304,  // 4MB
                    MaxNameTableCharCount = 16384,
                    MaxBytesPerRead = 4096
                };
        }

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public WCFHelper(IKernel k, ILogger l, IAppSettings s)
        {
            Kernel = k;
            Logger = l;
            Settings = s;
        }

        /// <summary>
        /// Allows this object to be allocated via Resolve() in a using statement
        /// and be properly released from the kernel.
        /// </summary>
        
        public void Dispose()
        {
            if (Disposing) { return; }
            Disposing = true;
            Kernel.ReleaseComponent(this);
        }

        // ---------------------------------- Injected Components ---------------------------------

        private IKernel Kernel = null;
        private ILogger Logger = null;
        private IAppSettings Settings = null;
        private bool Disposing = false;
    }
}
