using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    public class NotANavPageLinkException : FaultableException<NotANavPageLinkFault>
    {
        public override NotANavPageLinkFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotANavPageLinkFault(Message, reqPath, userInfo);
            f.ItemPath = ItemPath;
            f.ItemId = ItemId;
            return f;
        }

        /// <summary>
        /// Exception if the content item is not a nav page.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemId">Item Id.</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception.</param>

        public NotANavPageLinkException(string message, Guid itemId, string itemPath = null, Exception innerException = null) : base(message, innerException)
        {
            this.ItemId = itemId;
            this.ItemPath = itemPath;
        }

        /// <summary>
        /// Exception if the content item is not a nav page link.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception</param>

        public NotANavPageLinkException(string message, string itemPath = null, Exception innerException = null) : base(message, innerException)
        {
            this.ItemPath = itemPath;
        }

        /// <summary>
        /// Path of the link item which is not a nav page link.
        /// </summary>

        public string ItemPath { get; set; }

        /// <summary>
        /// Id of the link item which is not nav page link.
        /// Guid.Empty if not set.
        /// </summary>

        public Guid ItemId { get; set; }
    }

    /// <summary>
    /// Fault thrown when a link must point to a Nav Page (for correctness), but does not.
    /// </summary>
    
    [DataContract]
    public class NotANavPageLinkFault : BasicFault
    {
        public NotANavPageLinkFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path of the link item which is not a nav page link.
        /// </summary>

        [DataMember]
        public string ItemPath { get; set; }

        /// <summary>
        /// Id of the link item which is not nav page link.
        /// Guid.Empty if not set.
        /// </summary>

        [DataMember]
        public Guid ItemId { get; set; }
    }
}
