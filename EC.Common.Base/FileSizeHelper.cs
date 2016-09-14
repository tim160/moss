using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Base
{
    public static class FileSizeHelper
    {
        /// <summary>
        /// Convert <paramref name="fileSize"/> to its biggest representation (i.e. 1024bytes will return 1KB)
        /// </summary>
        /// <param name="fileSize">File size in bytes</param>
        /// <returns>Return converted <paramref name="fileSize"/> with unit (i.e. 12 MB)</returns>

        public static string FormatFileSize(long fileSize)
        {
            string[] sizes = { "Bytes", "KB", "MB", "GB" };

            float len = fileSize;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }
    }
}
