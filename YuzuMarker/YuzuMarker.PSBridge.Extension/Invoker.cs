using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.PSBridge.Extension
{
    public static class Invoker
    {
        public static bool ExistLayerSet(string layerSetName)
        {
            JObject ret = WebUtil.GET("existLayerSet", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName }
            });
            try
            {
                return (bool)ret["exist"];
            }
            catch
            {
                throw new Exception("YuzuMarker.PSBridge.Extension.Invoker: failed to read data");
            }
        }

        public static bool ExistArtLayer(string layerSetName, string artLayerName)
        {
            JObject ret = WebUtil.GET("existArtLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
            try
            {
                return (bool)ret["exist"];
            }
            catch
            {
                throw new Exception("YuzuMarker.PSBridge.Extension.Invoker: failed to read data");
            }
        }

        public static void OpenFile(string path)
        {
            WebUtil.GET("openFile", new Dictionary<string, string>
            {
                { "path", path }
            });
        }

        public static void CreateFile(string path)
        {
            WebUtil.GET("createFile", new Dictionary<string, string>
            {
                { "path", path }
            });
        }

        public static void AddLayerSet(string layerSetName)
        {
            WebUtil.GET("addLayerSet", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName }
            });
        }

        public static void AddArtLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET("addArtLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void RemoveLayerSet(string layerSetName)
        {
            WebUtil.GET("removeLayerSet", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName }
            });
        }

        public static void RemoveArtLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET("removeArtLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void AddTextLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET("addTextLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void SetTextLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET("setTextLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }
    }
}
