using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public class BasicYuzuNotation : NotifyObject
    {
        private BasicYuzuNotationGroup _parentNotationGroup;

        public BasicYuzuNotationGroup ParentNotationGroup
        {
            get => _parentNotationGroup;
            set => SetProperty(ref _parentNotationGroup, value);
        }

        public BasicYuzuNotation(BasicYuzuNotationGroup parentNotationGroup)
        {
            ParentNotationGroup = parentNotationGroup;
        }
    }
}