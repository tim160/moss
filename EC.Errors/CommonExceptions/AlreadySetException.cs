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
    /// This exception indicates that something has already been set and can't be replaced with a different item.
    /// </summary>

    public class AlreadySetException : FaultableException<AlreadySetFault>
    {
        public override AlreadySetFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new AlreadySetFault(Message, path, userInfo);
            f.ExistingItemId = ExistingItemId;
            f.NewItemId = NewItemId;
            return f;
        }

        /// <summary>
        /// This exception indicates that something has already been set and can't be replaced with a different item.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="existingItemId">Item Id which is set already</param>
        /// <param name="newItemId">Optional: If the new item Id is known</param>
        /// <param name="innerException">Optional: Any inner exception</param>

        public AlreadySetException(string message, Guid? existingItemId, Guid? newItemId = null, Exception innerException = null) : base(message, innerException)
        {
            this.ExistingItemId = existingItemId;
            this.NewItemId = newItemId;
        }

        /// <summary>
        /// Item name which is already set.
        /// </summary>

        public Guid? ExistingItemId { get; set; }

        /// <summary>
        /// Item which was tried to set.
        /// </summary>

        public Guid? NewItemId { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an item has been set and can't be replaced with a different item.
    /// </summary>

    [DataContract]
    public class AlreadySetFault : BasicFault
    {
        public AlreadySetFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Get or set the item Id already set.
        /// </summary>

        [DataMember]
        public Guid? ExistingItemId { get; set; }

        /// <summary>
        /// Get or set the item Id which was tried to be set.
        /// </summary>

        [DataMember]
        public Guid? NewItemId { get; set; }
    }
}
