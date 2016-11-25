using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// Details about a File.
    /// Retrieves information from the file system about a file.
    /// </summary>
    
    [DataContract]
    public class FileDetails
    {
        
        /// <summary>
        /// The Name of the File
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Absolute path incl. file name where the file is located on the file system.
        /// </summary>
        /// <remarks>
        /// Important: this will break streaming videos on Core-Web as soon as the Web and Service installations
        /// are on separate servers!
        /// </remarks>
        
        [DataMember]
        public string AbsoluteFileName { get; set; }

        /// <summary>
        /// The Content Length of the File in Bytes
        /// </summary>
        [DataMember]
        public long ContentLength { get; set; }

        /// <summary>
        /// The Last date the File was modified
        /// </summary>
        [DataMember]
        public DateTime DateModified { get; set; }
    }
}
