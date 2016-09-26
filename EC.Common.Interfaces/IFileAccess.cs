using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EC.Common.Base;
using System.Runtime.InteropServices.ComTypes;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Interface wrapper around system calls for file access.
    /// </summary>

    public interface IFileAccess
    {
        /// <summary>
        /// Read chunk of file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="offset"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        /// <exception cref="OutOfRangeException">
        /// If the file offset is less than 0. 
        /// If the file chunk size is less than 0. 
        /// If <paramref name="offset"/> + <paramref name="chunkSize"/> is greater or equal to the file size</exception>
        /// <exception cref="FolderOrFileNotFoundException">If the file doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file can't be accessed.</exception>

        byte[] ReadFileChunk(string filePath, long offset, int chunkSize);

        /// <summary>
        /// Write a chunk of file. The offset must be smaller/equal to the current file size - no writing outside the file size possible.
        /// </summary>
        /// <remarks>If the file doesn't exist - create it</remarks>
        /// <param name="filePath">Full path and file name</param>
        /// <param name="offset">Offset where to write the <paramref name="chunk"/></param>
        /// <param name="chunk">Chunk of bytes to write</param>
        /// <returns>Next offset position</returns>
        /// <exception cref="FileWriteException">If the file couldn't be written.</exception>

        long WriteFileChunk(string filePath, long offset, byte[] chunk);

        /// <summary>
        /// Get file size of the file <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Path incl. file name</param>
        /// <returns>Return size of the file.</returns>
        /// <exception cref="FolderOrFileNotFoundException">If the file doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file can't be accessed.</exception>

        long GetFileSizeInBytes(string filePath);

        /// <summary>
        /// Check whether the given path refers to a file or a directory.
        /// </summary>
        /// <param name="path">path to check</param>
        /// <returns>true if the path is a directory, false otherwise</returns>
        ///<exception cref="FileAccessException">if any System.IO exception occurs</exception>
        
        bool IsDirectory(string path);

        /// <summary>
        /// Check whether the path is rooted (absolute path).
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Return <c>true</c> if the <paramref name="path"/> is rooted. <c>false</c> otherwise.</returns>

        bool IsPathRooted(string path);

        /// <summary>
        /// Get the name of the last element referred to by the given path, regardless
        /// of whether it is a directory or a file.
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>the name of the last item in the path</returns>
        /// <exception cref="FileAccessException">if any System.IO exception occurs</exception>
        
        string GetNameFromPath(string path);

        /// <summary>
        /// Get the parent directory (absolute path) of a <paramref name="path"/> (either file or directory).
        /// </summary>
        /// <example>
        /// <paramref name="path"/> = c:\marinelms\content\filecollection
        /// Return value is c:\marinelms\content
        /// <paramref name="path"/> = c:\
        /// Return value is c:\
        /// <paramref name="path"/> = c:\marinlms\web\web.config
        /// Return value is c:\marinelms\web
        /// </example>
        /// <param name="path">Path to a directory or file (must exist)</param>
        /// <returns>Return the parent directory path. Return <c>null</c> the <paramref name="path"/> is the root path</returns>
        /// <exception cref="DirectoryDoesNotExistException">If the directory <paramref name="path"/> is a directory and doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file doesn't exist or on any other error</exception>

        string GetParentDirectory(string path);

        /// <summary>
        /// Get the creation date (in UTC) for the last item in the given path.
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>create date (UTC)</returns>
        ///<exception cref="FileAccessException">if any System.IO exception occurs</exception>
       
        DateTime GetCreationDate(string path);

        /// <summary>
        /// Get the last modification date (in UTC) for the last item in the given path.
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>modification date (UTC)</returns>

        DateTime GetModificationDate(string path);

        /// <summary>
        /// Gets the length of the file in bytes. If a directory returns 0
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>length of file in bytes</returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        long GetLength(string path);
       
        /// <summary>
        /// Return a stream via which the contents of a file can be read.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>
        /// <returns>the stream</returns>

        Stream OpenRead(string path);

        /// <summary>
        /// Read the contents of the specified file and return them as a string.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        string ReadFileAsString(string path);

        /// <summary>
        /// Create a StreamReader for the given file.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>
        /// <returns>the StreamReader</returns>

        StreamReader CreateStreamReader(string path, Encoding encoding = null);

        /// <summary>
        /// Empty the contents of the specified file.
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <exception cref="FileWriteException">If there is any problem emptying the contents</exception>

        void EmptyFile(string path);

        /// <summary>
        /// Create or overwrite the given file with the given contents. This will create
        /// directories as needed.
        /// </summary>
        /// <param name="path">full absolute path to the file</param>
        /// <param name="contents">data to write to file</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileWriteException">thrown if the file cannot be written to</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown if <paramref name="overwriteExistingFile"/> == false and the file already exists.</exception>

        void WriteFile(string path, byte[] contents, bool overwriteExistingFile = true);

        /// <summary>
        /// Create or overwrite the given file with the given contents. This will create
        /// directories as needed.
        /// </summary>
        /// <param name="path">Full absolute path to the file.</param>
        /// <param name="contentStream">data to write to file</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileWriteException">thrown if the file cannot be written to</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown if <paramref name="overwriteExistingFile"/> == false and the file already exists.</exception>
        
        void WriteFile(string path, Stream contentStream, bool overwriteExistingFile = true);

        /// <summary>
        /// Move the file from source to target. Optionally replace an already existing file.
        /// This will create directories as needed.
        /// </summary>
        /// <param name="sourceFileName">Full path and file name of the source file.</param>
        /// <param name="targetFileName">Full path and file name of the target file to put the source.</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileNotFoundException">If the <paramref name="sourceFileName"/> doesn't exist.</exception>
        /// <exception cref="FileWriteException">If the file can't be moved.</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown if <paramref name="overwriteExistingFile"/> == false and the file already exists.</exception>

        void MoveFile(string sourceFileName, string targetFileName, bool overwriteExistingFile = true);

        /// <summary>
        /// Move a file into a specific directory. Optionally replace an already existing file.
        /// </summary>
        /// <param name="sourceFileName">Full path and file name of the source file.</param>
        /// <param name="targetDirectory">Full path of the target directory to move the file into.</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileNotFoundException">If the <paramref name="sourceFileName"/> doesn't exist.</exception>
        /// <exception cref="FileWriteException">If the file can't be moved.</exception>
        /// <exception cref="FileAlreadyExistsException">if the file already exists and overwriteExistingFile is false</exception>

        void MoveFileToDirectory(string sourceFileName, string targetDirectory, bool overwriteExistingFile = true);

        /// <summary>
        /// Move the directory from source to target.
        /// This will create directories as needed.
        /// </summary>
        /// <param name="sourceDirectory">Full path of the source directory.</param>
        /// <param name="targetDirectory">Full path of the target directory where to put the source.</param>
        /// <exception cref="DirectoryDoesNotExistException">If the <paramref name="sourceDirectory"/> doesn't exist.</exception>
        /// <exception cref="FileAccessException">On any other error (i.e. the <paramref name="sourceDirectory"/> and <paramref name="targetDirectory"/> are on different drives)</exception>

        void MoveDirectory(string sourceDirectory, string targetDirectory);
        
        /// <summary>
        /// Create a new directory with the given path.
        /// </summary>
        /// <remarks>
        /// If parts or all of the directory already exists - do nothing.
        /// </remarks>
        /// <param name="path">full absolute path for new directory</param>
        /// <exception cref="DirectoryCreationException">thrown when the directory cannot be created</exception>

        void CreateDirectory(string path);

        /// <summary>
        /// Delete a file from disk.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>
        /// <exception cref="FileDeletionException">thrown if the file cannot be deleted</exception>
        
        void DeleteFile(string path);

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <exception cref="FileDeletionException"></exception>

        void DeleteDirectory(string path, bool recursive);

        /// <summary>
        /// Deletes the directory's contents, but not the directory itself.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="FileDeletionException"></exception>
        /// <exception cref="FileAccessException"></exception>

        void DeleteDirectoryContents(string path);

        /// <summary>
        /// Check if the file exists.
        /// </summary>
        /// <param name="path">Path incl. file name.</param>
        /// <returns>Return <c>true</c> if the file exists. Return <c>false</c> if the file doesn't exist.</returns>
        /// <exception cref="FileAccessException">Thrown if the file can't be accessed or on any other error.</exception>
        
        bool FileExists(string path);

        /// <summary>
        /// Check if the directory exists.
        /// </summary>
        /// <param name="path">Directory path.</param>
        /// <returns>Return <c>true</c> if the directory exists. Return <c>false</c> if the directory doesn't exist.</returns>
        /// <exception cref="FileAccessException">Thrown if the directory can't be accessed or on any other error.</exception>

        bool DirectoryExists(string path);

        /// <summary>
        /// Get file name incl. file extension from full path.
        /// </summary>
        /// <param name="path">file name (incl. path)</param>
        /// <returns>Return file name incl. file extension.</returns>
        /// <exception cref="FileAccessExecption">Thrown in any case of error.</exception>

        string GetFileName(string path);

        /// <summary>
        /// Get directory name of <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="path"/> contains a file name - the file name is ignored.
        /// </remarks>
        /// <param name="path">Directory path (may include a file name) to get the directory name from</param>
        /// <returns>Return directory name</returns>
        /// <exception cref="FileAccessException">Thrown in any other case of error.</exception>

        string GetDirectoryName(string path);

        /// <summary>
        /// Get directory information (absolute path) of the specified path string.
        /// </summary>
        /// <remarks>If the <paramref name="path"/> is already a directory - return the same <paramref name="path"/></remarks>
        /// <param name="path">Path (might include a file name)</param>
        /// <returns>
        /// Return directory name (absolute path).
        /// </returns>
        /// <exception cref="FileAccessException">If the path can't be accessed or the path is wrong</exception>

        string GetDirectory(string path);

        /// <summary>
        ///  Get file name incl. file extension from full path.
        /// </summary>
        /// <param name="path">file name (incl. path)</param>
        /// <returns>Return file name without the file extension.</returns>
        /// <exception cref="FileAccessExecption">Thrown in any case of error.</exception>

        string GetFileNameWithoutExtension(string path);

        /// <summary>
        /// Combine two paths. If the second path is not relative (i.e. begins with
        /// a '/' or a '\'), then those characters a stripped off.
        /// </summary>
        /// <param name="path1">first path</param>
        /// <param name="path2">second path</param>
        /// <returns><paramref name="path1"/> + <paramref name="path2"/>. Return path1 if <paramref name="path2"/> is an empty string.</returns>
        /// <exception cref="CombinePathsException">Thrown if the paths can't be combined.</exception>

        string CombinePaths(string path1, string path2);

        /// <summary>
        /// Make sure that there is a trailing slash at the end of the
        /// given path.
        /// </summary>
        /// <param name="path">input path</param>
        /// <returns>path guaranteed to have a slash at the end</returns>

        string EnsureTrailingSlashes(string path);
        
        /// <summary>
        /// Copy all files/directories (recursively) from source to target.
        /// Overwrite already existing files without warning.
        /// <remarks>
        /// If an error occurs, already copied files/directories are not deleted.
        /// </remarks>
        /// </summary>
        /// <param name="sourceDirectory">Source directory to copy from.</param>
        /// <param name="targetDirectory">Target directory to copy to.</param>
        /// <returns></returns>
        /// <exception cref="DirectoryCreationException">Can't create directory in target location.</exception>
        /// <exception cref="FileCopyException">Error copying a file.</exception>
        /// <exception cref="DirectoryDoesNotExistException">If the <paramref name="sourceDirectory"/> doesn't exist.</exception>

        void Copy(string sourceDirectory, string targetDirectory);

        /// <summary>
        /// Copy files and directories (recursively) which have a modified 
        /// date greater or equal to <paramref name="lastModifiedDate"/> 
        /// from source to target. Optionally exclude some <paramref name="excludedFiles"/>.
        /// Overwrite already existing files without warning.
        /// </summary>
        /// <remarks>
        /// If an error occurs, already copied files/directories are not deleted.
        /// </remarks>
        /// <param name="sourceDirectory">Source directory to copy from.</param>
        /// <param name="targetDirectory">Target directory to copy to.</param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="excludedFiles">Optional: Exclude a list of files not to copy (case-insensitive check)</param>
        /// <exception cref="DirectoryCreationException">Can't create directory in target location.</exception>
        /// <exception cref="FileCopyException">Error copying a file.</exception>
        /// <exception cref="DirectoryNotFoundException">If the <paramref name="sourceDirectory"/> doesn't exist.</exception>

        void CopyDelta(string sourceDirectory, string targetDirectory, DateTime lastModifiedDate, IList<string> excludedFiles = null);

        /// <summary>
        /// Copy <paramref name="sourceFile"/> to its destination <paramref name="targetFile"/>
        /// </summary>
        /// <remarks>
        /// If the target directory path doesn't exist - try to create it.
        /// Overwrite already existing file.
        /// </remarks>
        /// <param name="sourceFile">File to copy (incl. absolute directory path)</param>
        /// <param name="targetFile">Target of the file (incl. absolute directory path)</param>
        /// <exception cref="FileCopyException">If the file can't be copied</exception>

        void CopyFile(string sourceFile, string targetFile);

        /// <summary>
        /// Detects the byte order mark of a file and returns
        /// an appropriate encoding for the file.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <returns></returns>
        /// http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader

        Encoding GetFileEncoding(MemoryStream stream);

        /// <summary>
        /// Get list of files (including their paths) in the specified directory (no recursive looking down).
        /// </summary>
        /// <param name="directoryPath">Directory to get files from.</param>
        /// <param name="searchPattern">Optional: filter which files should be returned (e.g. *.xml)</param>
        /// <returns>Return list of files (including their paths) in the specified directory</returns>
        /// <exception cref="FileAccessException">On any error (i.e. directory doesn't exist)</exception>

        IList<string> GetFiles(string directoryPath, string searchPattern = null);

         /// <summary>
        /// Get list of sub-directories (including their paths) in the specified directory (no recursive looking down).
        /// </summary>
        /// <param name="directoryPath">Directory to get directories from.</param>
        /// <param name="searchPattern">Optional: filter which directories should be returned</param>
        /// <returns>Return list of sub-directories (including their paths) in the specified directory</returns>

        IList<string> GetDirectories(string directoryPath, string searchPattern = null);
        
        /// <summary>
        /// Get all files from the <paramref name="directoryPath"/> which match
        /// the regex <paramref name="regexPattern"/>.
        /// </summary>
        /// <param name="directoryPath">Directory path to look for files</param>
        /// <param name="regexPattern">Regular expression to filter files</param>
        /// <returns>Return list of matching files (incl. absolute path)</returns>
        /// <exception cref="FileAccessException">If anything goes wrong</exception>

        IList<string> GetFilesByRegex(string directoryPath, string regexPattern);
      
        /// <summary>
        /// Get all directories from the <paramref name="directoryPath"/> which match
        /// the regex <paramref name="regexPattern"/>.
        /// </summary>
        /// <param name="directoryPath">Directory path to look for directories</param>
        /// <param name="regexPattern">Regular expression to filter directories</param>
        /// <returns>Return list of matching directories (incl. absolute path)</returns>
        /// <exception cref="FileAccessException">If anything goes wrong</exception>

        IList<string> GetDirectoriesByRegex(string directoryPath, string regexPattern);

        /// <summary>
        /// Get all file and directories from <paramref name="directoryPath"/>.
        /// Only direct items are returned (no recursive going through the sub-directories).
        /// </summary>
        /// <param name="directoryPath">Full path from where to read the items from</param>
        /// <returns>Returns list of items found. Returns an empty list if the <paramref name="directoryPath"/> is empty</returns>
        /// <exception cref="DirectoryDoesNotExistException">If the <paramref name="directoryPath"/> doesn't exist.</exception>
        /// <exception cref="FileAccessException">In any other case.</exception>
        
        IList<IFileItem> GetItemsFromDirectory(string directoryPath);

        /// <summary>
        /// Check whether two paths are equal (ignore mixed '\' and '/' or trailing slashes).
        /// </summary>
        /// <remarks>The paths doesn't need to exist.</remarks>
        /// <example>
        /// c:\marinelms\content\ == C:\MarineLMS/content
        /// c:/MarineLMS/content/ == c:\marinelms\content\
        /// c:/marinelms/ != c:/marine/
        /// </example>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns>Return <c>true</c> if both are <c>null</c> or equal (case-insensitive). Return <c>false</c> if they are different</returns>

        bool AreEqualPaths(string path1, string path2);

        /// <summary>
        /// Assemble a unique directory within <paramref name="baseDirectory"/>.
        /// The <paramref name="baseDirectoryName"/> and optional <paramref name="dirExtension"/> 
        /// is used to generate the unique directory name.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="baseDirectoryName"/> already exists we number the directory at the end until 
        /// we find a directory name which doesn't exist.
        /// Append '#' where # is the first non-conflicting number from <paramref name="baseDirectoryName"/>.
        /// </remarks>
        /// <example>
        /// <paramref name="baseDirectoryName"/>: 'c:\MarineLMS\Folder'
        /// <paramref name="baseDirectoryName"/>: 'ExportPackage'
        /// <paramref name="dirExtension"/>: <c>null</c>
        /// If 'ExportPackage' doesn't exist in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage'
        /// If 'ExportPackage' already exists in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage1'
        /// If 'ExportPackage' and 'ExportPackage1' already exists return 'c:\MarineLMS\Folder\ExportPackage2'
        /// If 'ExportPackage' and 'ExportPackage2 already exists return 'c:\MarineLMS\Folder\ExportPackage1'
        /// </example>
        /// <example>
        /// <paramref name="baseDirectoryName"/>: 'c:\MarineLMS\Folder'
        /// <paramref name="baseDirectoryName"/>: 'ExportPackage'
        /// <paramref name="dirExtension"/>: <c>.Repo</c>
        /// If 'ExportPackage.Repo' doesn't exist in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage.Repo'
        /// If 'ExportPackage.Repo' already exists in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage1.Repo'
        /// If 'ExportPackage.Repo' and 'ExportPackage1.Repo' already exists return 'c:\MarineLMS\Folder\ExportPackage2.Repo'
        /// If 'ExportPackage.Repo' and 'ExportPackage2.Repo already exists return 'c:\MarineLMS\Folder\ExportPackage1.Repo'
        /// </example>
        /// <param name="baseDirectory">Directory path in which we want to create the <paramref name="baseDirectory"/></param>
        /// <param name="baseDirectoryName">Base directory name to use to generate a unique non-existing directory name</param>
        /// <param name="dirExtension">Optional: directory extension. Set <c>null</c> to ignore it</param>
        /// <param name="createDirectory">Optional: create the new unique directory (default: <c>false</c>).</param>
        /// <returns>Return the non-existing unique directory name incl. <paramref name="baseDirectory"/></returns>

        string GenerateUniqueDirectoryName(string baseDirectory, string baseDirectoryName, string dirExtension = null, bool createDirectory = false);

        /// <summary>
        /// Assemble a unique file name within <paramref name="baseDirectory"/>.
        /// The <paramref name="baseFileName"/> is used to generate the unique file name.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="baseFileName"/> already exists we number the file name at the end until 
        /// we find a file name which doesn't exist.
        /// Append '#' where # is the first non-conflicting number from <paramref name="baseFileName"/>.
        /// </remarks>
        /// <example>
        /// <paramref name="baseDirectoryName"/>: 'c:\MarineLMS\Folder'
        /// <paramref name="baseFileName"/>: 'ExportPackage'
        /// <paramref name="fileExtension"/>: '.zip'
        /// If 'ExportPackage.zip' doesn't exist in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage.zip'
        /// If 'ExportPackage.zip' already exists in 'c:\MarineLMS\Folder' return 'c:\MarineLMS\Folder\ExportPackage1.zip'
        /// If 'ExportPackage.zip' and 'ExportPackage1.zip' already exists return 'c:\MarineLMS\Folder\ExportPackage2.zip'
        /// If 'ExportPackage.zip' and 'ExportPackage2.zip' already exists return 'c:\MarineLMS\Folder\ExportPackage1.zip'
        /// </example>
        /// <param name="baseDirectory">Directory path in which we want to create the <paramref name="baseDirectory"/></param>
        /// <param name="baseFileName">Base file name (without extension) to use to generate a unique non-existing file name</param>
        /// <param name="fileExtension">File extension (incl. '.'). For example, '.zip'</param>
        /// <returns>Return the non-existing unique file name incl. <paramref name="baseDirectory"/> and <paramref name="fileExtension"/></returns>

        string GenerateUniqueFileName(string baseDirectory, string baseFileName, string fileExtension);

        /// <summary>
        /// Shorten a file name (with optional extension) to the appropriate <paramref name="maxLength"/>).
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="maxLength">Max file name length to return (length to shorten the <paramref name="fileName"/> to.</param>
        /// <param name="hasExtension">Optional: tells whether <paramref name="fileName"/> has an extension to take care while shortening</param>
        /// <returns>
        /// Return the shortened file name. 
        /// Return full file name if <paramref name="maxLength"/> is longer than the <paramref name="fileName"/>
        /// </returns>

        string ShortenFileName(string fileName, int maxLength, bool hasExtension = false);

        /// <summary>
        /// Remove <paramref name="dir"/> if it is empty.
        /// </summary>
        /// <param name="dir">Directory to check/remove if empty</param>

        void RemoveEmptyDirectory(string dir);

        /// <summary>
        /// Get all filesInfos for each file from directory recursive.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>Array of FileInfo[]</returns>

        IFileItem[] GetAllFileFromDirectory(string directoryPath);
    }
}
