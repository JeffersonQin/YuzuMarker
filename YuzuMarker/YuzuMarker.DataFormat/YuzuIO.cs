using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
            string projectFileName = Path.GetFileNameWithoutExtension(path);
            XDocument yuzuProjectXMLDoc = XDocument.Load(path);
            
            XElement xProject = yuzuProjectXMLDoc.Element("YuzuProject");
            string projectName = xProject.Element("Name").Value;

            string notationFolderPath = Path.Combine(Path.GetDirectoryName(path), "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            
            XElement xImages = xProject.Element("Images");
            LP yuzuImages = new LP();
            YuzuProject<LP, LI> yuzuProject = new YuzuProject<LP, LI>(Path.GetDirectoryName(path), projectFileName, projectName, yuzuImages);

            foreach (XElement xImage in xImages.Elements("Image"))
            {
                string imageName = xImage.Value;
                bool imageFinished = (bool)xImage.Attribute("IsFinished");
                YuzuImage<LI> yuzuImage = new YuzuImage<LI>(yuzuProject.path, imageName, imageFinished);
                yuzuProject.Images.Add(yuzuImage);

                string notationFolderPathForImage = Path.Combine(notationFolderPath, "./" + imageName + "/");
                IOUtils.EnsureDirectoryExist(notationFolderPathForImage);

                string indexFileContent = File.ReadAllText(Path.Combine(notationFolderPathForImage, "./index.json"));
                JArray notationIndexJArray = JArray.Parse(indexFileContent);
                foreach (JObject notationIndexJObject in notationIndexJArray)
                {
                    long timestamp = long.Parse(notationIndexJObject["timestamp"].ToString());
                    bool notationFinished = (bool)notationIndexJObject["finished"];

                    JObject markNotationJObject = JObject.Parse(File.ReadAllText(Path.Combine(notationFolderPathForImage, "./" + timestamp + "-mark.json")));
                    YuzuNotationGroup notationGroup = new YuzuNotationGroup(timestamp, (int)markNotationJObject["x"], (int)markNotationJObject["y"], (string)markNotationJObject["text"], notationFinished);

                    JObject cleaningNotationJObject = JObject.Parse(File.ReadAllText(Path.Combine(notationFolderPathForImage, "./" + timestamp + "-cleaning.json")));
                    var cleaningNotationType = (YuzuCleaningNotationType)int.Parse(cleaningNotationJObject["type"].ToString());
                    List<PointF> cleaningNotationPoints = new List<PointF>();
                    foreach (JObject cleaningNotationPoint in cleaningNotationJObject["points"] as JArray)
                    {
                        cleaningNotationPoints.Add(new PointF(float.Parse(cleaningNotationPoint["x"].ToString()), float.Parse(cleaningNotationPoint["y"].ToString())));
                    }
                    notationGroup.CleaningNotation = new YuzuCleaningNotation(cleaningNotationType, cleaningNotationPoints);
                    
                    // Other Notations

                    yuzuImage.NotationGroups.Add(notationGroup);
                }
            }

            return yuzuProject;
        }

        public static void SaveProject<LP, LI>(YuzuProject<LP, LI> project) 
            where LP : IList<YuzuImage<LI>>, new() where LI : IList<YuzuNotationGroup>, new()
        {
            XDocument yuzuProjectXMLDoc = new XDocument();
            yuzuProjectXMLDoc.Declaration = new XDeclaration("1.0", "utf-8", "no");
            
            XElement xProject = new XElement("YuzuProject");
            XElement xProjectName = new XElement("Name", project.projectName);
            xProject.Add(xProjectName);
            
            XElement xImages = new XElement("Images");

            string notationFolderPath = Path.Combine(project.path, "./Notations/");
            IOUtils.EnsureDirectoryExist(notationFolderPath);
            foreach (YuzuImage<LI> yuzuImage in project.Images)
            {
                XElement xImage = new XElement("Image", yuzuImage.ImageName);
                xImage.SetAttributeValue("IsFinished", yuzuImage.IsFinished);
                xImages.Add(xImage);

                string notationFolderPathForImage = Path.Combine(notationFolderPath, "./" + yuzuImage.ImageName + "/");
                IOUtils.EnsureDirectoryExist(notationFolderPathForImage);

                JArray notationIndexJArray = new JArray();

                foreach (YuzuNotationGroup notationGroup in yuzuImage.NotationGroups)
                {
                    long timestamp = notationGroup.Timestamp;
                    JObject notationIndexJObject = new JObject
                    {
                        { "timestamp", timestamp + "" },
                        { "finished", notationGroup.IsFinished }
                    };
                    notationIndexJArray.Add(notationIndexJObject);

                    JObject markNotationJObject = new JObject
                    {
                        { "x", notationGroup.x },
                        { "y", notationGroup.y },
                        { "text", notationGroup.text }
                    };
                    File.WriteAllText(Path.Combine(notationFolderPathForImage, timestamp + "-mark.json"), markNotationJObject.ToString(), Encoding.UTF8);

                    JArray cleaningNotationPointJArray = new JArray();
                    foreach (PointF point in notationGroup.CleaningNotation.CleaningPoints)
                    {
                        cleaningNotationPointJArray.Add(new JObject()
                        {
                            { "x", point.X },
                            { "y", point.Y }
                        });
                    }
                    JObject cleaningNotationJObject = new JObject()
                    {
                        {"type", (int) notationGroup.CleaningNotation.CleaningNotationType},
                        {"points", cleaningNotationPointJArray}
                    };
                    File.WriteAllText(Path.Combine(notationFolderPathForImage, timestamp + "-cleaning.json"), cleaningNotationJObject.ToString(), Encoding.UTF8);

                    // Other Notations
                }
                File.WriteAllText(Path.Combine(notationFolderPathForImage, "./index.json"), notationIndexJArray.ToString(), Encoding.UTF8);
            }

            xProject.Add(xImages);
            yuzuProjectXMLDoc.Add(xProject);

            yuzuProjectXMLDoc.Save(Path.Combine(project.path, project.fileName + ".yuzu"));
        }
    }
}
