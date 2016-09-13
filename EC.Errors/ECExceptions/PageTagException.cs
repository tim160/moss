using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Constants;
using System.Runtime.Serialization;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown if a tagged item (e.g. page) has not been found.
    /// </summary>

    public class PageTagException : FaultableException<PageTagFault>
    {
        public override PageTagFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new PageTagFault(Message, reqPath, userInfo);
            f.PagePath = PagePath;
            f.Tag = Tag;
            return f;
        }

        public PageTagException(string msg, NavPageTagTypesEnum tag, Exception innerException = null) : base(msg, innerException)
        {
            Tag = tag;
        }

        public PageTagException(string msg, string path, NavPageTagTypesEnum tag) : base(msg)
        {
            PagePath = path;
            Tag = tag;
        }

        /// <summary>
        /// Path from where it was started to look for the Course.
        /// </summary>

        public string PagePath { get; set; }

        /// <summary>
        /// Tag which was not found.
        /// </summary>

        public NavPageTagTypesEnum Tag { get; set; }
    }

    /// <summary>
    /// Fault thrown if a tagged item (e.g. page) has not been found.
    /// </summary>

    [DataContract]
    public class PageTagFault : BasicFault
    {
        public PageTagFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string PagePath { get; set; }

        [DataMember]
        public NavPageTagTypesEnum Tag { get; set; }
    }
}
