using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Core.Common
{
    /// <summary>
    /// Helper class to hash data.
    /// </summary>

    public interface IHashHelper
    {
         /// <summary>
        /// Create hash string from input.
        /// </summary>
        /// <param name="input">String to hash.</param>
        /// <returns>Hashed input string.</returns>

        string CalculateMD5Hash(string input);
    }
}
