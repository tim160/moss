using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception if the content item is not a NavPage.
    /// </summary>

    public class NotANavPageException : FaultableException<NotANavPageFault>
    {
        public override NotANavPageFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NotANavPageFault(Message, path, userInfo);
            f.ItemId = ItemId;
            f.ItemPath = ItemPath;
            return f;
        }

        /// <summary>
        /// Exception if the item is not a NavPage.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemId">Item Id.</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception.</param>

        public NotANavPageException(string message, Guid itemId, string itemPath = null, Exception innerException = null) : base(message, innerException)
        {
            this.ItemId = itemId;
            this.ItemPath = itemPath;
        }

        /// <summary>
        /// Exception if the content item is not a NavPage.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception</param>

        public NotANavPageException(string message, string itemPath = null, Exception innerException = null) : base(message, innerException)
        {
            this.ItemPath = itemPath;
        }

        /// <summary>
        /// Path of the item which is not a NavPage.
        /// </summary>

        public string ItemPath { get; set; }

        /// <summary>
        /// Id of the item which is not a NavPage.
        /// Guid.Empty if not set.
        /// </summary>

        public Guid ItemId { get; set; }
    }

    /// <summary>
    /// Exception if the content item is not a course.
    /// </summary>

    public class NotACourseException : NotANavPageException
    {
        public override NotANavPageFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NotACourseFault(Message, path, userInfo);
            f.ItemId = ItemId;
            f.ItemPath = ItemPath;
            return f;
        }

        /// <summary>
        /// Exception if the content item is not a course.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemId">Item Id.</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception.</param>

        public NotACourseException(string message, Guid itemId, string itemPath = null, Exception innerException = null) : base(message, itemId, itemPath, innerException)
        {
        }

        /// <summary>
        /// Exception if the content item is not a course.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="itemPath">Item path</param>
        /// <param name="innerException">Any inner exception</param>

        public NotACourseException(string message, string itemPath = null, Exception innerException = null) : base(message, Guid.Empty, itemPath, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown if the content item is found, but isn't a nav page.
    /// </summary>

    [DataContract]
    public class NotANavPageFault : BasicFault
    {
        public NotANavPageFault(string msg, string path, CurrentUserInfo userInfo)  : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Id of the content item which is not a nav page.
        /// </summary>

        [DataMember]
        public Guid? ItemId { get; set; }

        [DataMember]
        public string ItemPath { get; set; }
    }

    /// <summary>
    /// Thrown if the content item is found, but the item is not a course because it isn't a nav page.
    /// </summary>


    [DataContract]
    public class NotACourseFault : NotANavPageFault
    {
        public NotACourseFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
