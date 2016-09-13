using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.NotificationExceptions
{
    /// <summary>
    /// Exception thrown when a notification target uses a non-existent channel name.
    /// </summary>

    public class BadChannelException : FaultableException<BadChannelFault>
    {
        public override BadChannelFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new BadChannelFault(Message, reqPath, userInfo);
            f.ChannelName = ChannelName;
            return f;
        }

        public BadChannelException(string msg, string channelName) : base(msg)
        {
            ChannelName = channelName;
        }

        public string ChannelName = null;
    }    
    
    /// <summary>
    /// Fault thrown when a notification target uses a non-existent channel name.
    /// </summary>

    [DataContract]
    public class BadChannelFault : BasicFault
    {
        public BadChannelFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Channel name that was not found
        /// </summary>

        [DataMember]
        public string ChannelName = null;
    }

}
