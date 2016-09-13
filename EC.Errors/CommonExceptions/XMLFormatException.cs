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
    /// Exception thrown when there is an error parsing an XML file
    /// </summary>

    public class XMLFormatException : FaultableException<XMLFormatFault>
    {
        public override XMLFormatFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new XMLFormatFault(Message, path, userInfo);
            f.Attribute = Attribute;
            f.Element = Element;
            return f;
        }

        public XMLFormatException(string element, string attr, string message, Exception innerException = null) : base(message, innerException)
        {
            Element = element;
            Attribute = attr;
        }

        public string Element { get; set; }
        public string Attribute { get; set; }
    }

    /// <summary>
    /// Fault thrown when there is a format error in an XML file.
    /// </summary>

    [DataContract]
    public class XMLFormatFault : BasicFault
    {
        public XMLFormatFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string Element;

        [DataMember]
        public string Attribute;
    }
}
