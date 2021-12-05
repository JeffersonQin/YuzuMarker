using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using OpenCvSharp;
using YuzuMarker.DataFormat;

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
        
        public static bool ExistArtLayerURI(string artLayerPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    return Extension.Invoker.ExistArtLayerURI(artLayerPath);
                    break;
            }
            throw new ArgumentOutOfRangeException();
        }
        
        public static bool ExistLayerSetURI(string layerSetPath)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    return Extension.Invoker.ExistLayerSetURI(layerSetPath);
                    break;
            }
            throw new ArgumentOutOfRangeException();
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

        public static void ApplyMask()
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.ApplyMask();
                    break;
            }
        }
        
        public static void PerformSelection(List<PointF> points)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.PerformSelection(points);
                    break;
            }
        }
        
        public static void PerformRasterization() 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.PerformRasterization();
                    break;
            }
        }
        
        public static void PerformChannelSelection() 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.PerformChannelSelection();
                    break;
            }
        }
        
        public static void RenameBackgroundTo(string newName) 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.RenameBackgroundTo(newName);
                    break;
            }
        }
        
        public static void DeleteArtLayerByURI(string artLayerPath) 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.DeleteArtLayerByURI(artLayerPath);
                    break;
            }
        }
        
        public static void DeleteLayerSetByURI(string layerSetPath) 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.DeleteLayerSetByURI(layerSetPath);
                    break;
            }
        }
        
        public static void SelectArtLayerByURI(string artLayerPath) 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.SelectArtLayerByURI(artLayerPath);
                    break;
            }
        }
        
        public static void SelectLayerSetByURI(string layerSetPath) 
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.SelectLayerSetByURI(layerSetPath);
                    break;
            }
        }

        public static void ImportImage(string fileName)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.ImportImage(fileName);
                    break;
            }
        }

        public static void DuplicateAndSelectArtLayerByURI(string sourcePath, string targetDir, string targetName)
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.DuplicateAndSelectArtLayerByURI(sourcePath, targetDir, targetName);
                    break;
            }
        }

        public static void PerformRgbChannelSelection()
        {
            switch (Properties.CoreSettings.PhotoshopBridgeType)
            {
                case Properties.PSBridgeType.COM:
                    throw new NotImplementedException();
                    break;
                case Properties.PSBridgeType.Extension:
                    Extension.Invoker.PerformRgbChannelSelection();
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

        public static void OpenAndInitPSDFileStructureIfNotExist(string imagePath, string psdPath)
        {
            if (!File.Exists(psdPath))
                GeneratePSD(imagePath, psdPath);
            else OpenFile(psdPath);
            
            if (!ExistArtLayerURI("Background"))
                RenameBackgroundTo("Background");
            CreateLayerSetIfNotExistByURI("CustomBackground");
            CreateLayerSetIfNotExistByURI("AutoBackground");
            CreateLayerSetIfNotExistByURI("AutoBackground/Color");
            CreateLayerSetIfNotExistByURI("AutoBackground/Impainting");
            CreateLayerSetIfNotExistByURI("AutoTextItems");
            CreateLayerSetIfNotExistByURI("CustomTextItems");
        }

        public static void OpenAndInitPSDFileStructureIfNotExist(this YuzuImage image)
        {
            OpenAndInitPSDFileStructureIfNotExist(image.GetImageFilePath(), image.GetImagePsdPath());
        }

        public static void SelectAutoExportedLayer(this YuzuCleaningNotation notation)
        {
            switch (notation.CleaningNotationType)
            {
                case YuzuCleaningNotationType.Color:
                    if (!ExistArtLayerURI("AutoBackground/Color/ColorExport"))
                        throw new Exception("未发现颜色图层，可能是尚未导出");
                    else SelectArtLayerByURI("AutoBackground/Color/ColorExport");
                    break;
                case YuzuCleaningNotationType.Impainting:
                    if (!ExistArtLayerURI("AutoBackground/Impainting/Impainting-" + 
                        (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1)))
                        throw new Exception("未发现修复图层，可能是尚未导出或者顺序变动");
                    else SelectArtLayerByURI("AutoBackground/Impainting/Impainting-" + 
                        (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1));
                    break;
            }
        }

        public static void DeleteAutoExportedLayer(this YuzuCleaningNotation notation)
        {
            switch (notation.CleaningNotationType)
            {
                case YuzuCleaningNotationType.Color:
                    if (!ExistArtLayerURI("AutoBackground/Color/ColorExport"))
                        throw new Exception("未发现颜色图层，可能是尚未导出");
                    else DeleteArtLayerByURI("AutoBackground/Color/ColorExport");
                    break;
                case YuzuCleaningNotationType.Impainting:
                    if (!ExistArtLayerURI("AutoBackground/Impainting/Impainting-" + 
                        (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1)))
                        throw new Exception("未发现修复图层，可能是尚未导出或者顺序变动");
                    else DeleteArtLayerByURI("AutoBackground/Impainting/Impainting-" + 
                        (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1));
                    break;
            }
        }
        
        public static void SelectCustomLayerSet(this YuzuCleaningNotation notation)
        {
            if (!ExistLayerSetURI("CustomBackground/Background-" + 
                (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1)))
                throw new Exception("未发现自定义图层组，可能是尚未导出或者顺序变动");
            SelectLayerSetByURI("CustomBackground/Background-" + 
                (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1));
        }

        public static void DeleteCustomLayerSet(this YuzuCleaningNotation notation)
        {
            if (!ExistLayerSetURI("CustomBackground/Background-" + 
                (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1)))
                throw new Exception("未发现自定义图层组，可能是尚未导出或者顺序变动");
            DeleteLayerSetByURI("CustomBackground/Background-" + 
                (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1));
        }

        public static void ExportMask(this YuzuCleaningNotation notation)
        {
            var tempFilePath = Path.GetTempFileName() + ".png";
            var writeMat = notation.CleaningMask.GetMat(AccessFlag.READ);
            Cv2.ImWrite(tempFilePath, writeMat);
            writeMat.Dispose();
            var tempFileName = Path.GetFileNameWithoutExtension(tempFilePath);
            SelectLayerSetByURI("CustomTextItems");
            ImportImage(tempFilePath);
            PerformRgbChannelSelection();
            DuplicateAndSelectArtLayerByURI("Background", "CustomBackground/Background-" + 
                (notation.ParentNotationGroup.ParentImage.NotationGroups.IndexOf(notation.ParentNotationGroup) + 1), "Mask");
            ApplyMask();
            DeleteArtLayerByURI(tempFileName);
        }
        #endregion
    }
}
