using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YuzuMarker.DataFormat
{
    public static class IOUtils
    {
        public static bool JudgeFilePath(string path)
        {
            if (path.Contains("?") || path.Contains("|") || path.Contains("\"") ||
                path.Contains("<") || path.Contains(">") || path.Contains("*"))
                return false;
            return true;
        }

        public static bool JudgeFileName(string name)
        {
            if (name.Contains("/") || name.Contains("?") || name.Contains("|") || name.Contains("\"") ||
                name.Contains("\\") || name.Contains("<") || name.Contains(">") || name.Contains("*"))
                return false;
            return true;
        }

        public static void EnsureDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
