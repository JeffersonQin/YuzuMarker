using System.Drawing;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuColorCleaningNotation : YuzuCleaningNotation
    {
        public YuzuColorCleaningNotation(YuzuNotationGroup parentNotationGroup) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Color) {}
        
        public YuzuColorCleaningNotation(YuzuNotationGroup parentNotationGroup, Color color) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Color)
        {
            CleaningNotationColor = color;
        }

        private Color _cleaningNotationColor;

        public Color CleaningNotationColor
        {
            get => _cleaningNotationColor;
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = CleaningNotationColor;
                SetProperty(ref _cleaningNotationColor, (Color)o);
                return nowValue;
            }, o =>
            {
                var nowValue = CleaningNotationColor;
                o ??= value;
                SetProperty(ref _cleaningNotationColor, (Color)o);
                return nowValue;
            });
        }
    }
}