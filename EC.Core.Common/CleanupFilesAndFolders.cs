using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// This container holds a list of all files and folders which may have to be removed
    /// in case of an error during file/question import.
    /// The class is registered at IOC with lifestyle 'LifestylePerWcfOperation'.
    /// <remarks>
    /// Don't register with IOC by adding any attributes! It is manually registered somewhere else.
    /// </remarks>
    /// </summary>

    [TransientType]
    [RegisterAsType(typeof(ICleanupFilesAndFolders))]
    
    public class CleanupFilesAndFolders : ICleanupFilesAndFolders
    {
        public void AddEntry(string fileOrFolder)
        {
            if (string.IsNullOrWhiteSpace(fileOrFolder) || (this.Entries.Contains(fileOrFolder)))
            {
                return;
            }
            this.Entries.Add(fileOrFolder);
        }

        public void RemoveEntry(string fileOrFolder)
        {
            if (string.IsNullOrWhiteSpace(fileOrFolder))
            {
                return;
            }
            this.Entries.Remove(fileOrFolder);
        }

        public void Clear()
        {
            this.Entries.Clear();
        }

        public IQueryable<string> GetAllEntries()
        {
            return this.Entries.AsQueryable<string>();
        }

        public bool HasEntries()
        {
            return (this.Entries.Count > 0);
        }

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

        public bool Cleanup()
        {
            if (this.Entries.Count == 0)
            {
                return true;
            }

            bool hasError = false;
            var allEntries = this.Entries.ToList();
            foreach (string item in allEntries)
            {
                try
                {
                    if (this.fileAccess.DirectoryExists(item) || this.fileAccess.FileExists(item)) 
                    {
                        if (this.fileAccess.IsDirectory(item))
                        {
                            // Directory...
                            this.fileAccess.DeleteDirectory(item, true);
                            this.logger.InfoFormat("[Cleanup] - deleted directory [{0}].", item);
                        }
                        else
                        {
                            // File...
                            this.fileAccess.DeleteFile(item);
                            this.logger.InfoFormat("[Cleanup] - deleted file [{0}].", item);
                        } 
   
                        // Remove deleted item from list...
                        this.Entries.Remove(item); 
                    }
                    else 
                    {
                        // if the item doesn't exist ... log it and remove it.
                        this.logger.WarnFormat("[Cleanup] - item [{0}] doesn't exist. The item is removed from the list without action.", item);
                        this.Entries.Remove(item);
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                    this.logger.WarnFormat("[Cleanup] - error removing [{0}]: {1}", item, ex.Message);
                }
            }

            return !hasError;
        }

        /// <summary>
        /// Constructor used by IOC.
        /// </summary>
        public CleanupFilesAndFolders(ILogger l, IFileAccess fa)
        {
            this.logger = l;
            this.Entries = new List<string>();
            this.fileAccess = fa;
        }

        private IList<string> Entries { get; set; }
        private ILogger logger = null;
        private IFileAccess fileAccess = null;
    }
}
