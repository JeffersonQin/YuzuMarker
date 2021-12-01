using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace YuzuMarker.PSBridge.Extension
{
    public static class Invoker
    {
        #region [Deprecated] Old API Section
        public static bool ExistLayerSet(string layerSetName)
        {
            JObject ret = WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "existLayerSet", new Dictionary<string, string>
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
            JObject ret = WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "existArtLayer", new Dictionary<string, string>
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

        public static void CreateFile(string path)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "createFile", new Dictionary<string, string>
            {
                { "path", path }
            });
        }

        public static void AddLayerSet(string layerSetName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "addLayerSet", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName }
            });
        }

        public static void AddArtLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "addArtLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void RemoveLayerSet(string layerSetName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "removeLayerSet", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName }
            });
        }

        public static void RemoveArtLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "removeArtLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void AddTextLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "addTextLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }

        public static void SetTextLayer(string layerSetName, string artLayerName)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "setTextLayer", new Dictionary<string, string>
            {
                { "layerSetName", layerSetName },
                { "artLayerName", artLayerName }
            });
        }
        #endregion

        #region New Native Common API Section
        public static void OpenFile(string path)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "openFile", new Dictionary<string, string>
            {
                { "path", path }
            });
        }

        public static void SaveFileAs(string path)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "saveFileAs", new Dictionary<string, string>
            {
                { "path", path }
            });
        }
        
        public static bool ExistArtLayerURI(string artLayerPath)
        {
            JObject ret = WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "existArtLayerURI", new Dictionary<string, string>
            {
                { "artLayerPath", artLayerPath }
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
        
        public static bool ExistLayerSetURI(string layerSetPath)
        {
            JObject ret = WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "existLayerSetURI", new Dictionary<string, string>
            {
                { "layerSetPath", layerSetPath }
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

        public static void CreateArtLayerIfNotExistByURI(string artLayerPath)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "createArtLayerIfNotExistByURI", new Dictionary<string, string>
            {
                { "artLayerPath", artLayerPath }
            });
        }
        
        public static void CreateLayerSetIfNotExistByURI(string layerSetPath)
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "createLayerSetIfNotExistByURI", new Dictionary<string, string>
            {
                { "layerSetPath", layerSetPath }
            });
        }

        public static void ApplyMask()
        {
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "applyMask", new Dictionary<string, string>());
        }

        public static void PerformSelection(List<PointF> points)
        {
            JArray pointsJSON = new JArray();
            foreach (PointF point in points)
            {
                pointsJSON.Add(new JObject()
                {
                    { "x", point.X },
                    { "y", point.Y }
                });
            }
            
            WebUtil.GET(Properties.CoreSettings.PhotoshopExtensionHTTPServerPort, 
                "performSelection", new Dictionary<string, string>
            {
                { "points", pointsJSON.ToString() }
            });
        }
        #endregion
    }
}
