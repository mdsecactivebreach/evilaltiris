using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EvilAltiris.Lib.Crypto
{
    class PolicyDataDecryptor
    {
        private SymmetricKeyEncryption.enc_header header = new SymmetricKeyEncryption.enc_header();    
        private int nBuffOffset;
        private Aes m_symmetricKey;
        public byte[] DecryptPolicyHeader(byte[] data, string privateKeyXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    // Import the private key from the XML string
                    rsa.FromXmlString(privateKeyXml);
                    int num = 0;
                    int int16 = (int)BitConverter.ToInt16(data, 0);
                    int srcOffset = num + 2;
                    int count = rsa.KeySize / 8;
                    byte[] numArray = new byte[count];
                    Buffer.BlockCopy((Array)data, srcOffset, (Array)numArray, 0, count);
                    this.nBuffOffset = srcOffset + count;
                    byte[] array = rsa.Decrypt(numArray, false);

                    this.header.FromByteArray(ref array);
                    Console.WriteLine("[+] Got symmetric key from header: (m_key) {0}", Convert.ToBase64String(this.header.m_key));
                    Console.WriteLine("[+] Got IV from header: (m_IV) {0}", Convert.ToBase64String(this.header.m_IV));
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[!] Failed to get symmetric key from header" + ex.Message);
                    return null;
                }
            }
        }

        public void DecryptPolicyData(byte[] data)
        {
            try
            {
                MemoryStream result = SymmetricKeyEncryption.DecryptToMemoryStream(data, nBuffOffset, header, this.m_symmetricKey);
                result.Position = 0;

                // Convert MemoryStream to string and print
                using (var reader = new StreamReader(result, Encoding.UTF8))
                {
                    string text = reader.ReadToEnd();
                    Console.WriteLine($"[+] Decrypted Policy Data:\n{text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed to decrypt policy data: " + ex.Message);
                return;
            }
        }
    }
}