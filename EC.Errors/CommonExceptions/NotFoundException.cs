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
    /// Exception thrown when an item cannot be found.
    /// </summary>

    public class NotFoundException : FaultableException<NotFoundFault>
    {
        public override NotFoundFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NotFoundFault(Message, path, userInfo);
            f.ID = Id.HasValue ? Id.Value : Guid.Empty;
            f.Name = Name;
            return f;
        }

        public NotFoundException(string msg, Type typeOfItem) : base(msg)
        {
            TypeOfItem = typeOfItem;
        }

        public NotFoundException(string msg, string name, Type typeOfItem) : base(msg)
        {
            Name = name;
            TypeOfItem = typeOfItem;
        }

        public NotFoundException(string msg, Guid id, Type typeOfItem) : base(msg)
        {
            Id = id;
            TypeOfItem = typeOfItem;
        }

        public NotFoundException(string msg, string name, Guid id, Type typeOfItem) : base(msg)
        {
            Name = name;
            Id = id;
            TypeOfItem = typeOfItem;
        }

        public override string Message
        {
            get 
            {
                string msg = base.Message;
                if (Name != null) { msg += string.Format(", Name = {0}", Name); }
                if (Id.HasValue) { msg += string.Format(", Id = {0}", Id.Value); }
                return msg;
            }
        }

        /// <summary>
        /// Name of the item that could not be found. May be null, if null then
        /// the ID should not be null.
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// ID of item that cannot be found. May be null. If null, the name
        /// should not be null.
        /// </summary>

        public Guid? Id { get; set; }

        /// <summary>
        /// The type of item that was being looked for.
        /// </summary>

        public Type TypeOfItem { get; set; }
    }

    /// <summary>
    /// Fault returned when an item cannot be found.
    /// </summary>

    [DataContract]
    public class NotFoundFault : BasicFault
    {
        public NotFoundFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
