using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Various constants for files (i.e. file extensions, file lists not to copy,...).
    /// </summary>

    public static class FileConstants
    {
        /// <summary>
        /// List of files which should be excluded/not copied.
        /// </summary>

        public static readonly IList<string> ExcludedFiles = new List<string>() { "Thumbs.db" };     // Windows thumbnail cache

    }
}
