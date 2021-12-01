using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json.Linq;
using YuzuMarker.PSBridge.Extension;

namespace YuzuMarker.IPC
{
    public static class Invoker
    {
        private static void StartServer()
        {
            var processList = System.Diagnostics.Process.GetProcesses();
            var flag = processList.Any(process => process.ProcessName == "YuzuIPS");
            if (!flag) System.Diagnostics.Process.Start("YuzuIPS/YuzuIPS.exe");
        }
        
        public static Color DetectMaxColor(string src, string mask)
        {
            StartServer();
            var ret = WebUtil.GET(Properties.CoreSettings.ImageProcessingHTTPServerPort, 
                "detect_max_color_bgr", new Dictionary<string, string>
                {
                    { "src", src },
                    { "mask", mask },
                });
            try
            {
                return Color.FromArgb(ret.Value<int>("r"), ret.Value<int>("g"), ret.Value<int>("b"));
            }
            catch
            {
                throw new Exception("YuzuMarker.IPC.Invoker: failed to read data");
            }
        }

        public static Color DetectPeakColor(string src, string mask, double threshold = 0.1, int minDist = 1, 
            bool thresholdAbs = false, int preferredR = 255, int preferredG = 255, int preferredB = 255)
        {
            StartServer();
            var ret = WebUtil.GET(Properties.CoreSettings.ImageProcessingHTTPServerPort, 
                "detect_peak_color_bgr", new Dictionary<string, string>
                {
                    { "src", src },
                    { "mask", mask },
                    { "thres", threshold + "" },
                    { "min_dist", minDist + "" },
                    { "thres_abs", thresholdAbs + "" },
                    { "preferred_r", preferredR + "" },
                    { "preferred_g", preferredG + "" },
                    { "preferred_b", preferredB + "" }
                });
            try
            {
                return Color.FromArgb(ret.Value<int>("r"), ret.Value<int>("g"), ret.Value<int>("b"));
            }
            catch
            {
                throw new Exception("YuzuMarker.IPC.Invoker: failed to read data");
            }
        }
    }
}