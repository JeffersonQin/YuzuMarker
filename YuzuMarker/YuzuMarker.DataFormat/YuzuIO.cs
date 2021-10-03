using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace YuzuMarker.DataFormat
{
    public static class YuzuIO
    {
        public static YuzuProject<LP, LI> CreateProject<LP, LI>(string path, string fileName, string projectName)
            where LP : IList<YuzuImage<LI>>, new() where LI : IList<YuzuNotationGroup>, new()
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

            YuzuProject<LP, LI> project = new YuzuProject<LP, LI>(projectFolderPath, fileName, projectName);
            SaveProject(project);

            return project;
        }

        public static YuzuProject<LP, LI> LoadProject<LP, LI>(string path)
            where LP : IList<YuzuImage<LI>>, new() where LI : IList<YuzuNotationGroup>, new()
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            XDocument doc = XDocument.Load(path);
            
            XElement projectElement = doc.Element("YuzuProject");
            string projectName = projectElement.Element("Name").Value;

            string notationFolderPath = Path.Combine(Path.GetDirectoryName(path), "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            
            XElement imagesElement = projectElement.Element("Images");
            LP yuzuImages = new LP();
            YuzuProject<LP, LI> yuzuProject = new YuzuProject<LP, LI>(Path.GetDirectoryName(path), fileName, projectName, yuzuImages);

            foreach (XElement imageElement in imagesElement.Elements("Image"))
            {
                string imageName = imageElement.Value;
                YuzuImage<LI> yuzuImage = new YuzuImage<LI>(yuzuProject.path, imageName);
                yuzuProject.Images.Add(yuzuImage);

                string notationImagePath = Path.Combine(notationFolderPath, "./" + imageName + "/");
                IOUtils.EnsureDirectoryExist(notationImagePath);

                string indexFileContent = File.ReadAllText(Path.Combine(notationImagePath, "./index.json"));
                JArray notationFiles = JArray.Parse(indexFileContent);
                foreach (string notationTimestamp in notationFiles)
                {
                    long timestamp = long.Parse(notationTimestamp);

                    JObject markNotation = JObject.Parse(File.ReadAllText(Path.Combine(notationImagePath, "./" + timestamp + "-mark.json")));
                    YuzuNotationGroup notationGroup = new YuzuNotationGroup(timestamp, (int)markNotation["x"], (int)markNotation["y"], (string)markNotation["text"]);

                    // Other Notations

                    yuzuImage.NotationGroups.Add(notationGroup);
                }
            }

            return yuzuProject;
        }

        public static void SaveProject<LP, LI>(YuzuProject<LP, LI> project) 
            where LP : IList<YuzuImage<LI>>, new() where LI : IList<YuzuNotationGroup>, new()
        {
            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "utf-8", "no");
            
            XElement xProject = new XElement("YuzuProject");
            XElement xProjectName = new XElement("Name", project.projectName);
            xProject.Add(xProjectName);
            
            XElement xImages = new XElement("Images");

            string notationFolderPath = Path.Combine(project.path, "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            foreach (YuzuImage<LI> yuzuImage in project.Images)
            {
                xImages.Add(new XElement("Image", yuzuImage.ImageName));

                string notationImagePath = Path.Combine(notationFolderPath, "./" + yuzuImage.ImageName + "/");
                IOUtils.EnsureDirectoryExist(notationImagePath);

                JArray timestamps = new JArray();

                foreach (YuzuNotationGroup notationGroup in yuzuImage.NotationGroups)
                {
                    long timestamp = notationGroup.Timestamp;
                    timestamps.Add(timestamp + "");

                    JObject markNotationObject = new JObject();
                    markNotationObject.Add("x", notationGroup.x);
                    markNotationObject.Add("y", notationGroup.y);
                    markNotationObject.Add("text", notationGroup.text);
                    File.WriteAllText(Path.Combine(notationImagePath, timestamp + "-mark.json"), markNotationObject.ToString(), Encoding.UTF8);

                    // Other Notations
                }
                File.WriteAllText(Path.Combine(notationImagePath, "./index.json"), timestamps.ToString(), Encoding.UTF8);
            }

            xProject.Add(xImages);
            doc.Add(xProject);

            doc.Save(Path.Combine(project.path, project.fileName + ".yuzu"));
        }
    }
}
