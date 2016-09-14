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
    /// Thrown if a parameter is not correct.
    /// </summary>

    public class ParameterValidationException : FaultableException<ParameterValidationFault>
    {
        public override ParameterValidationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ParameterValidationFault(Message, path, userInfo);
            f.ParameterName = ParameterName;
            f.ValidationError = ValidationError;
            return f;
        }

        public ParameterValidationException(string paramName, string validationError) : base(validationError ?? "Parameter validation error")
        {
            ParameterName = paramName;
            ValidationError = validationError;
        }

        public string ParameterName { get; set; }
        public string ValidationError { get; set; }
    }

    /// <summary>
    /// Fault used to indicate a problem with parameters.
    /// </summary>

    [DataContract]
    public class ParameterValidationFault : BasicFault
    {
        public ParameterValidationFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string ParameterName { get; set; }

        [DataMember]
        public string ValidationError { get; set; }
    }
}
