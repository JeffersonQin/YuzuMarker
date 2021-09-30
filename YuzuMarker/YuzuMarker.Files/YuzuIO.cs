using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace YuzuMarker.Files
{
    public static class YuzuIO
    {
        public static YuzuProject CreateProject(string path, string fileName, string projectName)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("YuzuProjectIO Error: Create path does not exist. Path: " + path);
            }
            string projectFolderPath = Path.Combine(path, fileName);
            if (Directory.Exists(projectFolderPath) || File.Exists(projectFolderPath))
            {
                throw new Exception("YuzuProjectIO Error: file / directory already exist. Path: " + projectFolderPath);
            }
            Directory.CreateDirectory(projectFolderPath);

            YuzuProject project = new YuzuProject(projectFolderPath, fileName, projectName);
            SaveProject(project);

            return project;
        }

        public static YuzuProject LoadProject(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            XDocument doc = XDocument.Load(path);
            
            XElement projectElement = doc.Element("YuzuProject");
            string projectName = projectElement.Element("Name").Value;

            string simpleNotationFolderPath = Path.Combine(Path.GetDirectoryName(path), "./Notations/");
            IOUtils.EnsureDirectoryExist(simpleNotationFolderPath);
            
            XElement imagesElement = projectElement.Element("Images");
            List<YuzuImage> yuzuImages = new List<YuzuImage>();
            YuzuProject yuzuProject = new YuzuProject(Path.GetDirectoryName(path), fileName, projectName, yuzuImages);

            foreach (XElement imageElement in imagesElement.Elements("Image"))
            {
                string imageName = imageElement.Value;
                YuzuImage yuzuImage = new YuzuImage(yuzuProject, imageName);
                yuzuProject.Images.Add(yuzuImage);

                string simpleNotationImagePath = Path.Combine(simpleNotationFolderPath, "./" + imageName + "/");
                IOUtils.EnsureDirectoryExist(simpleNotationImagePath);

                string indexFileContent = File.ReadAllText(Path.Combine(simpleNotationImagePath, "./index.json"));
                JArray notationFiles = JArray.Parse(indexFileContent);
                foreach (string notationTimestamp in notationFiles)
                {
                    long timestamp = long.Parse(notationTimestamp);

                    JObject simpleNotation = JObject.Parse(File.ReadAllText(Path.Combine(simpleNotationImagePath, "./" + timestamp + "-simple.json")));
                    YuzuNotationGroup notationGroup = new YuzuNotationGroup(timestamp, (int)simpleNotation["x"], (int)simpleNotation["y"], (string)simpleNotation["text"]);

                    // Other Notations

                    yuzuImage.NotationGroups.Add(notationGroup);
                }
            }

            return yuzuProject;
        }

        public static void SaveProject(YuzuProject project)
        {
            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "utf-8", "no");
            
            XElement xProject = new XElement("YuzuProject");
            XElement xProjectName = new XElement("Name", project.projectName);
            xProject.Add(xProjectName);
            
            XElement xImages = new XElement("Images");

            string simpleNotationFolderPath = Path.Combine(project.path, "./Notations/");
            IOUtils.EnsureDirectoryExist(simpleNotationFolderPath);
            foreach (YuzuImage yuzuImage in project.Images)
            {
                xImages.Add(new XElement("Image", yuzuImage.ImageName));

                string simpleNotationImagePath = Path.Combine(simpleNotationFolderPath, "./" + yuzuImage.ImageName + "/");
                IOUtils.EnsureDirectoryExist(simpleNotationImagePath);

                JArray timestamps = new JArray();

                foreach (YuzuNotationGroup notationGroup in yuzuImage.NotationGroups)
                {
                    long timestamp = notationGroup.Timestamp;
                    timestamps.Add(timestamp + "");

                    YuzuSimpleNotation simpleNotation = notationGroup.SimpleNotation;
                    JObject simpleNotationObject = new JObject();
                    simpleNotationObject.Add("x", simpleNotation.x);
                    simpleNotationObject.Add("y", simpleNotation.y);
                    simpleNotationObject.Add("text", simpleNotation.text);
                    File.WriteAllText(Path.Combine(simpleNotationImagePath, timestamp + "-simple.json"), simpleNotationObject.ToString(), Encoding.UTF8);

                    // Other Notations
                }
                File.WriteAllText(Path.Combine(simpleNotationImagePath, "./index.json"), timestamps.ToString(), Encoding.UTF8);
            }

            xProject.Add(xImages);
            doc.Add(xProject);

            doc.Save(Path.Combine(project.path, project.fileName + ".yuzu"));
        }
    }
}
