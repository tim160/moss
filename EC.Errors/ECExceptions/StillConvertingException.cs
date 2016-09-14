using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// This exception indicated that the file is already being converted
    /// </summary>

    public class StillConvertingException : FaultableException<StillConvertingFault>
    {
        public override StillConvertingFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new StillConvertingFault(Message, path, userInfo);
            f.Item = Item;
            return f;
        }

        public StillConvertingException(string message, string item, Exception innerException = null)
            : base(message, innerException)
        {
            this.Item = item;
        }

        /// <summary>
        /// Location of the File that is still being converted
        /// </summary>
        
        public string Item { get; set; }
    }

    /// <summary>
    /// Fault returned when still in the process of converting
    /// a Video
    /// </summary>

    [DataContract]
    public class StillConvertingFault : BasicFault
    {
        public StillConvertingFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path to the video file that is still being converted.
        /// </summary>
        
        [DataMember]
        public string Item { get; set; }
    }
}
