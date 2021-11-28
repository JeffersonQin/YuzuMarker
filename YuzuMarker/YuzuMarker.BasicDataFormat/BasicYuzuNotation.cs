using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    [AncestorNotifiableMarker("ParentNotationGroup")]
    public class BasicYuzuNotation : ChainNotifiableObject
    {
        private BasicYuzuNotationGroup _parentNotationGroup;

        public BasicYuzuNotationGroup ParentNotationGroup
        {
            get => _parentNotationGroup;
            set => SetProperty(value);
        }

        public BasicYuzuNotation(BasicYuzuNotationGroup parentNotationGroup)
        {
            ParentNotationGroup = parentNotationGroup;
        }
    }
}