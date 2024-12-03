using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EvilAltiris.Lib.Crypto
{
    class SymmetricKeyEncryption
    {
        internal class PolicySymmetricKeyGenerator
        {
            internal static readonly byte[] kZeroVector = new byte[16];
        }
        internal class enc_header
        {
            private const short kHeaderVersion = 1;
            public short m_nHeaderVersion = 1;
            public byte[] m_key = new byte[32];
            public byte[] m_hashkey = new byte[32];
            public byte[] m_IV = new byte[16];
            public const int HeaderSerializedSize = 82;

            public bool ContainsSUKey() => ((int)this.m_nHeaderVersion & (int)byte.MaxValue) == 1;

            public void FromByteArray(ref byte[] array)
            {
                int num = 0;
                this.m_nHeaderVersion = BitConverter.ToInt16(array, 0);
                int srcOffset1 = num + 2;
                Buffer.BlockCopy((Array)array, srcOffset1, (Array)this.m_key, 0, 32);
                int srcOffset2 = srcOffset1 + 32;
                Buffer.BlockCopy((Array)array, srcOffset2, (Array)this.m_IV, 0, 16);
                int srcOffset3 = srcOffset2 + 16;
                Buffer.BlockCopy((Array)array, srcOffset3, (Array)this.m_hashkey, 0, 32);
            }
        }

        public class XorCounter
        {
            private readonly byte[] m_counter;

            public XorCounter(int nLen) => this.m_counter = nLen <= 16 ? new byte[nLen] : throw new Exception(string.Format("[!] XorCounter max counter length {0} bytes", 16.ToString(CultureInfo.InvariantCulture)));

            public static SymmetricKeyEncryption.XorCounter operator ++(SymmetricKeyEncryption.XorCounter a)
            {
                for (int index = a.m_counter.Length - 1; index >= 0; --index)
                {
                    if (a.m_counter[index] < byte.MaxValue)
                    {
                        ++a.m_counter[index];
                        return a;
                    }
                    a.m_counter[index] = (byte)0;
                }
                throw new Exception("[!] XorCounter overflow");
            }

            public void Xor(byte[] array)
            {
                int length = this.m_counter.Length;
                for (int index = 0; index < length; ++index)
                    array[array.Length - 1 - index] ^= this.m_counter[index];
            }
        }

        private static void CTRCrypt(byte[] data, byte[] vector, long lStartOffset, long lLength, ICryptoTransform encryptor, Stream memoryStream)
        {
            SymmetricKeyEncryption.XorCounter xorCounter = new SymmetricKeyEncryption.XorCounter(4);
            byte[] outputBuffer = new byte[16];
            byte[] numArray = new byte[vector.Length];
            long num = 0;
            while (num < lLength)
            {
                Buffer.BlockCopy((Array)vector, 0, (Array)numArray, 0, vector.Length);
                xorCounter.Xor(numArray);
                encryptor.TransformBlock(numArray, 0, 16, outputBuffer, 0);
                for (int index = 0; index < 16 && num < lLength; ++index)
                    memoryStream.WriteByte((byte)(data[(int)(lStartOffset + num++)] ^ outputBuffer[index]));
                ++xorCounter;
            }
        }

        internal static MemoryStream DecryptToMemoryStream(byte[] data, int nBuffOffset, SymmetricKeyEncryption.enc_header header, Aes symmetricKey, int dataLength = -1)
        {
            nBuffOffset += !header.ContainsSUKey() ? 0 : 112;

            try
            {
                MemoryStream memoryStream = new MemoryStream();

                using (SymmetricAlgorithm AesSymmetricKey = Aes.Create())
                {
                    // Set properties for the encryption algorithm
                    AesSymmetricKey.KeySize = 256;
                    AesSymmetricKey.BlockSize = 128;
                    AesSymmetricKey.Mode = CipherMode.ECB;
                    AesSymmetricKey.Padding = PaddingMode.None;

                    using (ICryptoTransform encryptor = AesSymmetricKey.CreateEncryptor(header.m_key, PolicySymmetricKeyGenerator.kZeroVector))
                        SymmetricKeyEncryption.CTRCrypt(data, header.m_IV, (long)nBuffOffset, (long)(data.Length - nBuffOffset - 32), encryptor, (Stream)memoryStream);
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed to decrypt policy data: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                return null;
            }
        }
    }
}
