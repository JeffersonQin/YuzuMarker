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

        public static PSBridgeType PhotoshopBridgeType = PSBridgeType.Extension;

        // TODO: The following properties have not been added to settings
        public static string MaskImageType = "jpg";
    }
}
