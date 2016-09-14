using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Priority levels for notifications.
    /// <remarks>
    /// Keep up-to-date with DTO enumeration.
    /// </remarks>
    /// </summary>

    [DataContract]

    public enum NotificationPrioritiesEnum
    {
        [EnumMember]
        Low = 0,

        [EnumMember]
        Medium = 1,
        
        [EnumMember]
        High = 2
    }
}
