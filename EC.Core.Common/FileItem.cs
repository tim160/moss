using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Transient data structure used to return information about a file/directory
    /// on disk.
    /// </summary>
    /// <remarks>
    /// Because they have <c>CreatedDate</c> and <c>ModifiedDate</c> but are transient
    /// the class doesn't need to inherit from <c>IModifiableDateTimeItem</c>
    /// </remarks>

    [TransientType]
    [RegisterAsType(typeof(IFileItem))]

    public class FileItem : IFileItem
    {
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
