using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class UserRelationships
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public List<Guid> Supervisees { get; set; }

        [DataMember]
        public List<Guid> Supervisors { get; set; }

        [DataMember]
        public List<Guid> Mentees { get; set; }

        [DataMember]
        public List<Guid> Mentors { get; set; }
    }
}
