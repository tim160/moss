using System;
using System.Runtime.Serialization;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception used to indicate that a course rule has a malformed format.
    /// </summary>

    public class RuleFormatException : FaultableException<RuleFormatFault>
    {
        public override RuleFormatFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new RuleFormatFault(Message, path, userInfo);
            f.RuleExpression = RuleExpression;
            return f;
        }

        public RuleFormatException(string msg, string ruleExpression, Exception innerException = null)
            : base(msg, innerException)
        {
            RuleExpression = ruleExpression;
        }

        /// <summary>
        /// Malformed rule expression.
        /// </summary>

        public string RuleExpression { get; set; }
    }

    /// <summary>
    /// Exception used to indicate that the format is wrong.
    /// </summary>

    [DataContract]
    public class RuleFormatFault : BasicFault
    {
        public RuleFormatFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Malformed rule expression.
        /// </summary>

        [DataMember]
        public string RuleExpression { get; set; }
    }
}