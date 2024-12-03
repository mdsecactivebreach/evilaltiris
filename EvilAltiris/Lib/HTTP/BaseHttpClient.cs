using System;
using System.IO;
using System.Net;
using System.Text;

namespace EvilAltiris.Lib.HTTP
{
    public abstract class BaseHttpClient
    {
        protected readonly string BaseUrl;

        protected BaseHttpClient(string baseUrl)
        {
            BaseUrl = baseUrl;
            ConfigureSecurity();
        }

        // Generic method to send HTTP request
        protected string SendRequest(string endpoint, string method, string contentType, string content, string machineGuid = null)
        {
            string url = $"{BaseUrl}{endpoint}";
            byte[] byteArray = Encoding.UTF8.GetBytes(content);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;
            request.Headers.Add("Cache-Control", "max-age=0");
            request.UseDefaultCredentials = true;  // Use the default credentials of the logged-in user

            if (!string.IsNullOrEmpty(machineGuid))
            {
                request.Headers.Add("X-SMA-ID", machineGuid.Replace("{", "").Replace("}", ""));
            }

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return ReadResponse(response);
                    }
                    else
                    {
                        Console.WriteLine($"[!] Response error with status code: {response.StatusCode}");
                        return ReadResponse(response);
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("[!] HTTP error: " + ex.Message);
                if (ex.Response is HttpWebResponse errorResponse)
                {
                    Console.WriteLine($"[!] HTTP error code: {errorResponse.StatusCode}");
                    return ReadResponse(errorResponse);
                }
                return null;
            }
        }

        // Modified ReadResponse to handle binary or text content
        private string ReadResponse(HttpWebResponse response)
        {
            // Check the content type to determine if the response is binary
            bool isBinary = response.ContentType != null && (response.ContentType.StartsWith("binary/octet-stream"));

            using (Stream responseStream = response.GetResponseStream())
            {
                if (isBinary)
                {
                    // Read as binary and return Base64-encoded string
                    using (MemoryStream ms = new MemoryStream())
                    {
                        responseStream.CopyTo(ms);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
                else
                {
                    // Read as text (UTF-8 encoding)
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        // Configure TLS settings
        private void ConfigureSecurity()
        {
            if (BaseUrl.ToLower().StartsWith("https"))
            {
                try
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288
                                                         | (SecurityProtocolType)3072
                                                         | (SecurityProtocolType)768
                                                         | SecurityProtocolType.Tls;
                }
                catch (NotSupportedException)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }
        }
    }
}