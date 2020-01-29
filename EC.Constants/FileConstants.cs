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
        public static readonly IList<string> ExtensionFiles = new List<string>() {
            //excel
            ".xlsx",
            ".xls",
            ".csv",
            //outlook
            ".pst",
            //word
            ".doc",
            ".docx",
            //music video
            ".mp3",
            ".wav",
            ".mp4",
            ".mov",
            ".MPEG",
        };
        public static readonly IList<string> MimeTypes = new List<string>() {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel",
            "application/vnd.oasis.opendocument.spreadsheet",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
            "application/pdf",
            "application/msword",
            "audio/*",
            "image/*",
        };
    }
}
