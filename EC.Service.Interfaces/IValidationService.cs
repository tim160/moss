using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// This interface is used to extend existing services if they need user validation.
    /// </summary>
    /// <remarks>
    /// Always use the AuthenticationEndpointBehavior using the validation service.
    /// </remarks>

    [ServiceContract]
    public interface IValidationService 
    {
        /// <summary>
        /// Authenticate a user and return a unique authentication token for further calls.
        /// </summary>
        /// <remarks>
        /// The authentication token has a time out. Every time the token is used with an endpoint call
        /// the timeout is reset. <see cref="Logout"/> renders the authentication token invalid.
        /// </remarks>
        /// <param name="path">Path where the user is trying to logon</param>
        /// <param name="loginId">user name or email address to identify the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Return a unique authentication token</returns>
        /// <exception cref="NotFoundFault">If the user doesn't exist or is not active. If the <paramref name="path"/> doesn't exist</exception>
        /// <exception cref="NotAuthorizedFault">If the (<paramref name="loginId"/>, <paramref name="password"/>) pair don't match</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        Guid ValidateUser(string path, string loginId, string password);

        /// <summary>
        /// Renders the authentication token invalid. Use that if you don't need the token anymore.
        /// </summary>
        /// <remarks>
        /// Do nothing if authentication Id doesn't exist.
        /// </remarks>        
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void Logout();

        /// <summary>
        /// Call to just keep the authentication Id alive - extend timeout of the authentication token.
        /// </summary>
        /// <remarks>
        /// This will reset the timeout for the authentication Id.
        /// </remarks>
        /// <exception cref="AuthenticationRequiredFault">If wrong/no authentication Id or the user Id is wrong/not active/deleted.</exception>
        /// <exception cref="AuthenticationExpiredFault">If the authentication Id has expired</exception>
        /// <exception cref="NotFoundFault">If the authentication Id doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(AuthenticationExpiredFault))]     
        [FaultContract(typeof(UnknownFault))]

        void KeepAlive();
       
        /// <summary>
        /// This is used to check the network connection (and possibly network latency).
        /// </summary>
        /// <remarks>
        /// This call doesn't need an authentication Id. Callable without validation.
        /// </remarks>
        /// <exception cref="UnkownFault">On any error</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void Ping(Guid? siteInfoId = null, DateTime? monoTime = null, DateTime? sysTime = null);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <remarks>
        /// This call doesn't need an authentication Id. Callable without validation.
        /// </remarks>
        /// <returns>true</returns>
        /// <exception cref="UnknownFault">On any error</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        bool IsValidationService();
    }
}
