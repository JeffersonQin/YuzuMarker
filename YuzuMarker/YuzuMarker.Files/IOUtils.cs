using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.Files
{
    public static class IOUtils
    {
        public static bool JudgeFilePath(string path)
        {
            if (path.Contains('?') || path.Contains('|') || path.Contains('"') ||
                path.Contains('<') || path.Contains('>') || path.Contains('*'))
                return false;
            return true;
        }

        public static bool JudgeFileName(string name)
        {
            if (name.Contains('/') || name.Contains('?') || name.Contains('|') || name.Contains('"') ||
                name.Contains('\\') || name.Contains('<') || name.Contains('>') || name.Contains('*'))
                return false;
            return true;
        }
    }
}
