using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using EC.Errors;
using EC.Errors.CommonExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// Created separate interface to be able to isolate CUK related methods
    /// </summary>
    [ServiceContract]
    public interface ICUKIntegration
    {
        /// <summary>
        /// Import users from CUK MAPS formatted CSV
        /// </summary>
        /// <param name="mapsFile">Stream that provides the MAPS CSV to be imported</param>
        /// <exception cref="IsRequiredFault"></exception>
        /// <exception cref="ParameterValidationFault"></exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path, or the login Id</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(IsRequiredFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        List<string> ImportUsers(Stream mapsFile);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]
        bool IsCUKIntegration();
    }
}