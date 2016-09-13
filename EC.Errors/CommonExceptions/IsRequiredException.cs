using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// This exception indicates that something is missing.
    /// </summary>

    public class IsRequiredException : FaultableException<IsRequiredFault>
    {
        public override IsRequiredFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new IsRequiredFault(Message, path, userInfo);
            f.ItemName = ItemName;
            return f;
        }

        /// <summary>
        /// This exception indicates that something is missing.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemName">Item name which is required</param>
        /// <param name="innerException">Any inner exception</param>

        public IsRequiredException(string message, string itemName, Exception innerException = null) : base(message, innerException)
        {
            this.ItemName = itemName;
        }

        /// <summary>
        /// Item name which is required.
        /// </summary>

        public string ItemName { get; set; }
    }

    /// <summary>
    /// Thrown if something is missing but required.
    /// </summary>

    [DataContract]
    public class IsRequiredFault : BasicFault
    {
        public IsRequiredFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
        /// <summary>
        /// Name of the required item (e.g. property name).
        /// </summary>

        [DataMember]
        public string ItemName { get; set; }
    }
}
