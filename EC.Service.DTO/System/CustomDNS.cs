using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class CustomDNS
    {
        [DataMember]
        public string IncomingDNS { get; set; }

        [DataMember]
        public string TargetDNS { get; set; }

        [DataMember]
        public string DefaultPath { get; set; }

        [DataMember]
        public bool SSL { get; set; }

        /// <summary>
        /// The org path for this custom DNS entry. Used for organizational level login. 
        /// </summary>
        
        [DataMember]
        public string OrgPath { get; set; }

        [DataMember]
        public bool Deleted { get; set; }
    }
}
