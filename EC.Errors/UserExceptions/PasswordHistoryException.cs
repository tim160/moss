using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown when the password history rule is violated.
    /// </summary>

    public class PasswordHistoryException : FaultableException<PasswordHistoryFault>
    {
        public override PasswordHistoryFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new PasswordHistoryFault(Message, reqPath, userInfo)
            {
                HistoryRule = HistoryRule,
                ValidationError = ValidationError
            };
            return f;
        }

        public PasswordHistoryException(PasswordErrors historyRule, string validationError)
            : base(validationError ?? "Password history rule violated")
        {
            HistoryRule = historyRule;
            ValidationError = validationError;
        }

        public PasswordErrors HistoryRule { get; set; }
        public string ValidationError { get; set; }
    }


    /// <summary>
    /// Fault which indicates when the password history rule is violated.
    /// </summary>

    [DataContract]
    public class PasswordHistoryFault : BasicFault
    {
        public PasswordHistoryFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public PasswordErrors HistoryRule { get; set; }

        [DataMember]
        public string ValidationError { get; set; }
    }
}
