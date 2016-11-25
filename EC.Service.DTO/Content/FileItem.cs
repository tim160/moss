using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class FileItem
    {
        [DataMember]
        public bool IsDirectory { get; set; }

        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public DateTime ModifiedDate { get; set; }
    }
}
