using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO used to point out an error in an (page/link) attribute.
    /// </summary>

    [DataContract]
    public class AttributeFormatError
    {
        /// <summary>
        /// Attribute containing an error (key and/or value).
        /// </summary>
        
        [DataMember]
        public DTO.AttributeItem Attribute { get; set; }

        /// <summary>
        /// Error string.
        /// </summary>

        [DataMember]
        public string Error { get; set; }
    }
}
