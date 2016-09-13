using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Thrown when the validate function on a Model object fails
    /// </summary>

    public class ModelValidationException : FaultableException<ModelValidationFault>
    {
        public override ModelValidationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ModelValidationFault(Message, path, userInfo);
            f.PropertyName = PropertyName;
            return f;
        }

        public ModelValidationException(string propertyName, string message, Exception innerException = null) : base(message, innerException)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
    }

    /// <summary>
    /// Fault thrown when one type of content item is needed, but the content item provided
    /// is of a different type.
    /// </summary>

    [DataContract]
    public class ModelValidationFault : BasicFault
    {
        public ModelValidationFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string PropertyName { set; get; }

    }
}
