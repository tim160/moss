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
    /// Exception thrown if an item can't be created (e.g. if a DTO has an Id assigned where 
    /// it should be <c>null</c>).
    /// </summary>

    public class CannotCreateItemException : FaultableException<CannotCreateItemFault>
    {
        public override CannotCreateItemFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CannotCreateItemFault(Message, path, userInfo);
            f.Id = Id;
            f.ItemType = TypeOfItem.ToString();
            return f;
        }

        public CannotCreateItemException(string message, Guid? id, Type typeOfItem) : base(message)
        {
            this.Id = id;
            this.TypeOfItem = typeOfItem;
        }

        public CannotCreateItemException(string message, Guid? id, Type typeOfItem, Exception inner) : base(message, inner)
        {
            Id = id;
            TypeOfItem = typeOfItem;
        }

        public Guid? Id { get; set; }
        public Type TypeOfItem { get; set; }
    }

    /// <summary>
    /// Exception thrown if an item can't be created (e.g. if a DTO has an Id assigned where 
    /// it should be <c>null</c>).
    /// </summary>

    [DataContract]
    public class CannotCreateItemFault : BasicFault
    {
        public CannotCreateItemFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid? Id { get; set; }

        [DataMember]
        public string ItemType { get; set; }
    }
}
