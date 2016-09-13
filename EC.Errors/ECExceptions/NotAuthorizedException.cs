using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when a user is not authorized to view a page.
    /// </summary>

    public class NotAuthorizedException : FaultableException<NotAuthorizedFault>
    {
        public override NotAuthorizedFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NotAuthorizedFault(Message, path, userInfo);
            f.CapabilitiesNeeded = CapabilitiesNeeded;
            f.ItemPath = ItemPath;
            return f;
        }

        public NotAuthorizedException(string path, string msg) : base(msg)
        {
            ItemPath = path;
        }

        public NotAuthorizedException(string path, IList<string> capabilitiesNeeded, string msg) : base(msg)
        {
            ItemPath = path;
            if (capabilitiesNeeded == null) { return; }
            if (capabilitiesNeeded.Count() > 0) { CapabilitiesNeeded = string.Join(",", capabilitiesNeeded); }
        }

        public override string Message
        {
	        get 
	        {
                string msg = base.Message;
                if (!string.IsNullOrWhiteSpace(ItemPath)) { msg += string.Format(", Item = {0}", ItemPath); }
                if (!string.IsNullOrWhiteSpace(CapabilitiesNeeded)) { msg += string.Format(", Capabilities needed = {0}", CapabilitiesNeeded); }
                return msg;
	        }
        }

        /// <summary>
        /// Path where error occurred
        /// </summary>

        public string ItemPath { get; set; }

        /// <summary>
        /// Capability that was required to carry out action (and was possessed by the user)
        /// </summary>

        public string CapabilitiesNeeded { get; set; }
    }

    /// <summary>
    /// Fault returned if the access to a content item is rejected.
    /// </summary>

    [DataContract]
    public class NotAuthorizedFault : BasicFault
    {
        public NotAuthorizedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Indicates which capabilities were needed to carry out the action.
        /// </summary>
        
        [DataMember]
        public string CapabilitiesNeeded { get; set; }

        [DataMember]
        public string ItemPath { get; set; }
    }
}
