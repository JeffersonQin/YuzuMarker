using System;
using System.Collections.Generic;
using System.Text;
using YuzuMarker.Files;

namespace YuzuMarker.Manager
{
    public static class YuzuMarkerManager
    {
        private static YuzuProject _Project = null;

        public static YuzuProject Project {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
            }
        }

        private static YuzuImage _Image = null;

        public static YuzuImage Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
            }
        }
    }
}
