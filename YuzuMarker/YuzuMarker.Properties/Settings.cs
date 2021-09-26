using System;

namespace YuzuMarker.Properties
{
    public enum PSBridgeType: uint
    {
        COM = 0,
        Extension = 1
    }

    public static class Settings
    {
        // TODO: when wpf starts up, load settings
        public static int PhotoshopExtensionHTTPServerPort = 4016;

        public static PSBridgeType PhotoshopBridgeType = PSBridgeType.Extension;
    }
}
