using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EC.Constants;

namespace EC.Service.DTO
{
    [DataContract]
    public class TagMapping
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public TagSet Source { get; set; }

        [DataMember]
        public TagSet Target { get; set; }


        [DataMember]
        public ModelState State { get; set; }

        [DataMember]
        public List<MappedTagPair> MappedTagPairs { get; set; }
    }

    [DataContract]
    public class MappedTagPair
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Tag SourceTag { get; set; }

        [DataMember]
        public Tag TargetTag { get; set; }

        [DataMember]
        public TagMapping Owner { get; set; }

        [DataMember]
        public ModelState State { get; set; }

    }
}
