using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace EC.Common.Base
{
    /// <summary>
    /// This class of exception indicates a problem that terminates processing of the current
    /// request or operation.
    /// </summary>
    
    public class OperationFailedException : Exception
    {
        public OperationFailedException() : base()
        {
        }

        public OperationFailedException(string message) : base(message)
        {
        }

        public OperationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This class of exception indicates that the operation failed because an exception
    /// occurred which was not planned for in the code. Generally speaking this is indicative
    /// of a bug.
    /// </summary>
    
    public class UnexpectedException : OperationFailedException
    {
        public UnexpectedException(string msg, Exception ex) : base(msg, ex)
        {
        }
    }
}
