using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YuzuMarker.PSBridge
{
    public static class CommonWrapper
    {
        #region [Deprecated] Old API Section
        public static bool ExistLayerSet(string layerSetName)
        {
            return Properties.CoreSettings.PhotoshopBridgeType switch
            {
                Properties.PSBridgeType.COM => COM.Invoker.ExistLayerSet(layerSetName),
                Properties.PSBridgeType.Extension => Extension.Invoker.ExistLayerSet(layerSetName),
                _ => false,
            };
        }

        public static bool ExistArtLayer(string layerSetName, string artLayerName)
        {
            return Properties.CoreSettings.PhotoshopBridgeType switch
            {
                Properties.PSBridgeType.COM => COM.Invoker.ExistArtLayer(layerSetName, artLayerName),
                Properties.PSBridgeType.Extension => Extension.Invoker.ExistArtLayer(layerSetName, artLayerName),
                _ => false,
            };
        }

        public static void CreateFile(string path)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
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
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.SetTextLayer(layerSetName, artLayerName);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.SetTextLayer(layerSetName, artLayerName);
                    break;
            }
        }
        #endregion

        #region New API Section
        public static void OpenFile(string path)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    COM.Invoker.OpenFile(path);
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.OpenFile(path);
                    break;
            }
        }

        public static void SaveFileAs(string path)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.SaveFileAs(path);
                    break;
            }
        }
        
        public static void ExistArtLayerURI(string artLayerPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.ExistArtLayerURI(artLayerPath);
                    break;
            }
        }
        
        public static void ExistLayerSetURI(string layerSetPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.ExistLayerSetURI(layerSetPath);
                    break;
            }
        }
        
        public static void CreateArtLayerIfNotExistByURI(string artLayerPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.CreateArtLayerIfNotExistByURI(artLayerPath);
                    break;
            }
        }
        
        public static void CreateLayerSetIfNotExistByURI(string layerSetPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.CreateLayerSetIfNotExistByURI(layerSetPath);
                    break;
            }
        }
        #endregion
        
        #region Wrapping Section
        public static void GeneratePSD(string imagePath, string psdPath)
        {
            OpenFile(imagePath);
            SaveFileAs(psdPath);
        }

        public static void OpenAndGeneratePSDIfNotExist(string imagePath, string psdPath)
        {
            if (!File.Exists(psdPath)) GeneratePSD(imagePath, psdPath);
            else OpenFile(psdPath);
        }

        public static void InitPSDFileStructure(string psdPath)
        {
            OpenFile(psdPath);
            /* TODO:
             * Rename background to Background
             * Add LayerSet 自定义涂白, 自动涂白, 嵌字
             */
        }
        #endregion
    }
}
