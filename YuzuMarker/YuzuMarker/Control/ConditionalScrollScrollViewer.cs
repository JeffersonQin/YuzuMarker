using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace YuzuMarker.Control
{
    public class ConditionalScrollScrollViewer : ScrollViewer
    {
        public delegate bool ShouldScrollDelegate(MouseWheelEventArgs e);

        public ShouldScrollDelegate ShouldScrollEventHandler;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (ShouldScrollEventHandler(e))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ScrollToHorizontalOffset(HorizontalOffset - e.Delta);
                    e.Handled = true;
                }
                else
                {
                    ScrollToVerticalOffset(VerticalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
    }
}