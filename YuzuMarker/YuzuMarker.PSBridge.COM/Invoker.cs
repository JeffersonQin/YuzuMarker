using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.PSBridge.COM
{
    public static class Invoker
    {
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

        public static Photoshop.LayerSet AddNewLayerSet(string layerSetName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSet layerSet = app.ActiveDocument.LayerSets.Add();
            layerSet.Name = layerSetName;
            return layerSet;
        }

        public static Photoshop.ArtLayer AddNewArtLayer(string layerSetName, string artLayerName)
        {
            Photoshop.Application app = new Photoshop.Application();
            foreach (Photoshop.LayerSet layerSet in app.ActiveDocument.LayerSets)
            {
                if (layerSet.Name == layerSetName)
                {
                    Photoshop.ArtLayer artLayer = layerSet.ArtLayers.Add();
                    artLayer.Name = artLayerName;
                    return artLayer;
                }
            }
            throw new Exception("PSBridge COM Exception: LayerSet Not Found");
        }

        public static void RemoveLayerSet(string layerSetName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSets layerSets = app.ActiveDocument.LayerSets;
            foreach (Photoshop.LayerSet layerSet in layerSets)
            {
                if (layerSet.Name == layerSetName)
                {
                    layerSets.Remove(layerSet);
                    return;
                }
            }
        }

        public static void RemoveArtLayer(string layerSetName, string artLayerName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSets layerSets = app.ActiveDocument.LayerSets;
            foreach (Photoshop.LayerSet layerSet in layerSets)
            {
                if (layerSet.Name == layerSetName)
                {
                    Photoshop.ArtLayer artLayer = layerSet.ArtLayers.Add();
                    artLayer.Name = artLayerName;
                    return;
                }
            }
        }

        public static Photoshop.ArtLayer AddTextLayer(string layerSetName, string textLayerName)
        {
            Photoshop.ArtLayer textLayer = AddNewArtLayer(layerSetName, textLayerName);
            textLayer.Kind = Photoshop.PsLayerKind.psTextLayer;
            return textLayer;
        }

        public static Photoshop.ArtLayer SetTextLayer(string layerSetName, string textLayerName)
        {
            Photoshop.Application app = new Photoshop.Application();
            Photoshop.LayerSets layerSets = app.ActiveDocument.LayerSets;
            foreach (Photoshop.LayerSet layerSet in layerSets)
            {
                if (layerSet.Name == layerSetName)
                {
                    Photoshop.ArtLayers artLayers = layerSet.ArtLayers;
                    foreach (Photoshop.ArtLayer artLayer in artLayers)
                    {
                        if (artLayer.Name == textLayerName)
                        {
                            artLayer.Kind = Photoshop.PsLayerKind.psTextLayer;
                            return artLayer;
                        }
                    }
                    throw new Exception("PSBridge COM Exception: ArtLayer Not Found");
                }
            }
            throw new Exception("PSBridge COM Exception: LayerSet Not Found");
        }
    }
}
