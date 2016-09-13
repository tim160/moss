using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EC.Common.Base
{
    /// <summary>
    /// Helper for path related manipulations (e.g. create an absolute file path or fix a path).
    /// </summary>

    public static class PathHelper
    {
        /// <summary>
        /// Check the format of the path.
        /// A path must start with a "/" and not end with a "/".
        /// Must not contain "\".
        /// Must not have 2 "/" right beside each other (e.g. "//Root").
        /// Must not contain only white spaces between two "/".
        /// <remarks>
        /// Process the path with <see cref="FixPath(path, trimEndingSlash)"/> before calling this.
        /// </remarks>
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Return <c>true</c> if the <paramref name="path"/> has the correct format. Return <c>false</c> if the format is wrong.</returns>

        public static bool CheckPathFormat(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return false; }
            if (!path.StartsWith(PathConstants.ElementSeparator)) { return false; }
            if (path.EndsWith(PathConstants.ElementSeparator)) { return false; }
            if (path.Contains("\\")) { return false; }

            var pathParts = path.SplitStringBy(new char[] { '/' }, true);
            if (pathParts.Any(p => p == string.Empty)) { return false; }
            if (pathParts.Any(p => string.IsNullOrWhiteSpace(p) == true)) { return false; }

            return true;
        }

        /// <summary>
        /// Ensure that the given path starts with a "/" and optionally (<paramref name="trimEndingSlash"/>) trims an ending "/".
        /// </summary>
        /// <param name="path">input path</param>
        /// <param name="trimEndingSlash">Optional: set to <c>true</c> to get rid of an ending '/'. Set <c>false</c> to leave it there if it exists (it is not added).</param>
        /// <returns>path with "/" prefix</returns>

        public static string FixPath(string path, bool trimEndingSlash = false)
        {
            if (path == null) { return null; }
            if (string.IsNullOrWhiteSpace(path)) { return path; }
            if (path.EndsWith(PathConstants.ElementSeparator) && trimEndingSlash) { path = path.TrimEnd(new char[] { '/' }); }
            if (path.StartsWith(PathConstants.ElementSeparator)) { return path; }
            return PathConstants.ElementSeparator + path;
        }

        /// <summary>
        /// Create a URL that refers directly to a file in a file collection. The caller must provide
        /// the path to the NavPage where the File Collection resides, and the relative path to the
        /// file within that File Collection. The resulting path can be appended to the 
        /// URL for the content controller to produce a complete absolute URL that will fetch the
        /// contents of the file.
        /// </summary>

        public static string CreateFileURLPath(string navPagePath, string relativePath)
        {
            if (string.IsNullOrEmpty(navPagePath)) { return null; }
            if (relativePath == null) { throw new ArgumentException("Cannot construct a file path without a relative path"); }
            if (IsFilePath(navPagePath)) { throw new ArgumentException("Cannot build a file path from another file path"); }
            if (relativePath == null) { return PathHelper.FixPath(navPagePath); }
            navPagePath = navPagePath.TrimEnd(new char[] { '/' });
            relativePath = relativePath.TrimStart(new char[] { '/' });
            return String.Format("{0}{1}{2}", navPagePath, PathConstants.PathSeparatorEnclosed, relativePath);
        }

        /// <summary>
        /// Predicate indicating whether the given path is for a simple content item.
        /// </summary>
        /// <remarks>
        /// The implementation assumes that the path component '/-File/' is reserved
        /// and never occurs except at the marker for a simple content item.
        /// </remarks>
        /// <param name="path">the path</param>
        /// <returns>true if the path is for a simple content item</returns>

        public static bool IsFilePath(string path)
        {
            if (path.Contains(PathConstants.PathSeparatorEnclosed, StringComparison.InvariantCultureIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// Check whether the <paramref name="path"/> is a relative path.
        /// A relative path starts either with '~/' or '../'.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>
        /// Return <c>true</c> if the path starts with '~/' or '../'. 
        /// Return <c>false</c> for any other <paramref name="path"/> (even if <paramref name="path"/> is <c>null</c>).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the path is a relative path with '..' and contains '..' other than at the start of the path (i.e. ../Gorge/../Home.css)
        /// </exception>

        public static bool IsRelativePath(string path)
        {
            if (IsLocalPath(path))
            {
                return true;
            }
            else
            {
                if (GetRelativePathLevel(path) > 0) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Check whether the <paramref name="path"/> is a local path.
        /// A local path is considered if it starts with '~/'.
        /// This refer, for example, to a local FileCollection.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>
        /// Return <c>true</c> if the path starts with '~/'. 
        /// Return false in any other case (even if <paramref name="path"/> is <c>null</c>
        /// </returns>

        public static bool IsLocalPath(string path)
        {
            if (string.IsNullOrEmpty(path)) { return false; }
            if (path.StartsWith("~/")) { return true; }
            return false;
        }

        /// <summary>
        /// Determines whether the specified path is a path.
        /// </summary>
        /// <remarks>
        /// A string is considered a path if 
        /// - string is not empty or only contains of white spaces
        /// - starts with ~/
        /// - or starts with ../
        /// - or starts with /
        /// - and doesn't contain '\'
        /// </remarks>
        /// <param name="path">The path.</param>
        /// <returns></returns>

        public static bool IsPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return false; }
            if (path.Contains("\\")) { return false; }
            if (path.StartsWith("~" + PathConstants.ElementSeparator) || path.StartsWith(".." + PathConstants.ElementSeparator) || path.StartsWith(PathConstants.ElementSeparator)) { return true; }

            return false;
        }


        /// <summary>
        /// Get how many '..' exist in the path.
        /// '..' in a <paramref name="path"/> are only valid if they are at the start of the path.
        /// </summary>
        /// <example>
        /// ../../Home => PathLevel = 2
        /// /Root/Home => PathLevel = 0
        /// ~/Home => PathLevel = 0
        /// null => PathLevel = 0
        /// ../../Home/../ => throw ArgumentException
        /// </example>
        /// <param name="path">Path (can be absolute as well)</param>
        /// <returns>Return how many levels ('..') the <paramref name="path"/> has. Return 0 if it doesn't contain any '..' and therefore is absolute</returns>
        /// <exception cref="ArgumentException">If the path contains '..' other than at the start of the path (i.e. ../Gorge/../Home.css)</exception>

        public static int GetRelativePathLevel(string path)
        {
            if (string.IsNullOrEmpty(path)) { return 0; }
            var pathParts = path.SplitStringBy(new char[] { '/' }, true);
            int relativePathLevel = pathParts.Where(z => z.Equals("..")).Count();
            var prefixLevelCount = pathParts.Take(relativePathLevel).Where(z => z.Equals("..")).Count();

            if (relativePathLevel != prefixLevelCount)
            {
                throw new ArgumentException(
                    string.Format("Could not parse path because it has \"..\" within the relative path. Relative \"..\" can only be used at the beginning of the path. RelativePath={0}",
                    path));
            }
            return relativePathLevel;
        }

        /// <summary>
        /// Take a path and return the path to the parent item.
        /// </summary>
        /// <remarks>
        /// This is just a matter of trimming off the last element of the path.
        /// The path must be separated with '/'.
        /// </remarks>
        /// <param name="path">original path</param>
        /// <returns>path to the parent</returns>

        public static string ParentPath(string path)
        {
            var pathElements = path.SplitStringBy(new char[] { '/' }, true);
            if (pathElements.Count() <= 1) return "/";
            return "/" + pathElements.Take(pathElements.Count() - 1).Aggregate((a, b) => a + "/" + b);
        }

        /// <summary>
        /// Get Url name of a path (= last part of the path).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>

        public static string GetUrlNameFromPath(string path)
        {
            // Get Url name of the path...
            var subPath = path.TrimEnd(new char[] { '/' });
            return subPath.Substring(subPath.LastIndexOf('/') + 1);
        }

        /// <summary>
        /// If the path contains an absolute path and a relative file part - only return the absolute path part.
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Return the absolute path part. If the <paramref name="path"/> doesn't contain a relative file part, return the original path.</returns>
        /// <returns>Portion of path that specifies the Link</returns>

        public static string GetAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path)) { return null; }
            if (!path.Contains(PathConstants.PathSeparatorEnclosed, StringComparison.InvariantCultureIgnoreCase)) { return path; }
            return Regex.Split(path, "/" + PathConstants.PathSeperator, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Where(s => s != "").FirstOrDefault();
        }

        /// <summary>
        /// Returns the relative file path for the <paramref name="path"/> which refers to a disk file in
        /// a file collection. Returns null if the path does not refer to a simple file.
        /// </summary>
        /// <param name="path">Path to parse</param>
        /// <returns>Return the relative file path. If the <paramref name="path"/> doesn't contain a relative file part with a leading '/', return <c>null</c>.</returns>

        public static string GetRelativeFilePath(string path)
        {
            if (string.IsNullOrEmpty(path)) { return null; }
            if (!path.Contains(PathConstants.PathSeparatorEnclosed, StringComparison.InvariantCultureIgnoreCase)) { return null; }
            return Regex.Split(path, "/" + PathConstants.PathSeperator, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Where(s => s != "").Skip(1).FirstOrDefault();
        }

        /// <summary>
        /// Check if the path contains a relative file path part ('/-File/'). 
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Return <c>true</c> if the path contains '/-File/'. Return <c>false</c> otherwise</returns>

        public static bool HasRelativeFilePath(string path)
        {
            if (string.IsNullOrEmpty(path)) { return false; }
            if (path.Contains(PathConstants.PathSeparatorEnclosed, StringComparison.InvariantCultureIgnoreCase)) { return true; }
            return false;
        }

        /// <summary>
        /// Create a 3 level directory structure from a <c>Guid</c>.
        /// <remarks>
        /// The first 8-10 characters change the most when using sequential Guid.
        /// Therefore the selection of the characters are as follows:
        /// \id[3]id[6]\id[4]id[7]\id\ 
        /// where id[xx] is the character at position xx in the <c>Guid</c> and 'id' is the full Guid.
        /// </remarks>
        /// <example>
        /// Guid: d421de82-eb2d-4194-aa8f-089c76a1f10e 
        /// path: \d6\a8\d421de82-eb2d-4194-aa8f-089c76a1f10e\ 
        /// </example>
        /// </summary>
        /// <param name="id">Id to generate the 3 level path from</param>
        /// <returns></returns>

        public static string Generate3LevelPathFromGuid(Guid id)
        {
            StringBuilder strBuilder = new StringBuilder();
            string strId = id.ToString();

            strBuilder.Append("\\");
            strBuilder.Append(strId[3]).Append(strId[6]).Append("\\");
            strBuilder.Append(strId[4]).Append(strId[7]).Append("\\");
            strBuilder.Append(strId).Append("\\");

            return strBuilder.ToString();
        }

        /// <summary>
        /// Test if the 2 paths are equal.
        /// '/' at the beginning or end are ignored.
        /// </summary>
        /// <param name="path1">First path to check.</param>
        /// <param name="path2"></param>
        /// <returns></returns>

        public static bool ArePathsEqual(string path1, string path2)
        {
            if (path1 == path2) { return true; }

            if (!string.IsNullOrEmpty(path1))
            {
                path1 = path1.Trim('/');
            }
            if (!string.IsNullOrEmpty(path2))
            {
                path2 = path2.Trim('/');
            }

            if (path1 == path2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Extend <paramref name="path "/> by the <paramref name="urlName"/>.
        /// </summary>
        /// <param name="path">parent path you want to extend</param>
        /// <param name="urlName">urlName of childlink</param>
        /// <returns>return extended path</returns>

        public static string ExtendPath(string path, string urlName)
        {
            return path + "/" + urlName;
        }

        /// <summary>
        /// Combine the two paths to one (with leading '/').
        /// </summary>
        /// <param name="path1">First part of the path</param>
        /// <param name="path2">Second part of the path</param>
        /// <returns>Return the combined path (path1 + path2). Trailing '/' will be removed.</returns>

        public static string CombinePaths(string path1, string path2)
        {
            StringBuilder strB = new StringBuilder();
            FixPath(strB, path1);
            if (!path1.EndsWith("/"))
            {
                strB.Append("/");
            }
            strB.Append(path2.Trim('/'));
            return strB.ToString();
        }

        /// <summary>
        /// Determines whether the parent is a parent path of the child.
        /// </summary>

        public static bool IsPathChildOfParentPath(string childPath, string parentPath)
        {
            childPath = FixPath(childPath, true).ToUpperInvariant();
            parentPath = FixPath(parentPath, true).ToUpperInvariant();

            return childPath.IndexOf(parentPath, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Append path with leading '/' using string builder.
        /// </summary>
        /// <param name="strBuilder">String builder to append the fixed path to.</param>
        /// <param name="path">Path to fix.</param>

        private static void FixPath(StringBuilder strBuilder, string path)
        {
            if (path == null) { return; }

            if (path.StartsWith("/"))
            {
                strBuilder.Append(path);
            }
            else
            {
                strBuilder.Append("/");
                strBuilder.Append(path);
            }
        }
    }
}