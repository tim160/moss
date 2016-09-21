using System;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel;
using EC.Common.Interfaces;


namespace EC.Core.Common
{
    /// <summary>
    /// Helper class to hash data.
    /// </summary>
    
    [SingletonType]
    [RegisterAsType(typeof(IHashHelper))]
    
    public class HashHelper : IHashHelper, IDisposable
    {      
        /// <summary>
        /// Create hash string from input. The string returned is the hexadecimal representation
        /// of the MD5 hash in the input string.
        /// </summary>
        /// <param name="input">String to hash.</param>
        /// <returns>Hashed input string.</returns>
       
        public string CalculateMD5Hash(string input)
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = null;

            using (var s = new ScopedLock(ProviderLock))
            {
                hash = MD5Provider.ComputeHash(inputBytes);
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// IoC object creation entry point
        /// </summary>
        /// <remarks>
        /// NOTE: The Create() call on the MD5 provider is thread safe, but other methods
        /// are not.
        /// </remarks>

        public HashHelper(IKernel k, ILogger l, ILock lk)
        {
            Kernel = k;
            Logger = l;
            ProviderLock = lk;
            MD5Provider = System.Security.Cryptography.MD5.Create();
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        
        public void Dispose()
        {
            if (MD5Provider != null) { MD5Provider.Dispose(); }
            MD5Provider = null;
        }
        
        /// <summary>
        /// MD5 hasher.
        /// </summary>

        private System.Security.Cryptography.MD5 MD5Provider = null;

        // ----------------------------- Injected Components --------------------------------------

        private readonly IKernel Kernel = null;
        private readonly ILogger Logger = null;
        private readonly ILock ProviderLock = null;
    }
}
