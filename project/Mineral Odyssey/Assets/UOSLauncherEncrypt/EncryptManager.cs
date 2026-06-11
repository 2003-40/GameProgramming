using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using UnityEngine;

namespace Unity.UOS.Encrypt
{
    /// <summary>
    /// Utility wrapper for encrypting and decrypting UOS launcher strings with AES.
    /// </summary>
    public class EncryptManager
    {
        private static string m_EncryptKey => EncryptKey.Value;

        public static string Encrypt(string plainText)
        {
            // Use a deterministic IV to match the launcher helper's expected output format.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKeyFromPassword(m_EncryptKey);
                aes.IV = new byte[16];
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            // Invalid cipher text is logged and converted to an empty string for caller safety.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = DeriveKeyFromPassword(m_EncryptKey);
                    aes.IV = new byte[16];
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream =
                               new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            byte[] plainTextBytes = memoryStream.ToArray();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return String.Empty;
        }

        private static byte[] DeriveKeyFromPassword(string password)
        {
            // PBKDF2 converts the configured string key into the 128-bit AES key used above.
            var salt = "202401121404";
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            return new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), iterations)
                .GetBytes(desiredKeyLength);
        }
    }
}
