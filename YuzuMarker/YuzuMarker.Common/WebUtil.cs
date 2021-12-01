using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YuzuMarker.PSBridge.Extension
{
    public static class WebUtil
    {
        private static WebClient webClient = new WebClient();

        public static JObject GET(int port, string requestUri, Dictionary<string, string> param)
        {
            webClient.QueryString.Clear();
            foreach (KeyValuePair<string, string> kv in param)
            {
                webClient.QueryString.Add(kv.Key, kv.Value);
            }
            
            string response;
            try
            {
                response = webClient.DownloadString("http://localhost:" + port + "/" + requestUri);
            }
            catch (Exception e)
            {
                throw new Exception("YuzuMarker.Common.WebUtil: connect to server failed. \nMessage: " + e.Message);
            }
            
            JObject obj, data;
            string status, error;
            try
            {
                obj = JObject.Parse(response);
                status = (string)obj["status"];
                error = (string)obj["error"];
                data = (JObject)obj["data"];
            }
            catch (Exception e)
            {
                throw new Exception("YuzuMarker.Common.WebUtil: JSON Parsing failed. \nMessage: " + e.Message);
            }

            if (error == null || status == null)
            {
                throw new Exception("YuzuMarker.Common.WebUtil: status and error received null.");
            }

            if (status != "success")
            {
                throw new Exception("YuzuMarker.Common.WebUtil: operation failed, error: " + error);
            }

            return data;
        }
    }
}
