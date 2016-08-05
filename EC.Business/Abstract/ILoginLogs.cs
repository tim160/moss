using EC.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Business.Actions
{
    public interface ILoginLogs
    {
        void Add(int eventTypeId, long? permissionId, bool success, string sessionId, long? userId, string ip, ClientType clientType, string username, string password, DateTime timestamp, string failedReason = null);
        void Add(int eventTypeId, long? permissionId, bool success, string sessionId, long? userId, string ip, string mac, ClientType clientType, string clientVersion, string username, string password, DateTime timestamp, string failedReason = null);
    }
}
