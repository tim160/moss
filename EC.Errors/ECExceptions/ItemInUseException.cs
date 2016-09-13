using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when an item cannot be deleted because it is referenced by another
    /// item or items.
    /// </summary>

    public class ItemInUseException : FaultableException<ItemInUseFault>
    {
        public override ItemInUseFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new ItemInUseFault(Message, reqPath, userInfo);
            f.ItemType = ItemType;
            f.UsedBy = UsedBy;
            f.ItemPath = ItemPath;
            return f;
        }

        public ItemInUseException(string msg, string itemPath, string itemType, string usedBy, Exception innerException = null) : base(string.Format("Item cannot be deleted because it is in use: {0}, source type={1}, usedBy={2} [{3}]", itemPath, itemType, usedBy, msg), innerException)
        {
            ItemPath = itemPath;
            ItemType = itemType;
            UsedBy = usedBy;
        }

        public string ItemPath = null;
        public string ItemType = null;
        public string UsedBy = null;
    }


    /// <summary>
    /// Fault thrown when an item cannot be deleted because it is in use by another item.
    /// </summary>

    [DataContract]
    public class ItemInUseFault : BasicFault
    {
        public ItemInUseFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// The name of the type of item being deleted.
        /// </summary>

        [DataMember]
        public string ItemType = null;

        /// <summary>
        /// The name of the type that is referencing the item.
        /// </summary>

        [DataMember]
        public string UsedBy = null;

        /// <summary>
        /// Path to the item that is in use.
        /// </summary>
       
        [DataMember]
        public string ItemPath { get; set; }
    }
}
