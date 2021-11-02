using System.Windows.Controls;
using System.Windows.Media;

namespace YuzuMarker.Control
{
    public class NonClickableImage : Image
    {
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }
    }
}