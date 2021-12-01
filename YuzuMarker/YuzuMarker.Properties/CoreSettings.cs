using System;

namespace YuzuMarker.Properties
{
    public enum PSBridgeType: uint
    {
        COM = 0,
        Extension = 1
    }

    public static class CoreSettings
    {
        // remember to load settings when wpf starts up
        public static int PhotoshopExtensionHTTPServerPort = 4016;

        public static int ImageProcessingHTTPServerPort = 1029;

        public static PSBridgeType PhotoshopBridgeType = PSBridgeType.Extension;
    }
}
