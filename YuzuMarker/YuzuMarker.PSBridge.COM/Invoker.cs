using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.PSBridge.COM
{
    public static class Invoker
    {
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

        public static Photoshop.Document OpenFile(string path)
        {
            Photoshop.Application app = new Photoshop.Application();
            return app.Open(path);
        }

        public static Photoshop.Document CreateFile(string path)
        {
            Photoshop.Application app = new Photoshop.Application();
            // TODO
            return null;
        }

        public static Photoshop.LayerSet AddLayerSet(string layerSetName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSet layerSet = app.ActiveDocument.LayerSets.Add();
            layerSet.Name = layerSetName;
            return layerSet;
        }

        public static Photoshop.ArtLayer AddArtLayer(string layerSetName, string artLayerName)
        {
            Photoshop.ArtLayer artLayer = GetLayerSet(layerSetName).ArtLayers.Add();
            artLayer.Name = artLayerName;
            return artLayer;
        }

        public static void RemoveLayerSet(string layerSetName)
        {
            GetLayerSet(layerSetName).Delete();
        }

        public static void RemoveArtLayer(string layerSetName, string artLayerName)
        {
            GetArtLayer(layerSetName, artLayerName).Delete();
        }

        public static Photoshop.ArtLayer AddTextLayer(string layerSetName, string textLayerName)
        {
            Photoshop.ArtLayer textLayer = AddArtLayer(layerSetName, textLayerName);
            textLayer.Kind = Photoshop.PsLayerKind.psTextLayer;
            return textLayer;
        }

        public static void SetTextLayer(string layerSetName, string textLayerName)
        {
            GetArtLayer(layerSetName, textLayerName).Kind = Photoshop.PsLayerKind.psTextLayer;
        }
    }
}
