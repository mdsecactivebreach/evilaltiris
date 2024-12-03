using System;
using Microsoft.Win32;

namespace EvilAltiris.Lib
{
    public class RegistryReader
    {
        private const string RegistryPath = @"SOFTWARE\Altiris\Altiris Agent";
        private const string RegistryKey = "MachineGuid";

        public static void GetMachineGuid()
        {
            try
            {
                // Open the registry key in read-only mode
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryPath))
                {
                    if (key != null)
                    {
                        // Read the MachineGuid value
                        object value = key.GetValue(RegistryKey);
                        if (value != null)
                        {
                            Console.WriteLine($"[+] Found MachineGuid value: {value}");
                        }
                        else
                        {
                            Console.WriteLine("[!] MachineGuid value not found.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[!] Registry path not found.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error reading registry: {ex.Message}");
                return;
            }
        }
    }
}