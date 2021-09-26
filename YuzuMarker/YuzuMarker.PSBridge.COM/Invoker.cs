using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.PSBridge.COM
{
    public static class Invoker
    {
        #region real private section
        private static Photoshop.LayerSet GetLayerSet(string layerSetName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSets layerSets = app.ActiveDocument.LayerSets;
            foreach (Photoshop.LayerSet layerSet in layerSets)
            {
                if (layerSet.Name == layerSetName)
                {
                    return layerSet;
                }
            }
            throw new Exception("PSBridge COM Exception: LayerSet Not Found");
        }

        private static Photoshop.ArtLayer GetArtLayer(string layerSetName, string artLayerName)
        {
            Photoshop.LayerSet layerSet = GetLayerSet(layerSetName);
            foreach (Photoshop.ArtLayer artLayer in layerSet.ArtLayers)
            {
                if (artLayer.Name == artLayerName)
                {
                    return artLayer;
                }
            }
            throw new Exception("PSBridge COM Exception: ArtLayer Not Found");
        }
        #endregion

        #region judging exist
        public static bool ExistLayerSet(string layerSetName)
        {
            try
            {
                GetLayerSet(layerSetName);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public static bool ExistArtLayer(string layerSetName, string artLayerName)
        {
            try
            {
                GetArtLayer(layerSetName, artLayerName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region functions
        private static Photoshop.Document _OpenFile(string path)
        {
            Photoshop.Application app = new Photoshop.Application();
            return app.Open(path);
        }

        private static Photoshop.Document _CreateFile(string path)
        {
            Photoshop.Application app = new Photoshop.Application();
            // TODO
            return null;
        }

        private static Photoshop.LayerSet _AddLayerSet(string layerSetName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSet layerSet = app.ActiveDocument.LayerSets.Add();
            layerSet.Name = layerSetName;
            return layerSet;
        }

        private static Photoshop.ArtLayer _AddArtLayer(string layerSetName, string artLayerName)
        {
            Photoshop.ArtLayer artLayer = GetLayerSet(layerSetName).ArtLayers.Add();
            artLayer.Name = artLayerName;
            return artLayer;
        }

        private static void _RemoveLayerSet(string layerSetName)
        {
            GetLayerSet(layerSetName).Delete();
        }

        private static void _RemoveArtLayer(string layerSetName, string artLayerName)
        {
            GetArtLayer(layerSetName, artLayerName).Delete();
        }

        private static Photoshop.ArtLayer _AddTextLayer(string layerSetName, string artLayerName)
        {
            Photoshop.ArtLayer textLayer = _AddArtLayer(layerSetName, artLayerName);
            textLayer.Kind = Photoshop.PsLayerKind.psTextLayer;
            return textLayer;
        }

        private static void _SetTextLayer(string layerSetName, string artLayerName)
        {
            GetArtLayer(layerSetName, artLayerName).Kind = Photoshop.PsLayerKind.psTextLayer;
        }
        #endregion

        #region wrapping
        public static void OpenFile(string path) { _OpenFile(path); }
        public static void CreateFile(string path) { _CreateFile(path); }
        public static void AddLayerSet(string layerSetName) { _AddLayerSet(layerSetName); }
        public static void AddArtLayer(string layerSetName, string artLayerName) { _AddArtLayer(layerSetName, artLayerName); }
        public static void RemoveLayerSet(string layerSetName) { _RemoveLayerSet(layerSetName); }
        public static void RemoveArtLayer(string layerSetName, string artLayerName) { _RemoveArtLayer(layerSetName, artLayerName); }
        public static void AddTextLayer(string layerSetName, string artLayerName) { _AddTextLayer(layerSetName, artLayerName); }
        public static void SetTextLayer(string layerSetName, string artLayerName) { _SetTextLayer(layerSetName, artLayerName); }
        #endregion
    }
}
