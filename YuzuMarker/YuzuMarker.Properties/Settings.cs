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

        public static uint CustomCleaningStrokeColor = 0xFFFF7FB3;

        public static uint NormalCleaningStrokeColor = 0xFFACF6AE;
        
        public static uint CustomCleaningFillColor = 0x7FFF7FB3;

        public static uint NormalCleaningFillColor = 0x7FACF6AE;
    }
}
