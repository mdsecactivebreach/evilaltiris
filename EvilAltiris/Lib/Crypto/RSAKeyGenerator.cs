using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace EvilAltiris.Lib.Crypto
{
    public class RSAKeyGenerator
    {
        // SMA constants
        public static readonly byte NonSMA_Key_Version = 15;
        public static readonly byte SMA_Key_Version = 0;
        public static readonly byte SMA_Key_Flags = 0;
        public static readonly int SMAkey_modulus_size = 256;
        public static readonly int SMAkey_exponent_size = 3;
        public static readonly int SMAkey_parity_size = 1;

        public static byte[] SmaCspBlobFromPublicKey(byte[] Modulus, byte[] Exponent)
        {

            if (Exponent.Length > SMAkey_exponent_size)
                throw new ArgumentException("[!] Exponent too large on passed key");

            // Exponent should be 3 bytes
            byte[] exponentArray = new byte[SMAkey_exponent_size];
            Array.Copy(Exponent, 0, exponentArray, exponentArray.Length - Exponent.Length, Exponent.Length);

            // Create the final CSP array with header, exponent, and modulus
            byte[] destinationArray = new byte[2 + SMAkey_parity_size + exponentArray.Length + Modulus.Length];

            // Add header bytes: version and flags
            destinationArray[0] = SMA_Key_Version;
            destinationArray[1] = SMA_Key_Flags;

            // Null byte at the start of exponent section
            destinationArray[2] = (byte)0;

            // Copy the exponent to the destination array after header and parity
            Array.Copy(exponentArray, 0, destinationArray, 2 + SMAkey_parity_size, exponentArray.Length);

            // Copy the modulus after the exponent
            Array.Copy(Modulus, 0, destinationArray, 2 + SMAkey_parity_size + exponentArray.Length, Modulus.Length);

            return destinationArray;
        }   

        public void GenerateKeyPair()
        {

            Console.WriteLine("[+] Generating new asymmetric key pair for Agent encryption...");

            // Generate a new RSA key pair
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                // Export RSA parameters
                RSAParameters rsaParameters = rsa.ExportParameters(false);

                // Convert the modulus and exponent to base64 strings for readability
                string modulusBase64 = Convert.ToBase64String(rsaParameters.Modulus);
                string exponentBase64 = Convert.ToBase64String(rsaParameters.Exponent);

                // Print out the modulus and exponent
                Console.WriteLine("[+] Public Key Modulus (Base64): " + modulusBase64);
                Console.WriteLine("[+] Public Key Exponent (Base64): " + exponentBase64);

                Console.WriteLine("[+] Generating SMA CSP blob (policyKey)...");
                byte[] SmaCspBlob = SmaCspBlobFromPublicKey(rsaParameters.Modulus, rsaParameters.Exponent);
                Console.WriteLine(Convert.ToBase64String(SmaCspBlob));

                // Export the private key in XML format
                string privateKeyXml = rsa.ToXmlString(true);
                Console.WriteLine("[+] Private Key (XML): " + privateKeyXml);
            }
        }
    }
}
