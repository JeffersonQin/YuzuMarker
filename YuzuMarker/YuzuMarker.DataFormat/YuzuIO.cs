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
            return (YuzuProject)BasicYuzuIO.LoadProject(path, (timestamp, x, y, text, finished, rootDir) =>
            {
                var notationGroup = new YuzuNotationGroup(timestamp, x, y, text, finished);

                var cleaningNotationJObject = JObject.Parse(File.ReadAllText(Path.Combine(rootDir, "./" + timestamp + "-cleaning.json")));
                var cleaningNotationType = (YuzuCleaningNotationType)int.Parse(cleaningNotationJObject["type"].ToString());
                var cleaningNotationPoints = 
                    (from JObject cleaningNotationPoint in (JArray) cleaningNotationJObject["points"] 
                        select new PointF(float.Parse(cleaningNotationPoint["x"].ToString()), float.Parse(cleaningNotationPoint["y"].ToString()))).ToList();
                notationGroup.CleaningNotation = new YuzuCleaningNotation(cleaningNotationType, cleaningNotationPoints);
                
                // Other Notations

                return notationGroup;
            }, (projectPath, fileName, projectName, images) => new YuzuProject(projectPath, fileName, projectName, images), 
               (parentPath, imageName, finished) => new YuzuImage(parentPath, imageName, finished));
        }

        public static void SaveProject(YuzuProject project)
        {
            BasicYuzuIO.SaveProject(project, (basicGroup, rootDir) =>
            {
                var notationGroup = basicGroup as YuzuNotationGroup;
                
                var cleaningNotationPointJArray = new JArray();
                foreach (var point in notationGroup.CleaningNotation.CleaningPoints)
                {
                    cleaningNotationPointJArray.Add(new JObject()
                    {
                        { "x", point.X },
                        { "y", point.Y }
                    });
                }
                var cleaningNotationJObject = new JObject()
                {
                    {"type", (int) notationGroup.CleaningNotation.CleaningNotationType},
                    {"points", cleaningNotationPointJArray}
                };
                File.WriteAllText(Path.Combine(rootDir, notationGroup.Timestamp + "-cleaning.json"), cleaningNotationJObject.ToString(), Encoding.UTF8);

                // Other Notations
            });
        }
    }
}
