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
    /// Exception thrown if a string contains white spaces.
    /// </summary>

    public class ContainsWhiteSpacesException : FaultableException<ContainsWhiteSpacesFault>
    {
        public override ContainsWhiteSpacesFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ContainsWhiteSpacesFault(Message, path, userInfo);
            f.Value = Value;
            return f;
        }

        public ContainsWhiteSpacesException(string message, string value) : base(message)
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    [DataContract]
    public class ContainsWhiteSpacesFault : BasicFault
    {
        public ContainsWhiteSpacesFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Value which contains white spaces.
        /// </summary>

        [DataMember]
        public string Value { get; set; }
    }
}
