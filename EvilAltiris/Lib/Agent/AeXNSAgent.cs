using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilAltiris.Lib.Agent
{
    class AeXNSAgent
    {
        public static void ClearMachineGuid()
        {
            try
            {
                Console.WriteLine("[+] Calling ClearMachineGuid() function...");
                dynamic comObject = Activator.CreateInstance(Type.GetTypeFromProgID("Altiris.AeXClient.1"));
                comObject.ClearMachineGuid();
                Console.WriteLine("[+] Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("\r\n[!] Unhandled exception:\r\n");
                Console.WriteLine(e);
            }
        }
    }
}
