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
        #endregion

        #region New Native Common API Section
        public static void OpenFile(string path)
        {
            WebUtil.GET("openFile", new Dictionary<string, string>
            {
                { "path", path }
            });
        }

        public static void SaveFileAs(string path)
        {
            WebUtil.GET("saveFileAs", new Dictionary<string, string>
            {
                { "path", path }
            });
        }
        
        public static bool ExistArtLayerURI(string artLayerPath)
        {
            JObject ret = WebUtil.GET("existArtLayerURI", new Dictionary<string, string>
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
            JObject ret = WebUtil.GET("existLayerSetURI", new Dictionary<string, string>
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
            WebUtil.GET("createArtLayerIfNotExistByURI", new Dictionary<string, string>
            {
                { "artLayerPath", artLayerPath }
            });
        }
        
        public static void CreateLayerSetIfNotExistByURI(string layerSetPath)
        {
            WebUtil.GET("createLayerSetIfNotExistByURI", new Dictionary<string, string>
            {
                { "layerSetPath", layerSetPath }
            });
        }

        public static void ApplyMask()
        {
            WebUtil.GET("applyMask", new Dictionary<string, string>());
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
            
            WebUtil.GET("performSelection", new Dictionary<string, string>
            {
                { "points", pointsJSON.ToString() }
            });
        }
        #endregion
    }
}
