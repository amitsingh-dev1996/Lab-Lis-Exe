using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class WebAPICommon
    {
        public static object PostApi(string baseUrl, string urlControllerActionName, object input, object outputRef, bool isGetRequest = false, int TimeoutMilliSeconds = 0)
        {
            //used NewtonSoft here to serialize anonymous objects, as it is not possible to do with .net classes
            string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            string url = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + urlControllerActionName;
            WebClientWithTimeout client = new WebClientWithTimeout();
            client.TimeoutMilliSeconds = TimeoutMilliSeconds;
            client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";

            DateTime dtmStart = DateTime.Now;
            string json = "";
            try
            {
                json = isGetRequest ? client.DownloadString(url) : client.UploadString(url, strRequest);
            }
            catch (Exception ex)
            {
                string message = "";
                string hostName = new Uri(baseUrl).Host;
                if (!hostName.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                {
                    string serverName = hostName.Substring(0, hostName.IndexOf("."));
                    if (ex.Message.StartsWith("The remote name could not be resolved"))
                    {
                        message = "Unable to connect " + serverName.ToUpper() + " server. Kindly check internet connetion, or contact Infra Team.";
                    }
                    else if (ex.Message.Contains("Could not establish trust relationship for the SSL/TLS secure channel"))
                    {
                        message = "Kindly get in touch with Infra Team to check SSL certificate issue on LIS PC. (Issue observed while connecting " + serverName + " server)";
                    }
                }

                if (message == "")
                {
                    throw;
                }
                else
                {
                    throw new Exception(message, ex);
                }
            }

            object obj = null;
            try
            {
                obj = InterfaceHelper.DeserializeFromJson(json, outputRef);
            }
            catch (Exception ex)
            {
                throw new Exception("Kindly check internet connectivity. Invalid response received.", ex);
            }
            return obj;
        }

        public static void DownloadFile(string baseUrl, string urlControllerActionName, string targetFileFullname)
        {
            string url = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + urlControllerActionName;
            using (WebClientWithTimeout client = new WebClientWithTimeout())
            {
                client.DownloadFile(url, targetFileFullname);
            }
        }
    }

    public class WebClientWithTimeout : System.Net.WebClient
    {
        public int TimeoutMilliSeconds { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            int timeout = TimeoutMilliSeconds == 0 ? (5 * 60 * 1000) : TimeoutMilliSeconds;
            lWebRequest.Timeout = timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = timeout;
            return lWebRequest;
        }
    }
}
