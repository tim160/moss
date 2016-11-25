using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service.DTO
{
    [DataContract]
    public enum UserRelationshipType
    {
        [EnumMember]
        Supervisee,

        [EnumMember]
        Supervisor,

        [EnumMember]
        Mentee,

        [EnumMember]
        Mentor,
    }
}
