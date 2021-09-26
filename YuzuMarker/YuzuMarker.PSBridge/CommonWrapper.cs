using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.PSBridge
{
    public static class CommonWrapper
    {
        public static bool ExistLayerSet(string layerSetName)
        {
            return Properties.Settings.PhotoshopBridgeType switch
            {
                Properties.PSBridgeType.COM => COM.Invoker.ExistLayerSet(layerSetName),
                Properties.PSBridgeType.Extension => Extension.Invoker.ExistLayerSet(layerSetName),
                _ => false,
            };
        }

        public static bool ExistArtLayer(string layerSetName, string artLayerName)
        {
            return Properties.Settings.PhotoshopBridgeType switch
            {
                Properties.PSBridgeType.COM => COM.Invoker.ExistArtLayer(layerSetName, artLayerName),
                Properties.PSBridgeType.Extension => Extension.Invoker.ExistArtLayer(layerSetName, artLayerName),
                _ => false,
            };
        }

        public static void OpenFile(string path)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.OpenFile(path);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.OpenFile(path);
                    break;
            }
        }

        public static void CreateFile(string path)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.CreateFile(path);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.CreateFile(path);
                    break;
            }
        }

        public static void AddLayerSet(string layerSetName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.AddLayerSet(layerSetName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.AddLayerSet(layerSetName);
                    break;
            }
        }

        public static void AddArtLayer(string layerSetName, string artLayerName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.AddArtLayer(layerSetName, artLayerName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.AddArtLayer(layerSetName, artLayerName);
                    break;
            }
        }

        public static void RemoveLayerSet(string layerSetName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.RemoveLayerSet(layerSetName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.RemoveLayerSet(layerSetName);
                    break;
            }
        }

        public static void RemoveArtLayer(string layerSetName, string artLayerName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.RemoveArtLayer(layerSetName, artLayerName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.RemoveArtLayer(layerSetName, artLayerName);
                    break;
            }
        }

        public static void AddTextLayer(string layerSetName, string artLayerName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.AddTextLayer(layerSetName, artLayerName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.AddTextLayer(layerSetName, artLayerName);
                    break;
            }
        }

        public static void SetTextLayer(string layerSetName, string artLayerName)
        {
            switch (Properties.Settings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.SetTextLayer(layerSetName, artLayerName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.SetTextLayer(layerSetName, artLayerName);
                    break;
            }
        }
    }
}
