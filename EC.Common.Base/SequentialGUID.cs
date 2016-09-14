using System;
using System.Runtime.InteropServices;

namespace EC.Common.Base
{
    public class SequentialGUID
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        internal static extern int UuidCreateSequential(out Guid guid);

        public static Guid Create()
        {
            Guid retVal;
            var rc = UuidCreateSequential(out retVal);

            if (rc != RPS_S_OK)
            {
                var ex = new GuidCreateFailedException("Could not allocate sequential GUID");
                ex.ErrorCode = rc;
                throw ex;
            }

            return retVal;
        }

        private const int RPS_S_OK = 0;
    }

    /// <summary>
    /// Exception thrown when we cannot generate a new sequential GUID.
    /// </summary>
    
    public class GuidCreateFailedException : OperationCanceledException
    {
        public GuidCreateFailedException(string msg) : base(msg)
        {
        }

        public int ErrorCode { get; set; }
    }
}
