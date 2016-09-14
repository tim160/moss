using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EC.Common.Base
{
    public static class GuidTools
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        public static Guid SequentialGuid()
        {
            const int RPC_S_OK = 0;
            Guid g;
            if (UuidCreateSequential(out g) != RPC_S_OK)
                return Guid.NewGuid();
            else
                return g;
        }
    }
}
