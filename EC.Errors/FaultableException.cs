using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace EC.Errors
{
    /// <summary>
    /// <para>
    /// Interface for all exceptions that support converion to WCF faults. NOTE: It may appear 
    /// that the method ToBasicFault() is sufficient, since one could call it and then wrap
    /// the returned fault into a FaultException. However, it is important that the FaultException
    /// be created with the the specific type parameter of the fault -- information which
    /// would be lost using ToBasicFault.
    /// </para>
    /// <para>
    /// A more natural solution to to would be to have a generic type parameter on this 
    /// interface to hold the specific fault type. However, the place where this interface is
    /// used (FaultHelper) handles all fault exceptions, and hence cannot bind that type
    /// parameter. So a non-parameterized class is needed.
    /// </para>
    /// </summary>
    
    public interface IFaultableException
    {
        FaultException ToFaultException(string path, CurrentUserInfo userInfo);
        BasicFault ToBasicFault(string path, CurrentUserInfo userInfo);
    }

    /// <summary>
    /// Base implementation for all exceptions that support conversion to WCF faults.
    /// </summary>

    public abstract class FaultableException<T> : Exception, IFaultableException where T : BasicFault
    {
        /// <summary>
        /// Return the fully typed FaultException corresponding to this exception. NOTE: It
        /// is important that the fault exception be created with the correct type parameter
        /// because WCF needs this information to serialize the actual fault.
        /// </summary>
        
        public FaultException ToFaultException(string path, CurrentUserInfo userInfo) 
        {
            var f = ToFault(path, userInfo);
            var ex = new FaultException<T>(f, f.Message);
            return ex;
        }

        public BasicFault ToBasicFault(string path, CurrentUserInfo userInfo)
        {
            return ToFault(path, userInfo);
        }

        /// <summary>
        /// Convert this exception into its corresponding fault.
        /// </summary>

        public abstract T ToFault(string path, CurrentUserInfo userInfo);

        /// <summary>
        /// Convert this exception into its corresponding FaultException properly
        /// parameterized by fault type.
        /// </summary>


        public FaultableException() : base() { }
        public FaultableException(string msg) : base(msg) { }
        public FaultableException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// Simple struct used to provide information about the current user to the methods
    /// in FaultableException.
    /// </summary>
    
    public class CurrentUserInfo
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
