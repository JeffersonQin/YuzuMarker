using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public static class BasicYuzuIO 
    {
        public static BasicYuzuProject CreateProject(string path, string fileName, string projectName)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("YuzuProjectIO Error: Create path does not exist. Path: " + path);
            }
            var projectFolderPath = Path.Combine(path, fileName);
            if (Directory.Exists(projectFolderPath) || File.Exists(projectFolderPath))
            {
                throw new Exception("YuzuProjectIO Error: file / directory already exist. Path: " + projectFolderPath);
            }
            Directory.CreateDirectory(projectFolderPath);

            var project = new BasicYuzuProject(projectFolderPath, fileName, projectName);
            SaveProject(project);

            return project;
        }

        public delegate BasicYuzuProject ProjectInitializer(string path, string fileName, string projectName, ObservableCollection<BasicYuzuImage> images);

        public delegate BasicYuzuImage ImageInitializer(BasicYuzuProject project, string imageName, bool finished);

        public delegate BasicYuzuNotationGroup LoadNotationGroupHandler(BasicYuzuImage image, long timestamp, int x, int y, string text, bool finished, string rootDir);

        public static BasicYuzuProject LoadProject(string path, 
            LoadNotationGroupHandler loadNotationGroupHandler = null,
            ProjectInitializer projectInitializer = null, 
            ImageInitializer imageInitializer = null)
        {
            var projectFileName = Path.GetFileNameWithoutExtension(path);
            var yuzuProjectXMLDoc = XDocument.Load(path);
            
            var xProject = yuzuProjectXMLDoc.Element("YuzuProject");
            var projectName = xProject.Element("Name").Value;

            var notationFolderPath = Path.Combine(Path.GetDirectoryName(path), "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            
            var xImages = xProject.Element("Images");
            var yuzuImages = new ObservableCollection<BasicYuzuImage>();

            var yuzuProject = projectInitializer == null ?
                new BasicYuzuProject(Path.GetDirectoryName(path), projectFileName, projectName, yuzuImages) : 
                projectInitializer.Invoke(Path.GetDirectoryName(path), projectFileName, projectName, yuzuImages);

            foreach (var xImage in xImages.Elements("Image"))
            {
                var imageName = xImage.Value;
                var imageFinished = (bool)xImage.Attribute("IsFinished");

                var yuzuImage = imageInitializer == null ?
                    new BasicYuzuImage(yuzuProject, imageName, imageFinished) : 
                    imageInitializer.Invoke(yuzuProject, imageName, imageFinished);
                
                yuzuProject.Images.Add(yuzuImage);

                var notationFolderPathForImage = Path.Combine(notationFolderPath, "./" + imageName + "/");
                IOUtils.EnsureDirectoryExist(notationFolderPathForImage);

                var indexFileContent = File.ReadAllText(Path.Combine(notationFolderPathForImage, "./index.json"));
                var notationIndexJArray = JArray.Parse(indexFileContent);
                foreach (var jToken in notationIndexJArray)
                {
                    var notationIndexJObject = (JObject) jToken;
                    var timestamp = long.Parse(notationIndexJObject["timestamp"].ToString());
                    var notationFinished = (bool)notationIndexJObject["finished"];

                    var markNotationJObject = JObject.Parse(File.ReadAllText(Path.Combine(notationFolderPathForImage, "./" + timestamp + "-mark.json")));

                    var notationGroup = loadNotationGroupHandler == null ? 
                        new BasicYuzuNotationGroup(yuzuImage, timestamp, (int)markNotationJObject["x"], (int)markNotationJObject["y"], (string)markNotationJObject["text"], notationFinished) : 
                        loadNotationGroupHandler.Invoke(yuzuImage, timestamp, (int)markNotationJObject["x"], (int)markNotationJObject["y"], (string)markNotationJObject["text"], notationFinished, notationFolderPathForImage);
                    yuzuImage.NotationGroups.Add(notationGroup);
                }
            }

            return yuzuProject;
        }

        public delegate void SaveNotationGroupHandler(BasicYuzuNotationGroup notationGroup, string rootDir);

        public static void SaveProject(BasicYuzuProject project, SaveNotationGroupHandler saveNotationGroupHandler = null) 
        {
            var yuzuProjectXmlDoc = new XDocument {Declaration = new XDeclaration("1.0", "utf-8", "no")};

            var xProject = new XElement("YuzuProject");
            var xProjectName = new XElement("Name", project.ProjectName);
            xProject.Add(xProjectName);
            
            var xImages = new XElement("Images");

            var notationFolderPath = Path.Combine(project.Path, "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            foreach (var yuzuImage in project.Images)
            {
                var xImage = new XElement("Image", yuzuImage.ImageName);
                xImage.SetAttributeValue("IsFinished", yuzuImage.IsFinished);
                xImages.Add(xImage);

                var notationFolderPathForImage = Path.Combine(notationFolderPath, "./" + yuzuImage.ImageName + "/");
                IOUtils.EnsureDirectoryExist(notationFolderPathForImage);

                var notationIndexJArray = new JArray();

                foreach (var notationGroup in yuzuImage.NotationGroups)
                {
                    var timestamp = notationGroup.Timestamp;
                    var notationIndexJObject = new JObject
                    {
                        { "timestamp", timestamp + "" },
                        { "finished", notationGroup.IsFinished }
                    };
                    notationIndexJArray.Add(notationIndexJObject);

                    var markNotationJObject = new JObject
                    {
                        { "x", notationGroup.X },
                        { "y", notationGroup.Y },
                        { "text", notationGroup.Text }
                    };
                    File.WriteAllText(Path.Combine(notationFolderPathForImage, timestamp + "-mark.json"), markNotationJObject.ToString(), Encoding.UTF8);

                    saveNotationGroupHandler?.Invoke(notationGroup, notationFolderPathForImage);
                }
                File.WriteAllText(Path.Combine(notationFolderPathForImage, "./index.json"), notationIndexJArray.ToString(), Encoding.UTF8);
            }

            xProject.Add(xImages);
            yuzuProjectXmlDoc.Add(xProject);

            yuzuProjectXmlDoc.Save(Path.Combine(project.Path, project.FileName + ".yuzu"));
        }
    }
}
