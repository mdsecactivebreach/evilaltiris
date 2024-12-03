using System;
using System.IO;

namespace EvilAltiris.Lib
{
    class DataHandler
    {
        public static string ConvertToBase64(string dataArgument)
        {
            // Check if the data argument is a valid file path
            if (File.Exists(dataArgument))
            {
                // Read the file content as binary
                byte[] fileBytes = File.ReadAllBytes(dataArgument);

                // Convert the binary content to a base64 string
                string base64Data = Convert.ToBase64String(fileBytes);

                return base64Data;
            }
            else
            {
                // Assume the argument is already base64 if it's not a file path
                return dataArgument;
            }
        }

        public static void WriteBase64ToFile(string base64String, string filePath)
        {
            try
            {
                // Decode the Base64 string to a byte array
                byte[] fileBytes = Convert.FromBase64String(base64String);

                // Write the byte array to the specified file path
                File.WriteAllBytes(filePath, fileBytes);

                Console.WriteLine($"[+] File written successfully to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] An unexpected error occurred: " + ex.Message);
            }
        }

        public static void WriteToFile(string inputString, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, inputString);

                Console.WriteLine($"[+] File written successfully to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] An unexpected error occurred: " + ex.Message);
            }
        }
    }
}