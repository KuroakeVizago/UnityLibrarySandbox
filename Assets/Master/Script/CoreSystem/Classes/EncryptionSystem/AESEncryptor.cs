using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Vizago.Security
{
    public static class AesEncryptor
    {
        
        /// <summary>
        /// Encrypt a data into an array of byte
        /// </summary>
        /// <param name="data"> String data to encrypt </param>
        /// <param name="password"> The password for encryption (Must be 32 chars of string) </param>
        /// <returns> Encrypted Data in Byte[] </returns>
        public static byte[] EncryptStringToBytes(in string data, string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            
            // Required password in byte array
            var cacheKey = Encoding.ASCII.GetBytes(password);
            
            // The standard for initial vector is 16
            var cacheIv = new byte[16];
            
            return EncryptStringToBytes(data, cacheKey, cacheIv);

        }

        /// <summary>
        /// Decrypt an array of byte into string data
        /// </summary>
        /// <param name="encryptedData"> The encrypted data before in array of byte </param>
        /// <param name="password"> The password for decryption. Password must be the same that being used for encryption ! (Must be 32 chars of string) </param>
        /// <returns> String data after decryption </returns>
        public static string DecryptBytesToString(in byte[] encryptedData, string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            
            // Required password in byte array
            var cacheKey = Encoding.ASCII.GetBytes(password);
            
            // The standard for initial vector is 16
            var cacheIv = new byte[16];
            
            return DecryptStringFromBytes(encryptedData, cacheKey, cacheIv);

        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;

            // Create an encryptor to perform the stream transform.
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            
            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an Aes object
            // with the specified key and IV.
            var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;

            // Create a decryptor to perform the stream transform.
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Create the streams used for decryption.
            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }
    }
}
