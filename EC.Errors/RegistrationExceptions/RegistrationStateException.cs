using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// Exception used to indicate an invalid registration state
    /// </summary>

    public class RegistrationStateException : FaultableException<RegistrationStateFault>
    {
        public override RegistrationStateFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new RegistrationStateFault(Message, path, userInfo);
            f.CurrentState = CurrentState;
            f.NewState = NewState;
            f.RegistrationId = RegistrationId;
            return f;
        }

        public RegistrationStateException(string msg, string currentState, string newState, Guid registartionId) : base(msg)
        {
            CurrentState = currentState;
            NewState = newState;
            RegistrationId = registartionId;
        }

        public string CurrentState { get; set; }
        public string NewState { get; set; }
        public Guid RegistrationId { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an invalid registration state
    /// </summary>
   
    [DataContract]
    public class RegistrationStateFault : BasicFault
    {
        public RegistrationStateFault(string msg, string path, CurrentUserInfo userInfo)  : base(msg, path, userInfo)
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
        public Guid RegistrationId { get; set; }
    }
}
