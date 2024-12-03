using System;
using System.Collections.Generic;
using System.Linq;

namespace EvilAltiris.Lib.CmdLine
{
    public class Options
    {
        public class Arguments
        {
            public bool Help = false;
            public bool Verbose { get; set; } = false;
            public string Smapath { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty;
            public string Data { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Machine { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Outfile { get; set; } = string.Empty;
        }

        // Define required arguments for each command
        private static readonly Dictionary<string, string[]> requiredArgsByCommand = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "DecryptPolicy", new[] { "/key", "/data" } },
            { "SetPublicKey", new[] { "/key", "/machine", "/url" } },
            { "SmaGetTypeGuid", new[] { "/smapath" } },
            { "SmaGetPublicKey", new[] { "/smapath" } },
            { "SmaDecrypt", new[] { "/smapath", "/data" } },
            { "DecryptAcc", new[] { "/data" } },
            { "GetClientPolicies", new[] { "/machine", "/type", "/url" } },
            { "GenerateKeys", Array.Empty<string>() }, 
            { "GetMachineGuid", Array.Empty<string>() },
            { "RestoreAgent", Array.Empty<string>() },
        };

        public static bool ValidateRequiredArgs(string command, Dictionary<string, string> parsedArgs)
        {
            // Check if the command has defined required arguments
            if (requiredArgsByCommand.TryGetValue(command, out var requiredArgs))
            {
                // If no arguments are required, return true
                if (requiredArgs.Length == 0 || (requiredArgs.Length == 1 && string.IsNullOrEmpty(requiredArgs[0])))
                {
                    return true;
                }

                // Validate that each required argument is provided
                foreach (var reqArg in requiredArgs)
                {
                    if (!parsedArgs.ContainsKey(reqArg))
                    {
                        Console.WriteLine($"[!] Missing required argument '{reqArg}' for command '{command}'.");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine($"[!] Unknown command '{command}'.");
                return false;
            }
            return true;
        }


        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var arg in args)
            {
                string[] parts = arg.Split(new[] { ':' }, 2);
                string key = parts[0].ToLower();
                string value = parts.Length > 1 ? parts[1].Trim('"') : "true";  // boolean flags default to true
                parsedArgs[key] = value;
            }

            return parsedArgs;
        }

        public static Arguments ArgumentValues(Dictionary<string, string> parsedArgs)
        {
            var arguments = new Arguments();

            foreach (var kvp in parsedArgs)
            {
                switch (kvp.Key)
                {
                    case "/smapath":
                        arguments.Smapath = kvp.Value;
                        break;
                    case "/key":
                        arguments.Key = kvp.Value;
                        break;
                    case "/data":
                        arguments.Data = kvp.Value;
                        break;
                    case "/url":
                        arguments.Url = kvp.Value;
                        break;
                    case "/machine":
                        arguments.Machine = kvp.Value;
                        break;
                    case "/type":
                        arguments.Type = kvp.Value;
                        break;
                    case "/outfile":
                        arguments.Outfile = kvp.Value;
                        break;
                    case "/verbose":
                        arguments.Verbose = Convert.ToBoolean(kvp.Value);
                        break;
                    case "/help":
                        arguments.Help = true;
                        Info.ShowUsage();
                        return null;
                }
            }
            return arguments;
        }
    }
}
