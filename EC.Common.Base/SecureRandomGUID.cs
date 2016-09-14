using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MarineLMS.Core.Base
{
    /// <summary>
    /// Generate a strongly random Guid.
    /// Uses the <c>System.Security.Cryptography.RandomNumberGenerator</c> to generate the Guid.
    /// </summary>
    
    public class SecureRandomGUID
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
       
        private static RandomNumberGenerator RandomNumberGen = RandomNumberGenerator.Create();

        /// <summary>
        /// Generate cryptographically strong random Guid.
        /// </summary>
        /// <returns>Return a cryptographically strong random Guid.</returns>
       
        public static Guid Create()
        {
            byte[] randomId = new byte[16];  // 128bit for the Guid.
            SecureRandomGUID.RandomNumberGen.GetBytes(randomId); // Get 16 random bytes.
            return new Guid(randomId); // Convert bytes into a Guid.
        }
    }
}
