using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using EC.Common.Base;
using Castle.Core.Logging;
using EC.Common.Interfaces;
using DotNetSystem = System.Configuration;
using Castle.MicroKernel;
using System.Text.RegularExpressions;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.FileExceptions;

namespace EC.Core.Common
{
    /// <summary>
    /// Wrapper around file system access. This wrapper exists so as to make unit testing
    /// possible (i.e. these entry points can be mocked).
    /// </summary>

    [SingletonType]
    [RegisterAsType(typeof(IFileAccess))]

    public class FileAccess : IFileAccess
    {
        /// <summary>
        /// Read chunk of file.
        /// </summary>
        /// <remarks>
        /// Automatically adjusts <paramref name="chunkSize"/> if (<paramref name="offset"/> + <paramref name="chunkSize"/>) is greater than the file size
        /// </remarks>
        /// <param name="filePath"></param>
        /// <param name="offset"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        /// <exception cref="OutOfRangeException">
        /// If the file offset is less than 0. 
        /// If the file chunk size is less than 0.
        /// if the offset is bigger than the actual file size. 
        /// </exception>
        /// <exception cref="FolderOrFileNotFoundException">If the file doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file can't be accessed.</exception>
       
        public byte[] ReadFileChunk(string filePath, long offset, int chunkSize)
        {
            var fileSize = GetFileSizeInBytes(filePath);
            if (offset < 0) { throw new OutOfRangeException("File offset must be a value >= 0."); }
            if (chunkSize <= 0) { throw new OutOfRangeException("The file chunk size must be < 0."); }
            if (offset > fileSize) { throw new OutOfRangeException("File offset must be a value < file file size.");}
            if (offset + chunkSize > fileSize)
            {
                // Adapt chunk size if we are at the end of the file.
                chunkSize = (int)(fileSize - offset);
            }
            
            try
            {
                byte[] buffer = new byte[chunkSize];
                using (FileStream fs = File.Open(filePath, FileMode.Open, System.IO.FileAccess.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    BinaryReader br = new BinaryReader(fs);
                    buffer = br.ReadBytes(chunkSize);
                    br.Close();
                    fs.Close();
                }
                return buffer;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't access the file", filePath, ex);
            }
        }

        /// <summary>
        /// Write a chunk of file. The offset must be smaller/equal to the current file size - no writing outside the file size possible.
        /// </summary>
        /// <remarks>If the file doesn't exist - create it</remarks>
        /// <param name="filePath">Full path and file name</param>
        /// <param name="offset">Offset where to write the <paramref name="chunk"/></param>
        /// <param name="chunk">Chunk of bytes to write</param>
        /// <returns>Return next offset position</returns>
        /// <exception cref="FileWriteException">If the file couldn't be written.</exception>

        public long WriteFileChunk(string filePath, long offset, byte[] chunk)
        {
            try
            {
                using (var f = File.OpenWrite(filePath))
                {
                    var currentPos = f.Seek(offset, SeekOrigin.Begin);
                    f.Write(chunk, 0, chunk.Length);
                    f.Flush();
                    f.Close();
                }
                return offset + chunk.Length;
            }
            catch (Exception ex)
            {
                throw new FileWriteException(filePath, ex);
            }
        }

        /// <summary>
        /// Get file size of the file <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Path incl. file name</param>
        /// <returns>Return size of the file.</returns>
        /// <exception cref="FolderOrFileNotFoundException">If the file doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file can't be accessed.</exception>

        public long GetFileSizeInBytes(string filePath)
        {
            if (!FileExists(filePath)) { throw new FolderOrFileNotFoundException(filePath, string.Format("The file at '{0}' doesn't exist.", filePath)); }
            try
            {
                return new FileInfo(filePath).Length;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get file size.", filePath, ex);
            }
        }

        /// <summary>
        /// Check whether the given path refers to a file or a directory.
        /// </summary>
        /// <param name="path">path to check</param>
        /// <returns>true if the path is a directory, false otherwise</returns>
        ///<exception cref="FileAccessException">if any System.IO exception occurs</exception>
        /// TODO: This method does not work if path is neither a dir nor file. Replace with System.IO.DirectoryExists()

        public bool IsDirectory(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't check whether the path is a directory.", path, ex);
            }
        }


        /// <summary>
        /// Check whether the path is rooted (absolute path).
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>
        /// Return <c>true</c> if the <paramref name="path" /> is rooted. <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="FileAccessException">Can't check whether the path is rooted.</exception>

        public bool IsPathRooted(string path)
        {
            try
            {
                return Path.IsPathRooted(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't check whether the path is rooted.", path, ex);
            }
        }

        /// <summary>
        /// Get the name of the last element referred to by the given path, regardless
        /// of whether it is a directory or a file.
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>the name of the last item in the path</returns>
        ///<exception cref="FileAccessException">if any System.IO exception occurs</exception>

        public string GetNameFromPath(string path)
        {
            try
            {
                var info = IsDirectory(path) ? (FileSystemInfo)new DirectoryInfo(path) : (FileSystemInfo)new FileInfo(path);
                return info.Name;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get name of a path", path, ex);
            }
        }

        /// <summary>
        /// Get the parent directory (as absolute path) of a <paramref name="path"/> (either file or directory).
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
        /// <returns>Return the parent directory (absolute path). Return <c>null</c> the <paramref name="path"/> is the root path</returns>
        /// <exception cref="DirectoryDoesNotExistException">If the directory <paramref name="path"/> is a directory and doesn't exist</exception>
        /// <exception cref="FileAccessException">If the file doesn't exist or on any other error</exception>

        public string GetParentDirectory(string path)
        {
            try
            {
                if (this.IsDirectory(path))
                {
                    if (!this.DirectoryExists(path)) { throw new DirectoryDoesNotExistException(path); }
                    var dirInfo = new DirectoryInfo(path);
                    var parentInfo = dirInfo.Parent;
                    if (parentInfo == null) { return null; }
                    return parentInfo.FullName;
                }
                else
                {
                    var fInfo = new FileInfo(path);
                    var parentInfo = fInfo.Directory;
                    return parentInfo.FullName;
                }
            }
            catch (DirectoryNotFoundException dnfEx)
            {
                throw new DirectoryDoesNotExistException(path, dnfEx);
            }
        }

        /// <summary>
        /// Get the creation date (in UTC) for the last item in the given path.
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>create date (UTC)</returns>
        ///<exception cref="FileAccessException">if any System.IO exception occurs</exception>

        public DateTime GetCreationDate(string path)
        {
            try
            {
                var info = IsDirectory(path) ? (FileSystemInfo)new DirectoryInfo(path) : (FileSystemInfo)new FileInfo(path);
                return info.CreationTimeUtc;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get file or directory creation date", path, ex);
            }
        }

        /// <summary>
        /// Get the last modification date (in UTC) for the last item in the given path.
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>modification date (UTC)</returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        public DateTime GetModificationDate(string path)
        {
            var info = IsDirectory(path) ? (FileSystemInfo)new DirectoryInfo(path) : (FileSystemInfo)new FileInfo(path);
            return info.LastWriteTimeUtc;
        }

        /// <summary>
        /// Gets the length of the file in bytes. If a directory returns 0
        /// </summary>
        /// <param name="path">path to item</param>
        /// <returns>content length in bytes</returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        public long GetLength(string path) {
            if (IsDirectory(path)) { return 0; }

            var fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        /// <summary>
        /// Return a stream via which the contents of a file can be read.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>
        /// <returns>the stream</returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        /// <summary>
        /// Read the contents of the specified file and return them as a string.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">directory in path not found</exception>

        public string ReadFileAsString(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var contents = sr.ReadToEnd();
                return contents;
            }
        }

        /// <summary>
        /// Create a StreamReader for the given file.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>
        /// <returns>the StreamReader</returns>

        public StreamReader CreateStreamReader(string path, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            return new StreamReader(path, encoding);
        }

        /// <summary>
        /// Empty the contents of the specified file.
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <exception cref="FileWriteException">If there is any problem emptying the contents</exception>

        public void EmptyFile(string path)
        {
            try
            {
                var file = File.Open(path, FileMode.Truncate);
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw new FileWriteException(path, ex);
            }
        }

        /// <summary>
        /// Create or overwrite the given file with the given contents. This will
        /// create directories as needed.
        /// </summary>
        /// <param name="path">full absolute path to the file</param>
        /// <param name="contents">data to write to file</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileAlreadyExistsException"></exception>
        /// <exception cref="FileWriteException"></exception>

        public void WriteFile(string path, byte[] contents, bool overwriteExistingFile = true)
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (this.FileExists(path))
                {
                    if (!overwriteExistingFile) { throw new FileAlreadyExistsException(Path.GetFileName(path)); }
                    else {
                        //clear the contents of the file before writing
                        this.EmptyFile(path);
                    }
                }

                var file = File.OpenWrite(path);
                file.Write(contents, 0, contents.Count());
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw new FileWriteException(path, ex);
            }
        }

        /// <summary>
        /// Create or overwrite the given file with the given contents. This will create
        /// directories as needed.
        /// </summary>
        /// <param name="path">Full absolute path to the file.</param>
        /// <param name="contentStream">data to write to file</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>

        public void WriteFile(string path, Stream contentStream, bool overwriteExistingFile = true)
        {
            try
            {
                new FileInfo(path).Directory.Create();
                if (this.FileExists(path))
                {
                    if (!overwriteExistingFile) { throw new FileAlreadyExistsException(Path.GetFileName(path)); }
                    else
                    {
                        //clear the contents of the file before writing
                        this.EmptyFile(path);
                    }
                }

                using (var file = File.Create(path))
                {
                    int readBytes = 0;
                    byte[] buffer = new byte[8 * 1024];
                    while ((readBytes = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, readBytes);
                    }
                    file.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new FileWriteException(path, ex);
            }
        }

        /// <summary>
        /// Move the file from source to target. Optionally replace an already existing file.
        /// </summary>
        /// <param name="sourceFileName">Full path and file name of the source file.</param>
        /// <param name="targetFileName">Full path and file name of the target file to put the source.</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileNotFoundException">If the <paramref name="sourceFileName"/> doesn't exist.</exception>
        /// <exception cref="FileWriteException">If the file can't be moved.</exception>
        /// <exception cref="FileAlreadyExistsException">if the file already exists and overwriteExistingFile is false</exception>

        public void MoveFile(string sourceFileName, string targetFileName, bool overwriteExistingFile = true)
        {
            new FileInfo(targetFileName).Directory.Create();
            if (!this.FileExists(sourceFileName)) { throw new FileNotFoundException(string.Format("Source file '{0}' doesn't exist.", sourceFileName), sourceFileName); }

            // File name in case the file gets replacement...
            string oldFileName = targetFileName + "_old_" + SequentialGUID.Create();

            try
            {
                if ((overwriteExistingFile) && (this.FileExists(targetFileName)))
                {
                    // If target file already exists and should be overwritten...        
                    File.Move(targetFileName, oldFileName);
                }
                else if (this.FileExists(targetFileName))
                {
                    // If target file exists and shouldn't be overwritten...
                    throw new FileAlreadyExistsException(Path.GetFileName(targetFileName));
                }
            }
            catch (FileAlreadyExistsException ex) { throw ex; }
            catch (Exception ex) { throw new FileWriteException(oldFileName, ex); }

            try
            {
                File.Move(sourceFileName, targetFileName);
            }
            catch (Exception ex)
            {

                if ((overwriteExistingFile) && (this.FileExists(oldFileName)))
                {
                    // Undo renaming of the existing file...
                    File.Move(oldFileName, targetFileName);
                }
                throw new FileWriteException(targetFileName, ex);
            }

            try
            {
                // Remove old file...
                if (this.FileExists(oldFileName))
                {
                    File.Delete(oldFileName);
                }
            }
            catch (Exception ex)
            {
                this.Logger.ErrorFormat(ex, "MoveFile: The old file '{0}' (original file {1}) in file collection can't be deleted and remain as orphan file.", oldFileName, targetFileName);
                throw new FileWriteException(oldFileName, ex);
            }
        }

        /// <summary>
        /// Move a file into a specific directory. Optionally replace an already existing file.
        /// </summary>
        /// <param name="sourceFileName">Full path and file name of the source file.</param>
        /// <param name="targetDirectory">Full path of the target directory to move the file into.</param>
        /// <param name="overwriteExistingFile">Flag whether to replace an already existing file.</param>
        /// <exception cref="FileNotFoundException">If the <paramref name="sourceFileName"/> doesn't exist.</exception>
        /// <exception cref="FileWriteException">If the file can't be moved.</exception>
        /// <exception cref="FileAlreadyExistsException">if the file already exists and overwriteExistingFile is false</exception>

        public void MoveFileToDirectory(string sourceFileName, string targetDirectory, bool overwriteExistingFile = true)
        {
            if (!this.FileExists(sourceFileName)) { throw new FileNotFoundException(string.Format("The file '{0}' doesn't exist.", sourceFileName)); }
            var targetFileName = this.CombinePaths(targetDirectory, this.GetFileName(sourceFileName));
            this.MoveFile(sourceFileName, targetFileName, overwriteExistingFile);
        }

        /// <summary>
        /// Move the directory from source to target.
        /// This will create directories as needed.
        /// </summary>
        /// <param name="sourceDirectory">Full path of the source directory.</param>
        /// <param name="targetDirectory">Full path of the target directory where to put the source.</param>
        /// <exception cref="DirectoryDoesNotExistException">If the <paramref name="sourceDirectory"/> doesn't exist.</exception>
        /// <exception cref="FileAccessException">On any other error (i.e. the <paramref name="sourceDirectory"/> and <paramref name="targetDirectory"/> are on different drives)</exception>

        public void MoveDirectory(string sourceDirectory, string targetDirectory)
        {
            if (!this.DirectoryExists(sourceDirectory)) { throw new DirectoryDoesNotExistException(sourceDirectory); }
            try
            {
                var targetDirName = this.GetNameFromPath(sourceDirectory);
                Directory.Move(sourceDirectory, this.CombinePaths(targetDirectory, targetDirName));
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Error moving the directory.", sourceDirectory, ex);
            }
        }

        /// <summary>
        /// Create a new directory with the given path.
        /// </summary>
        /// <remarks>
        /// If parts or all of the directory already exists - do nothing.
        /// </remarks>
        /// <param name="path">full absolute path for new directory</param>
        /// <exception cref="DirectoryCreationException">Can't create directory.</exception>

        public void CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new DirectoryCreationException(path, ex);
            }
        }

        /// <summary>
        /// Delete a file from disk.
        /// </summary>
        /// <param name="path">the full absolute path to the file</param>

        public void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                throw new FileDeletionException(path, ex);
            }
        }



        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <exception cref="FileDeletionException"></exception>

        public void DeleteDirectory(string path, bool recursive)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                throw new FileDeletionException(path, ex);
            }
        }

        /// <summary>
        /// Deletes the directory's contents, but not the directory itself.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="FileDeletionException"></exception>
        /// <exception cref="FileAccessException"></exception>

        public void DeleteDirectoryContents(string path)
        {
            if (!DirectoryExists(path))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    throw new FileDeletionException(file, ex);
                }
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception ex)
                {
                    throw new FileDeletionException(directory, ex);
                }
            }
        }

        /// <summary>
        /// Check if the file exists.
        /// </summary>
        /// <param name="path">Path incl. file name.</param>
        /// <returns>Return <c>true</c> if the file exists. Return <c>false</c> if the file doesn't exist.</returns>
        /// <exception cref="FileAccessException">Thrown if the file can't be accessed or on any other error.</exception>

        public bool FileExists(string path)
        {
            try
            {
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't check whether the file exists.", path, ex);
            }
        }

        /// <summary>
        /// Check if the directory exists.
        /// </summary>
        /// <param name="path">Directory path.</param>
        /// <returns>Return <c>true</c> if the directory exists. Return <c>false</c> if the directory doesn't exist.</returns>
        /// <exception cref="FileAccessException">Thrown if the directory can't be accessed or on any other error.</exception>

        public bool DirectoryExists(string path)
        {
            try
            {
                return Directory.Exists(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't check whether the directory exists.", path, ex);
            }
        }

        /// <summary>
        /// Get file name incl. file extension from full path.
        /// </summary>
        /// <param name="path">file name (incl. path)</param>
        /// <returns>Return file name incl. file extension.</returns>
        /// <exception cref="FileAccessExecption">Thrown in any case of error.</exception>

        public string GetFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get file name from path.", path, ex);
            }
        }

        /// <summary>
        /// Get directory name of <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="path"/> contains a file name - the file name is ignored.
        /// </remarks>
        /// <param name="path">Directory path (may include a file name) to get the directory name from</param>
        /// <returns>Return directory name</returns>
        /// <exception cref="FileAccessException">Thrown in any other case of error.</exception>

        public string GetDirectoryName(string path)
        {
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException(ex.Message, path, ex);
            }
        }

        /// <summary>
        ///  Get file name incl. file extension from full path.
        /// </summary>
        /// <param name="path">file name (incl. path)</param>
        /// <returns>Return file name without the file extension.</returns>
        /// <exception cref="FileAccessExecption">Thrown in any case of error.</exception>

        public string GetFileNameWithoutExtension(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get file name without extension.", path, ex);
            }
        }

        /// <summary>
        /// Get directory information (absolute path) of the specified path string (the <paramref name="path"/> must exist).
        /// </summary>
        /// <remarks>If the <paramref name="path"/> is already a directory - return the same <paramref name="path"/></remarks>
        /// <param name="path">Path (might include a file name)</param>
        /// <returns>
        /// Return directory name (absolute path).
        /// </returns>
        /// <exception cref="FileAccessException">If the path can't be accessed or the path is wrong</exception>

        public string GetDirectory(string path)
        {
            try
            {
                if (this.IsDirectory(path)) { return path; }
                return Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get directory name from path.", path, ex);
            }
        }
        
        /// <summary>
        /// Combine two paths. If the second path is not relative (i.e. begins with
        /// a '/' or a '\'), then those characters a stripped off.
        /// </summary>
        /// <param name="path1">first path</param>
        /// <param name="path2">second path</param>
        /// <returns>path1 + path2</returns>

        public string CombinePaths(string path1, string path2)
        {
            try
            {

                if (path2 != null)
                {
                    path2 = path2.TrimStart(new char[] { '/', '\\' });
                    path2 = path2.ReplaceBackWithForwardSlashes();
                }
                if (path1 != null)
                {
                    path1 = path1.ReplaceBackWithForwardSlashes();
                }
                if (string.IsNullOrEmpty(path1))
                {
                    return path2;
                }
                if (string.IsNullOrEmpty(path2))
                {
                    // If the second path is empty...
                    return path1;
                }

                return Path.Combine(path1, path2);
            }
            catch (Exception ex)
            {
                throw new PathCombineException(path1, path2, ex);
            }
        }

        /// <summary>
        /// Make sure that there is a trailing slash at the end of the
        /// given path.
        /// </summary>
        /// <param name="path">input path</param>
        /// <returns>path guaranteed to have a slash at the end</returns>

        public string EnsureTrailingSlashes(string path)
        {
            if (path.EndsWith(PathConstants.ElementSeparator)) return path;
            if (path.EndsWith(@"\")) return path;
            return path += PathConstants.ElementSeparator;
        }

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
        /// <exception cref="DirectoryNotFoundException">If the <paramref name="sourceDirectory"/> doesn't exist.</exception>

        public void Copy(string sourceDirectory, string targetDirectory)
        {
            if (!this.DirectoryExists(sourceDirectory))
            {
                throw new DirectoryDoesNotExistException(sourceDirectory);
            }

            foreach (string srcFile in Directory.GetFiles(sourceDirectory))
            {
                try
                {
                    File.Copy(srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)), true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorFormat(ex, "File copy failed: source '{0}' to target '{1}'", srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)));
                    throw new FileCopyException(srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)), ex);
                }
            }
            foreach (var dir in Directory.GetDirectories(sourceDirectory))
            {
                string tempTargetDir = Path.Combine(targetDirectory, Path.GetFileName(dir));
                if (!this.DirectoryExists(tempTargetDir))
                {
                    try
                    {
                        this.CreateDirectory(tempTargetDir);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.ErrorFormat(ex, "Create directory '{0}' failed", tempTargetDir);

                    }
                }
                this.Copy(dir, tempTargetDir);
            }
        }

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

        public void CopyDelta(string sourceDirectory, string targetDirectory, DateTime lastModifiedDate, IList<string> excludedFiles = null)
        {
            if (!this.DirectoryExists(sourceDirectory))
            {
                throw new DirectoryDoesNotExistException(sourceDirectory);
            }

            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDirectory);
            var filesQuery = sourceDirInfo.GetFiles().Where(f => f.LastWriteTimeUtc >= lastModifiedDate);
            if ((excludedFiles != null) && (excludedFiles.Count > 0))
            {
                filesQuery = filesQuery.Where(f => excludedFiles.Any(e => e.Equals(f.Name, StringComparison.OrdinalIgnoreCase)) == false);
            }
            var filesForCopy = filesQuery.Select(f => f.FullName);

            foreach (string srcFile in filesForCopy)
            {
                try
                {
                    File.Copy(srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)), true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorFormat(ex, "File copy failed: source '{0}' to target '{1}'", srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)));
                    throw new FileCopyException(srcFile, Path.Combine(targetDirectory, Path.GetFileName(srcFile)), ex);
                }
            }

            var dirInfos = sourceDirInfo.GetDirectories();
            foreach (var dirInfo in dirInfos)
            {
                //string tempTargetDir = Path.Combine(targetDirectory, Path.GetFileName(dir));
                string tempTargetDir = Path.Combine(targetDirectory, Path.GetFileName(dirInfo.FullName));

                if (!this.DirectoryExists(tempTargetDir))
                {
                    try
                    {
                        this.CreateDirectory(tempTargetDir);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.ErrorFormat(ex, "Create directory '{0}' failed", tempTargetDir);

                    }
                }
                this.CopyDelta(dirInfo.FullName, tempTargetDir, lastModifiedDate, excludedFiles);

                this.RemoveEmptyDirectory(tempTargetDir);
            }
        }

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

        public void CopyFile(string sourceFile, string targetFile)
        {
            try
            {
                var targetDir = Path.GetDirectoryName(targetFile);

                if (!DirectoryExists(targetDir))
                {
                    CreateDirectory(targetDir);
                }

                File.Copy(sourceFile, targetFile, true);
            }
            catch (Exception ex)
            {
                this.Logger.ErrorFormat(ex, "File copy failed: source '{0}' to target '{1}'", sourceFile, targetFile);
                throw new FileCopyException(sourceFile, targetFile, ex);
            }
        }

        /// <summary>
        /// Detects the byte order mark of a file and returns
        /// an appropriate encoding for the file.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <returns></returns>
        /// http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
        /// 

        public Encoding GetFileEncoding(MemoryStream stream)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];

            stream.Position = 0;
            stream.Read(buffer, 0, 5);

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;

            return enc;
        }

        /// <summary>
        /// Get list of files (including their paths) in the specified directory (no recursive looking down).
        /// </summary>
        /// <param name="directoryPath">Directory to get files from.</param>
        /// <param name="searchPattern">Optional: filter which files should be returned (e.g. *.xml)</param>
        /// <returns>Return list of files (including their paths) in the specified directory</returns>
        /// <exception cref="FileAccessException">On any error (i.e. directory doesn't exist)</exception>

        public IList<string> GetFiles(string directoryPath, string searchPattern = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    return Directory.GetFiles(directoryPath);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }

            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get files from path.", directoryPath, ex);
            }
        }

        /// <summary>
        /// Get all files from the <paramref name="directoryPath"/> which match
        /// the regex <paramref name="regexPattern"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="regesPattern"/> is only applied to the file name (not the whole path).
        /// </remarks>
        /// <param name="directoryPath">Directory path to look for files</param>
        /// <param name="regexPattern">Regular expression to filter files</param>
        /// <returns>Return list of matching files (incl. absolute path)</returns>
        /// <exception cref="FileAccessException">If anything goes wrong</exception>

        public IList<string> GetFilesByRegex(string directoryPath, string regexPattern)
        {
            try
            {
                var allFiles = Directory.GetFiles(directoryPath);
                var allFileNames = allFiles.Select(f => this.GetFileName(f)).ToList();
                var foundFileNames = this.GetRegexMatches(allFileNames, regexPattern);
                return foundFileNames.Select(f => this.CombinePaths(directoryPath, f)).ToList();
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get files by RegEx from path.", directoryPath, ex);
            }
        }

        /// <summary>
        /// Get all directories from the <paramref name="directoryPath"/> which match
        /// the regex <paramref name="regexPattern"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="regesPattern"/> is only applied to the directory name (not the whole path).
        /// </remarks>
        /// <param name="directoryPath">Directory path to look for directories</param>
        /// <param name="regexPattern">Regular expression to filter directories</param>
        /// <returns>Return list of matching directories (incl. absolute path)</returns>
        /// <exception cref="FileAccessException">If anything goes wrong</exception>

        public IList<string> GetDirectoriesByRegex(string directoryPath, string regexPattern)
        {
            try
            {
                var allDirectories = Directory.GetDirectories(directoryPath);
                var allDirectoryNames = allDirectories.Select(d => this.GetNameFromPath(d)).ToList();
                var foundDirectoryNames = GetRegexMatches(allDirectoryNames, regexPattern);
                return foundDirectoryNames.Select(d => this.CombinePaths(directoryPath, d)).ToList();
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get directories by RegEx from path.", directoryPath, ex);
            }
        }

        /// <summary>
        /// Get all items from <paramref name="allItems"/> which match the regex expression in
        /// <paramref name="regexPattern"/>.
        /// </summary>
        /// <param name="allItems">List of items to look in</param>
        /// <param name="regexPattern">Regex expression to find items</param>
        /// <returns>Return all items which match the <paramref name="regexPattern"/></returns>

        private IList<string> GetRegexMatches(IList<string> allItems, string regexPattern)
        {
            IList<string> foundItems = new List<string>();
            foreach (var item in allItems)
            {
                var match = Regex.Match(item, regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (match.Success) { foundItems.Add(item); }
            }
            return foundItems;
        }

        /// <summary>
        /// Get list of sub-directories (including their paths) in the specified directory (no recursive looking down).
        /// </summary>
        /// <param name="directoryPath">Directory to get directories from.</param>
        /// <param name="searchPattern">Optional: filter which directories should be returned</param>
        /// <returns>Return list of sub-directories (including their paths) in the specified directory</returns>

        public IList<string> GetDirectories(string directoryPath, string searchPattern = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    return Directory.GetDirectories(directoryPath);
                }
                else { return Directory.GetDirectories(directoryPath, searchPattern); }
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get directories from path.", directoryPath, ex);
            }
        }

        public IList<IFileItem> GetItemsFromDirectory(string directoryPath)
        {
            try
            {
                if (!this.IsDirectory(directoryPath)) { throw new DirectoryDoesNotExistException(directoryPath); }
            }
            catch (Exception ex)
            {
                throw new DirectoryDoesNotExistException(directoryPath, ex);
            }

            try
            {
                // Get all directories...
                var dirInfo = new DirectoryInfo(directoryPath);
                var directories = dirInfo.GetDirectories();
                var files = dirInfo.GetFiles();

                var result = new List<IFileItem>();
                foreach (var d in directories)
                {
                    IFileItem dItem = null;

                    try
                    {
                        dItem = this.Kernel.Resolve<IFileItem>();
                        dItem.IsDirectory = true;
                        dItem.Name = d.Name;
                        dItem.FullName = d.FullName;
                        dItem.CreatedDate = d.CreationTimeUtc;
                        dItem.ModifiedDate = d.LastWriteTimeUtc;
                        result.Add(dItem);
                    }
                    finally
                    {
                        if (dItem != null) Kernel.ReleaseComponent(dItem);
                    }
                }

                foreach (var f in files)
                {
                    IFileItem fItem = null;
                    try
                    {
                        fItem = this.Kernel.Resolve<IFileItem>();
                        fItem.IsDirectory = false;
                        fItem.Name = f.Name;
                        fItem.FullName = f.FullName;
                        fItem.CreatedDate = f.CreationTimeUtc;
                        fItem.ModifiedDate = f.LastWriteTimeUtc;
                        result.Add(fItem);
                    }
                    finally
                    {
                        if (fItem != null) this.Kernel.ReleaseComponent(fItem);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get items from path", directoryPath, ex);
            }
        }

        /// <summary>
        /// Get all filesInfos for each file from directory recursive.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>Array of FileInfo[]</returns>

        public IFileItem[] GetAllFileFromDirectory(string directoryPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                var items = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Select(z =>
                {
                    IFileItem fItem = null;
                    try
                    {
                        fItem = Kernel.Resolve<IFileItem>();
                        fItem.CreatedDate = z.CreationTimeUtc;
                        fItem.FullName = z.FullName;
                        fItem.IsDirectory = false;
                        fItem.ModifiedDate = z.LastWriteTimeUtc;
                        fItem.Name = z.Name;
                    }
                    finally
                    {
                        if (fItem != null) Kernel.ReleaseComponent(fItem);
                    }

                    return fItem;
                }).ToArray();
                return items;
            }
            catch (Exception ex)
            {
                throw new FileAccessException("Can't get files from directory", directoryPath, ex);
            }
        }

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

        public bool AreEqualPaths(string path1, string path2)
        {
            path1 = path1.TrimOrDefault();
            path2 = path2.TrimOrDefault();
            if (path1 == path2) { return true; } // In case both are null.

            path1 = path1.ReplaceBackWithForwardSlashes();
            path2 = path2.ReplaceBackWithForwardSlashes();
            if (path1.EndsWith("/")) { path1 = path1.TrimEnd(new char[] { '/' }); }
            if (path2.EndsWith("/")) { path2 = path2.TrimEnd(new char[] { '/' }); }

            if (path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase)) { return true; }
            return false;
        }

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
        /// <param name="createDirectory">Optional: create the new unique directory.</param>
        /// <returns>Return the non-existing unique directory name incl. <paramref name="baseDirectory"/></returns>

        public string GenerateUniqueDirectoryName(string baseDirectory, string baseDirectoryName, string dirExtension = null, bool createDirectory = false)
        {
            List<string> dosReservedWord = new List<string>() { "CON", "COM1", "COM2", "COM3", "COM4", "PRN", "AUX", "LPT1" };

            if (dirExtension == null)
            {
                var newPath = this.CombinePaths(baseDirectory, baseDirectoryName);
                var count = 1;
                while (this.DirectoryExists(newPath))
                {
                    newPath = this.CombinePaths(baseDirectory, baseDirectoryName + count.ToString());
                    count++;
                }
                if (createDirectory) { this.CreateDirectory(newPath); }
                return newPath;
            }
            else
            {
                var newPath = this.CombinePaths(baseDirectory, baseDirectoryName + dirExtension);
                var count = 1;

                while (this.DirectoryExists(newPath) || dosReservedWord.Any(w => baseDirectoryName.ToUpper().Equals(w)))
                {
                    baseDirectoryName = baseDirectoryName + count.ToString();
                    newPath = this.CombinePaths(baseDirectory, baseDirectoryName + dirExtension);
                    count++;
                }
                if (createDirectory) { this.CreateDirectory(newPath); }
                return newPath;
            }
        }

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

        public string GenerateUniqueFileName(string baseDirectory, string baseFileName, string fileExtension)
        {
            var newFile = this.CombinePaths(baseDirectory, baseFileName + fileExtension);
            var count = 1;
            while (this.FileExists(newFile))
            {
                newFile = this.CombinePaths(baseDirectory, baseFileName + count.ToString() + fileExtension);
                count++;
            }
            return newFile;
        }

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

        public string ShortenFileName(string fileName, int maxLength, bool hasExtension = false)
        {
            if (fileName.Length <= maxLength) { return fileName; }

            if (hasExtension)
            {
                string tempFileName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                var subLength = maxLength - extension.Length;
                DBC.Assert(subLength > 0, string.Format("ShortenFileName failed because the extension is longer or equal than the maxLength ({0}) for '{1}'.", maxLength, fileName));
                return fileName.Substring(0, subLength) + extension;
            }
            else
            {
                var shortenedName = fileName.Substring(0, maxLength);
                return shortenedName;
            }
        }

        /// <summary>
        /// Remove <paramref name="dir"/> if it is empty.
        /// </summary>
        /// <param name="dir">Directory to check/remove if empty</param>

        public void RemoveEmptyDirectory(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFileSystemInfos().Count() == 0)
            {
                this.DeleteDirectory(dirInfo.FullName, false);
            }
        }

        /// <summary>
        /// Constructor used by IoC.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="k"></param>

        public FileAccess(IKernel k, ILogger l)
        {
            this.Kernel = k;
            this.Logger = l;
        }

        private ILogger Logger { get; set; }
        private IKernel Kernel { get; set; }
    }
}
