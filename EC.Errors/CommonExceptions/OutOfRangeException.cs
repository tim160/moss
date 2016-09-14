using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Thrown if a value is out of range (i.e. index, offset,...).
    /// </summary>

    public class OutOfRangeException : FaultableException<OutOfRangeFault>
    {
        public override OutOfRangeFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new OutOfRangeFault(Message, path, userInfo);
            return f;
        }

        public OutOfRangeException() : base("Value is out of range.") { }
        public OutOfRangeException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Fault used to indicate that a value is out of range (i.e. index, offset of a file,...).
    /// </summary>

    [DataContract]
    public class OutOfRangeFault : BasicFault
    {
        public OutOfRangeFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo) { }
    }
}
