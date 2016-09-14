using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If a requested link is not in the nav path. 
    /// </summary>

    public class LinkNotFoundInPathException : FaultableException<LinkNotFoundInPathFault>
    {
        public override LinkNotFoundInPathFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new LinkNotFoundInPathFault(Message, reqPath, userInfo);
            f.ItemPath = ItemPath;
            f.ItemId = ItemId;
            return f;
        }

        public LinkNotFoundInPathException(string itemPath, string message, Exception innerException = null) : base(message, innerException)
        {
            this.ItemPath = itemPath;
        }

        public LinkNotFoundInPathException(Guid itemId, string message, Exception innerException = null) : base(message, innerException)
        {
            this.ItemId = itemId;
        }

        public string ItemPath { get; set; }

        public Guid? ItemId { get; set; }
    }

    [DataContract]
    public class LinkNotFoundInPathFault : BasicFault
    {
        public LinkNotFoundInPathFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string ItemPath { get; set; }

        [DataMember]
        public Guid? ItemId { get; set; }
    }
}
