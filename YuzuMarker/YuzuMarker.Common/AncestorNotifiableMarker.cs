using System;

namespace YuzuMarker.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AncestorNotifiableMarker : Attribute
    {
        public string AncestorName { get; set; }

        public AncestorNotifiableMarker(string ancestorName)
        {
            AncestorName = ancestorName;
        }
    }
}