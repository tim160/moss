using System;

namespace EC.Common.Base
{
    /// <summary>
    /// Exceptions of this class indicate an error so severe that the system cannot continue
    /// operating -- either for the current request or any subsequent request. Fatal exceptions
    /// should be relatively rare; OperationFailedExceptions are more common as they indicate
    /// a problem that is likely only affecting the current request/operation.
    /// </summary>
    
    public class FatalException : Exception
    {
        public FatalException() : base()
        {
        }

        public FatalException(string message) : base(message)
        {
        }

        public FatalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
