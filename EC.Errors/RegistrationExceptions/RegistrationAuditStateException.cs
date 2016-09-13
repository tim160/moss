using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// Exception used to indicate an invalid registration audit state
    /// </summary>

    public class RegistrationAuditStateException : FaultableException<RegistrationAuditStateFault>
    {
        public override RegistrationAuditStateFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new RegistrationAuditStateFault(Message, path, userInfo);
            f.CurrentState = CurrentState;
            f.NewState = NewState;
            f.RegistrationAuditId = RegistrationAuditId;
            return f;
        }

        public RegistrationAuditStateException(string msg, string currentState, string newState, Guid registartionAuditId) : base(msg)
        {
            CurrentState = currentState;
            NewState = newState;
            RegistrationAuditId = registartionAuditId;
        }

        public string CurrentState { get; set; }
        public string NewState { get; set; }
        public Guid RegistrationAuditId { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an invalid registration audit state
    /// </summary>
    
    [DataContract]
    public class RegistrationAuditStateFault : BasicFault
    {
        public RegistrationAuditStateFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// State RegistartionAudit was in when trying to change it
        /// </summary>
        
        [DataMember]
        public string CurrentState { get; set; }

        /// <summary>
        /// State we tried to change to 
        /// </summary>
        
        [DataMember]
        public string NewState { get; set; }

        /// <summary>
        /// Id of the registration audit we tried to change
        /// </summary>
        
        [DataMember]
        public Guid RegistrationAuditId { get; set; }
    }
}
