using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Data structure used to return information about a file/directory
    /// on disk.
    /// </summary>
    /// <remarks>
    /// Because they have <c>CreatedDate</c> and <c>ModifiedDate</c> but are transient
    /// the class doesn't need to inherit from <c>IModifiableItem</c>
    /// </remarks>

    public interface IFileItem
    {
        /// <summary>
        /// Flag whether this item is a file (<c>false</c>) or directory (<c>true</c>).
        /// </summary>

        bool IsDirectory { get; set; }

        /// <summary>
        /// File or directory name (just the name, no full path).
        /// </summary>

        string Name { get; set; }

        /// <summary>
        /// File or directory name with full path
        /// </summary>
        
        string FullName { get; set; }

        /// <summary>
        /// UTC date when file/directory has been created.
        /// This does not need SysTime property as it always reads from the file system timestamp
        /// </summary>

        DateTime CreatedDate { get; set; }

        /// <summary>
        /// UTC date when the file/directory has been modified last.
        /// This does not need SysTime property as it always reads from the file system timestamp
        /// </summary>

        DateTime ModifiedDate { get; set; }
    }
}
