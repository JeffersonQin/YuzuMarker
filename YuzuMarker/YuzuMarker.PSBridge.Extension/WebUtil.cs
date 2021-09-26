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

        public static JObject GET(string requestUri, Dictionary<string, string> param)
        {
            webClient.QueryString.Clear();
            foreach (KeyValuePair<string, string> kv in param)
            {
                webClient.QueryString.Add(kv.Key, kv.Value);
            }
            
            string response;
            try
            {
                response = webClient.DownloadString("http://localhost:" +
                    Properties.Settings.PhotoshopExtensionHTTPServerPort + "/" + requestUri);
            }
            catch (Exception e)
            {
                throw new Exception("YuzuMarker.PSBridge.Extension.WebUtil: communication with photoshop failed, check whether the extension was enabled. \nMessage: " + e.Message);
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
                throw new Exception("YuzuMarker.PSBridge.Extension.WebUtil: JSON Parsing failed. \nMessage: " + e.Message);
            }

            if (error == null || status == null)
            {
                throw new Exception("YuzuMarker.PSBridge.Extension.WebUtil: status and error received null.");
            }

            if (status != "success")
            {
                throw new Exception("YuzuMarker.PSBridge.Extension.WebUtil: operation failed, error: " + error);
            }

            return data;
        }
    }
}
