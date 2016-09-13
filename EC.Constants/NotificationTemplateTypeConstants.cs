using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Specify the type of the notification template.
    /// </summary>

    [DataContract]
    public enum NotificationTemplateTypeEnum
    {
        /// <summary>
        /// Full template for the notification.
        /// </summary>

        [EnumMember]
        Full = 0,

        /// <summary>
        /// Title template for the notification.
        /// </summary>

        [EnumMember]
        Title = 1,

        /// <summary>
        /// Summary template for the notification.
        /// </summary>

        [EnumMember]
        Summary = 2
    }
}
