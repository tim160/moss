using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using EC.Common.Interfaces;
using EC.Service.DTO;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// Publicly accessible (secure) service interface for notifications.
    /// </summary>
    [ServiceContract]

    public interface INotificationServiceSecure
    {
        // NOTE: No operations for this service yet because it is not clear what the
        // permissions model is for sending notifications.
    }
}
