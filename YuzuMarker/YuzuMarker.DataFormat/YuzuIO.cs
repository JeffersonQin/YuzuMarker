using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using YuzuMarker.BasicDataFormat;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public static class YuzuIO
    {
        public static YuzuProject CreateProject(string path, string fileName, string projectName)
        {
            return BasicYuzuIO.CreateProject(path, fileName, projectName) as YuzuProject;
        }

        public static YuzuProject LoadProject(string path)
        {
            return (YuzuProject)BasicYuzuIO.LoadProject(path, (image, timestamp, x, y, text, finished, rootDir) =>
            {
                var notationGroup = new YuzuNotationGroup(image as YuzuImage, timestamp, x, y, text, finished);

                // Cleaning Notation
                var cleaningText = File.ReadAllText(Path.Combine(image.GetImageNotationPath(), "./" + timestamp + "-cleaning.json"));
                var cleaningJson = JObject.Parse(cleaningText);
                switch ((YuzuCleaningNotationType)int.Parse(cleaningJson["type"]?.ToString() ?? "0"))
                {
                    case YuzuCleaningNotationType.Color:
                        var colorCleaningNotation = new YuzuColorCleaningNotation(notationGroup);
                        colorCleaningNotation.CleaningNotationColor = Color.FromArgb(int.Parse(cleaningJson["color"]?.ToString() ?? "0"));
                        notationGroup.CleaningNotation = colorCleaningNotation;
                        break;
                    case YuzuCleaningNotationType.Inpainting:
                        notationGroup.CleaningNotation = new YuzuInpaintingCleaningNotation(notationGroup);
                        break;
                }
                notationGroup.CleaningNotation.DontAutoExport = bool.Parse(cleaningJson["dont_auto_export"]?.ToString() ?? "false");
                
                // Other Notations

                return notationGroup;
            }, 
            (projectPath, fileName, projectName, images) => new YuzuProject(projectPath, fileName, projectName, images), 
            (project, imageName, finished) => new YuzuImage(project as YuzuProject, imageName, finished),
            (project) => {
                project.EnsureTempFolderExist();
                Directory.Delete(Path.Combine(project.Path, "./temp"), true);
            });
        }

        public static void SaveProject(YuzuProject project)
        {
            BasicYuzuIO.SaveProject(project, (basicGroup, rootDir) =>
            {
                var notationGroup = basicGroup as YuzuNotationGroup;
                
                // Cleaning Notation
                var tempCleaningMaskPath = Path.Combine(notationGroup.ParentImage.GetImageTempPath(), "./" + notationGroup.Timestamp + "-cleaning-mask.png");
                if (File.Exists(tempCleaningMaskPath))
                {
                    var cleaningMaskTargetPath = Path.Combine(notationGroup.ParentImage.GetImageNotationPath(), "./" + notationGroup.Timestamp + "-cleaning-mask.png");
                    File.Copy(tempCleaningMaskPath, cleaningMaskTargetPath, true);
                }
                var cleaningJson = new JObject();
                cleaningJson["type"] = (int)notationGroup.CleaningNotation.CleaningNotationType;
                cleaningJson["dont_auto_export"] = notationGroup.CleaningNotation.DontAutoExport;
                
                switch (notationGroup.CleaningNotation.CleaningNotationType)
                {
                    case YuzuCleaningNotationType.Color:
                        cleaningJson["color"] = ((YuzuColorCleaningNotation)notationGroup.CleaningNotation).CleaningNotationColor.ToArgb();
                        break;
                    case YuzuCleaningNotationType.Inpainting:
                        var tempInpaintingFilePath = Path.Combine(notationGroup.ParentImage.GetImageTempPath(), "./" + notationGroup.Timestamp + "-inpainting.png");
                        if (File.Exists(tempInpaintingFilePath))
                        {
                            var inpaintingFileTargetPath = Path.Combine(notationGroup.ParentImage.GetImageNotationPath(), "./" + notationGroup.Timestamp + "-inpainting.png");
                            File.Copy(tempInpaintingFilePath, inpaintingFileTargetPath, true);
                        }
                        break;
                }
                
                File.WriteAllText(Path.Combine(notationGroup.ParentImage.GetImageNotationPath(), "./" + notationGroup.Timestamp + "-cleaning.json"), cleaningJson.ToString());

                // Other Notations
            });
        }
    }
}
