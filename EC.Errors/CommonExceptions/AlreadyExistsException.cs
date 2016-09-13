using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// This exception indicates that something already exists (e.g. within a collection).
    /// </summary>

    public class AlreadyExistsException : FaultableException<AlreadyExistsFault>
    {
        public override AlreadyExistsFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new AlreadyExistsFault(Message, path, userInfo);
            f.ItemName = ItemName;
            return f;
        }

        /// <summary>
        /// This exception indicates that something already exists (e.g. within a collection).
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="itemName">Item name which already exists</param>
        /// <param name="innerException">Any inner exception</param>

        public AlreadyExistsException(string message, string itemName, Exception innerException = null) : base(message, innerException)
        {
            this.ItemName = itemName;
        }

        /// <summary>
        /// Message displayed in logs for this exception.
        /// </summary>
        
        public override string Message
        {
            get
            {
                var msg = base.Message;
                msg += String.Format(", Item = {0}", String.IsNullOrWhiteSpace(ItemName) ? "n/a" : ItemName);
                return msg;
            }
        }

        /// <summary>
        /// Item name which already exists.
        /// </summary>

        public string ItemName { get; set; }
    }

    /// <summary>
    /// Fault used to indicate a double entry or trying to make a double entry.
    /// </summary>

    [DataContract]
    public class AlreadyExistsFault : BasicFault
    {
        public AlreadyExistsFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Get or set the item name which already exists.
        /// </summary>

        [DataMember]
        public string ItemName { get; set; }
    }
}
