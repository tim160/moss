using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// This container holds a list of all files and folders which may have to be removed
    /// in case of an error during file/question import.
    /// </summary>

    public interface ICleanupFilesAndFolders
    {
        /// <summary>
        /// Add new file or folder (must include an absolute path)
        /// </summary>
        /// <param name="fileOrFolder">File or folder path (absolute path)</param>

        void AddEntry(string fileOrFolder);

        /// <summary>
        /// Remove a file or folder from the list.
        /// </summary>
        /// <param name="fileOrFolder"></param>

        void RemoveEntry(string fileOrFolder);

        /// <summary>
        /// Remove all existing entries.
        /// </summary>

        void Clear();

        /// <summary>
        /// Get all entries as queryable.
        /// </summary>
        /// <returns>Return queryable of all entries.</returns>

        IQueryable<string> GetAllEntries();

        /// <summary>
        /// Check whether there are any entries in the list.
        /// </summary>
        /// <returns>Return <c>true</c> if entries are in the list. Return <c>false</c> if no entry exists.</returns>

        bool HasEntries();

        /// <summary>
        /// Delete all files and folders which have been added.
        /// Ignore not existing files - just remove them from the list.
        /// Log every error.
        /// </summary>
        /// <remarks>
        /// No exception is thrown - this can be safely used outside try-catch blocks.
        /// </remarks>
        /// <returns>
        /// Return <c>true</c> if all items could have been removed. 
        /// Return <c>false</c> on error (e.g. no access to a file/folder).
        /// Entries which were unable to be removed will remain in the entries list.
        /// </returns>

        bool Cleanup();
    }
}
