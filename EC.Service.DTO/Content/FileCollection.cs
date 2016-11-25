using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class FileCollection : BasicItem
    {
        /// <summary>
        /// Absolute path for the location on disk where this repository has 
        /// its contents.
        /// </summary>

        [DataMember]
        public string RootPath { get; set; }

    }
}