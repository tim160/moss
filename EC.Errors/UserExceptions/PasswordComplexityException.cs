using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown when a password complexity rule is violated.
    /// </summary>

    public class PasswordComplexityException : FaultableException<PasswordComplexityFault>
    {
        public override PasswordComplexityFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new PasswordComplexityFault(Message, reqPath, userInfo)
            {
                ComplexityRule = ComplexityRule,
                ErrorParameter = ErrorParameter,
                ErrorCategory = ErrorCategory
            };
            return f;
        }

        public PasswordComplexityException(PasswordErrors complexityRule, int errorParameter, string validationError) : base(validationError)
        {
            ComplexityRule = complexityRule;
            ErrorParameter = errorParameter;
            ErrorCategory = validationError;
        }

        /// <summary>
        /// Error string used in log files.
        /// </summary>
        
        public override string Message
        {
            get
            {
                var msg = base.Message;

                switch(ComplexityRule)
                {
                    case PasswordErrors.PASSWORD_HISTORY_ENTRIES: msg += String.Format(" - password history violation."); break;
                    case PasswordErrors.PASSWORD_MINIMUM_LENGTH: msg += String.Format(" - minimum length violation, must be at least {0} chars", ErrorParameter); break;
                    case PasswordErrors.PASSWORD_MINIMUM_LOWERCASE_CHARS: msg += String.Format(" - min lowercase violation, must have at least {0}", ErrorParameter); break;
                    case PasswordErrors.PASSWORD_MINIMUM_NUMERIC_CHARS: msg += String.Format(" - min numeric violation, must have at least {0}", ErrorParameter); break;
                    case PasswordErrors.PASSWORD_MINIMUM_SYMBOL_CHARS: msg += String.Format(" - min symbol violation, must have at least {0}", ErrorParameter); break;
                    case PasswordErrors.PASSWORD_MINIMUM_UPPERCASE_CHARS: msg += String.Format(" - min uppercase violation, must have at least {0}", ErrorParameter); break;
                    default: return msg;
                }

                return msg;
            }
        }

        public PasswordErrors ComplexityRule { get; set; }
        public int ErrorParameter { get; set; }
        public string ErrorCategory { get; set; }
    }


    /// <summary>
    /// Fault which indicates when a password complexity rule is violated.
    /// </summary>

    [DataContract]
    public class PasswordComplexityFault : BasicFault
    {
        public PasswordComplexityFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public PasswordErrors ComplexityRule { get; set; }

        [DataMember]
        public int  ErrorParameter { get; set; }

        [DataMember]
        public string ErrorCategory { get; set; }
    }
}
