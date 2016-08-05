using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    // bit wise, positional Notification Type
    public enum NotificationType
    {
        Indeterminate = 0,
        AssignOrder = 1,
        UnassignOrder = 2,
        EntityData = 4,                     // found in unused call
        ValueList = 8,                      // unused
        OrderType = 16,                     // found in unused call
        TextMsg = 32,
        Software = 64,                      // unused
        OrderModification = 128,
        OrderProjectModification = 256,     // unused
        Ping = 512,                         // not used in DB
        LISCheckout = 1024,
        QueuedMessageUpdate = 2048,
        AcknowledgeSafetyTimer = 4096,
        CancelSafetyTimer = 8192,
        SessionInvalidated = 16384
    }
}
