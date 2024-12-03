using System;
using EvilAltiris.Lib;
using EvilAltiris.Lib.Crypto;
using EvilAltiris.Lib.Agent;
using EvilAltiris.Lib.CmdLine;
using EvilAltiris.Lib.SMATool;

namespace EvilAltiris
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = Options.ParseArgs(args);
            var commandName = args.Length != 0 ? args[0] : "";

            Options.Arguments arguments = Options.ArgumentValues(parsedArgs);
            arguments.Data = DataHandler.ConvertToBase64(arguments.Data);

            if (!Options.ValidateRequiredArgs(commandName, parsedArgs))
            {
                Info.ShowUsage();
                return;
            }       

            {
                Info.ShowLogo();

                try
                {
                    if (commandName.ToLower() == "smagettypeguid")
                    {
                        new SMAToolRunner(arguments.Smapath).Run("gettypeguid", arguments.Outfile);
                    }
                    else if (commandName.ToLower() == "setpublickey")
                    {
                        new PolicyClient(arguments.Url).SetAgentPublicKey(arguments.Machine, arguments.Key);
                    }
                    else if (commandName.ToLower() == "getmachineguid")
                    {
                        RegistryReader.GetMachineGuid();
                    }
                    else if (commandName.ToLower() == "getclientpolicies")
                    {
                        new PolicyClient(arguments.Url).GetClientPolicies(arguments.Machine, arguments.Type, arguments.Outfile);
                    }
                    else if (commandName.ToLower() == "smagetpublickey")
                    {
                        new SMAToolRunner(arguments.Smapath).Run("getpublickey", arguments.Outfile);
                    }
                    else if (commandName.ToLower() == "smadecrypt")
                    {
                        new SMAToolRunner(arguments.Smapath).Run("decrypt", arguments.Outfile, arguments.Data);
                    }
                    else if (commandName.ToLower() == "decryptpolicy")
                    {
                        PolicyDataDecryptor decryptor = new PolicyDataDecryptor();
                        byte[] result = decryptor.DecryptPolicyHeader(Convert.FromBase64String(arguments.Data), arguments.Key);
                        decryptor.DecryptPolicyData(result);
                    }
                    else if (commandName.ToLower() == "decryptacc")
                    {
                        AccDecryptor.DecryptACC(arguments.Data);
                    }
                    else if (commandName.ToLower() == "generatekeys")
                    {
                        new RSAKeyGenerator().GenerateKeyPair();
                    }
                    else if (commandName.ToLower() == "restoreagent")
                    {
                        AeXNSAgent.ClearMachineGuid();
                    }
                    else
                    {
                        Info.ShowUsage();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\r\n[!] Unhandled exception:\r\n");
                    Console.WriteLine(e);
                }
            }
        }
    }
}