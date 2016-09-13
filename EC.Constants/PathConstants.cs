using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Constants related to Path to Content in EC
    /// </summary>

    public static class PathConstants
    {
        /// <summary>
        /// '-File' - The Separator between the File Collection path and the 
        /// relative File Path within the File Collection
        /// </summary>

        public const string PathSeperator = "-File";
 
        /// <summary>
        /// '/-File/' - The Separator with leading and trailing '/' between the File Collection path and the 
        /// relative File Path within the File Collection
        /// </summary>
      
        public const string PathSeparatorEnclosed = "/-File/";

        /// <summary>
        /// The character (represented as a string), that separates the components in a path.
        /// </summary>
        
        public const string ElementSeparator = "/";
    }
}
