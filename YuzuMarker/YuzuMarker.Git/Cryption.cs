using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Git
{
    public static class Cryption
    {
        public static void SetProjectEncryptStatus(YuzuProject project, string password)
        {
            string path = project.Path;
            // encrypt: password is not "" or null
            JObject yuzuGitSettings = new JObject();
            if (!string.IsNullOrEmpty(password))
            {
                yuzuGitSettings.Add("encryption", true);
                File.WriteAllText(Path.Combine(path, "./.gitignore"), "Images/\nPSD/\n.password");
                File.WriteAllText(Path.Combine(path, "./.password"), password);
            } else
            {
                yuzuGitSettings.Add("encryption", false);
                if (File.Exists(Path.Combine(path, "./.gitignore")))
                    File.Delete(Path.Combine(path, "./.gitignore"));
                if (File.Exists(Path.Combine(path, "./.password")))
                    File.Delete(Path.Combine(path, "./.password"));
                if (Directory.Exists(Path.Combine(path, "./Images-Encrypted")))
                    Directory.Delete(Path.Combine(path, "./Images-Encrypted"));
                if (Directory.Exists(Path.Combine(path, "./PSD-Encrypted")))
                    Directory.Delete(Path.Combine(path, "./PSD-Encrypted"));
            }
            File.WriteAllText(Path.Combine(path, "./.yuzugitsettings"), yuzuGitSettings.ToString());
        }

        public static bool GetProjectEncryptStatus(YuzuProject project)
        {
            string path = project.Path;
            if (!File.Exists(Path.Combine(path, "./.yuzugitsettings"))) return false;
            JObject yuzuSettings = JObject.Parse(File.ReadAllText(Path.Combine(path, "./.yuzugitsettings")));
            return (bool)yuzuSettings["encryption"];
        }

        public static void EncryptProject(YuzuProject project)
        {
            string path = project.Path;
            if (!File.Exists(Path.Combine(path, "./.password")))
                throw new Exception("YuzuMarker.Git Error: .password file does not exist. Please configure correctly.");
            // TODO
        }

        public static void DecryptProject(YuzuProject project)
        {
            // TODO
        }
    }
}
