using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace EC.Core.Common
{
    /// <summary>
    /// This class implements the Advanced Encryption Standard (AES), also called Rijndael. 
    /// AES is symmetric, same key is used for encryption and decryption.
    /// AES is a family of ciphers with different key and block sizes and is global encryption standard.
    /// </summary>
    
    public class CryptoSymmetricAes
    {
        private const string _saltAsciiDefault = "C5c0691zkbMX5r";  // Hard-coded default should be overrided by caller

        /// <summary>
        /// Encrypts text with a supplied password (must use same password to decrypt)
        /// </summary>
        /// <remarks>
        /// Salt is optional. If not set, default is used for all passwords though this is less secure. 
        /// Each unique password should have a unique salt for greatest security. 
        /// </remarks>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="password">Password use to encrypt</param>
        /// <returns>Encrypted cipher text</returns>
        /// <param name="salt">Optional Unique salt for password</param>

        public static string EncryptText(string plainText, string password, string salt = null)
        {
            if (string.IsNullOrEmpty(plainText)) { throw new ArgumentNullException("plainText"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }

            var aesAlgo = CreateAesAlgorithm(password, salt);
            var encryptor = aesAlgo.CreateEncryptor(aesAlgo.Key, aesAlgo.IV);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Prepend IV (length + data)
                    memoryStream.Write(BitConverter.GetBytes(aesAlgo.IV.Length), 0, sizeof(int));
                    memoryStream.Write(aesAlgo.IV, 0, aesAlgo.IV.Length);

                    using (var encryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(encryptStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            finally
            {
                if (aesAlgo != null) { aesAlgo.Clear(); }
            }
        }

        /// <summary>
        /// Decrypts text with a supplied password (must use same password to encrypt)
        /// </summary>
        /// <remarks>
        /// Salt is optional. If not set, default is used for all passwords though this is less secure. 
        /// Each unique password should have a unique salt for greatest security. 
        /// </remarks>
        /// <param name="cipherText">Encrypted text to decrypt</param>
        /// <param name="password">Password to use to decrypt</param>
        /// <returns>Decrypted plain text</returns>
        /// <param name="salt">Optional Unique salt for password</param>

        public static string DecryptText(string cipherText, string password, string salt = null)
        {
            if (string.IsNullOrEmpty(cipherText)) { throw new ArgumentNullException("cipherText"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }

            var aesAlgo = CreateAesAlgorithm(password, salt);

            try
            {
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    aesAlgo.IV = ReadIV(memoryStream);

                    var decryptor = aesAlgo.CreateDecryptor(aesAlgo.Key, aesAlgo.IV);

                    using (var decryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(decryptStream))
                        {
                            return streamReader.ReadToEnd();      // Read de-crypted bytes from stream and return
                        }
                    }
                }
            }
            finally
            {
                if (aesAlgo != null) { aesAlgo.Clear(); }
            }
        }

        /// <summary>
        /// Encrypt file with a supplied password (must use same password to decrypt)
        /// </summary>
        /// <remarks>
        /// Salt is optional. If not set, default is used for all passwords though this is less secure. 
        /// Each unique password should have a unique salt for greatest security. 
        /// </remarks>
        /// <param name="inFilePath">Path to file to encrypt</param>
        /// <param name="outFilePath">File path to save encrypted cipher text</param>
        /// <param name="password">Password used to encrypt file</param>
        /// <param name="salt">Optional Unique salt for password</param>

        public static void EncryptFile(string inFilePath, string outFilePath, string password, string salt = null)
        {
            if (string.IsNullOrEmpty(inFilePath)) { throw new ArgumentNullException("inFilePath"); }
            if (string.IsNullOrEmpty(outFilePath)) { throw new ArgumentNullException("outFilePath"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }

            var aesAlgo = CreateAesAlgorithm(password, salt);

            try
            {
                using (var outFileStream = new FileStream(outFilePath, FileMode.Create))
                {
                    var encryptor = aesAlgo.CreateEncryptor(aesAlgo.Key, aesAlgo.IV);
                    WriteIV(outFileStream, aesAlgo);

                    using (var cryptoStream = new CryptoStream(outFileStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var inFileStream = new FileStream(inFilePath, FileMode.Open))
                        {
                            int data;
                            while ((data = inFileStream.ReadByte()) != -1) { cryptoStream.WriteByte((byte) data); }
                        }
                    }
                }
            }
            finally
            {
                if (aesAlgo != null) { aesAlgo.Clear(); }
            }
        }

        /// <summary>
        /// Decrypt file with a supplied password (must use same password during encryption)
        /// </summary>
        /// <remarks>
        /// Salt is optional. If not set, default is used for all passwords though this is less secure. 
        /// Each unique password should have a unique salt for greatest security. 
        /// </remarks>
        /// <param name="inFilePath">Path to file to decrypt</param>
        /// <param name="outFilePath">File path to save decrypted plain text</param>
        /// <param name="password"></param>
        /// <param name="salt">Optional Unique salt for password</param>

        public static void DecryptFile(string inFilePath, string outFilePath, string password, string salt = null)
        {
            if (string.IsNullOrEmpty(inFilePath)) { throw new ArgumentNullException("inFilePath"); }
            if (string.IsNullOrEmpty(outFilePath)) { throw new ArgumentNullException("outFilePath"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }

            var encryptAlgo = CreateAesAlgorithm(password, salt);

            try
            {
                using (var inFileStream = new FileStream(inFilePath, FileMode.Open))
                {
                    encryptAlgo.IV = ReadIV(inFileStream);
                    var decryptor = encryptAlgo.CreateDecryptor(encryptAlgo.Key, encryptAlgo.IV);

                    using (var decryptStream = new CryptoStream(inFileStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var outFileStream = new FileStream(outFilePath, FileMode.Create))
                        {
                            int data;
                            while ((data = decryptStream.ReadByte()) != -1) { outFileStream.WriteByte((byte) data); }
                        }
                    }
                }
            }
            finally
            {
                if (encryptAlgo != null) { encryptAlgo.Clear(); }
            }
        }

        /// <summary>
        /// Create AES algorithm with supplied password. 
        /// Password is hashed with salt to create key used in algo (Hashed password protects against dictionary attacks)
        /// Initialization Vector (IV) is auto initialized and random when Rigndaelmanaged class is created. 
        /// IE  IV is different everytime encryption 
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Initialized RijndaelManaged class</returns>

        private static RijndaelManaged CreateAesAlgorithm(string password, string saltAscii = null)
        {
            var key = (saltAscii) != null
                            ? CreateKey(password, saltAscii)
                            : CreateKey(password, _saltAsciiDefault);

            // NOTE: 'aesAlgo.IV' is already random, leave as is
            var aesAlgo = new RijndaelManaged();
            aesAlgo.Key = key.GetBytes(aesAlgo.KeySize / 8);
            
            return aesAlgo;
        }

        /// <summary>
        /// Writes Initialization Vector to stream.
        /// IV is NOT encrypted and does not need to be secure. It is input to AES algorithm and initializes encryptor/decryptor. 
        /// Randomized IV creates different ciphers on successive encryptions on same plain text.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="symmetricAlgo"></param>

        private static void WriteIV(Stream stream, SymmetricAlgorithm symmetricAlgo)
        {
            // Prepend stream with unencrypted IV Length
            stream.Write(BitConverter.GetBytes(symmetricAlgo.IV.Length), 0, sizeof(int));
            // Prepend stream with unencrypted IV Data
            stream.Write(symmetricAlgo.IV, 0, symmetricAlgo.IV.Length);
        }

        /// <summary>
        /// Reads Initialization Vector from stream.
        /// IV is NOT encrypted and does not need to be secure. Read from start of unencrypted stream, IV initializes AES algorithm.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Initialization Vector read from stream</returns>

        private static byte[] ReadIV(Stream stream)
        {
            // Read IV Length
            var rawBytes = new byte[sizeof(int)];
            if (stream.Read(rawBytes, 0, rawBytes.Length) != rawBytes.Length) { throw new SystemException("Stream contains improper byte array format for IV"); }

            // Read IV Data
            var buffer = new byte[BitConverter.ToInt32(rawBytes, 0)];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length) { throw new SystemException("Error parsing IV byte array"); }

            return buffer;
        }

        /// <summary>
        /// Salt protects a password against dictionary attacks. Salt is public data combined with a secret password and secret hash function.
        /// IE  HashFunction(password + salt) = randomPassword 
        /// </summary>
        /// <returns>Byte array of Encoded ASCII salt string</returns>

        private static Rfc2898DeriveBytes CreateKey(string password, string saltAscii)
        {
            var saltBytes = Encoding.ASCII.GetBytes(saltAscii);
            return new Rfc2898DeriveBytes(password, saltBytes);
        }
    }
}
