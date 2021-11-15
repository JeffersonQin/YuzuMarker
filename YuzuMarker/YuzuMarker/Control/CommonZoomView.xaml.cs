using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace YuzuMarker.Control
{
    public partial class CommonZoomView : UserControl
    {
        public int ContentWidth
        {
            get { return (int)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(int), typeof(CommonZoomView), new PropertyMetadata(0));



        public int ContentHeight
        {
            get { return (int)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register("ContentHeight", typeof(int), typeof(CommonZoomView), new PropertyMetadata(0));



        public UIElement ContainerContent
        {
            get { return (UIElement)GetValue(ContainerContentProperty); }
            set { SetValue(ContainerContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainerContentProperty =
            DependencyProperty.Register("ContainerContent", typeof(UIElement), typeof(CommonZoomView), new PropertyMetadata(null));



        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(CommonZoomView), new PropertyMetadata(null));



        public delegate bool CanMouseEventHandler(object sender, MouseEventArgs e);


        public MouseEventHandler CustomMouseMoveEvent
        {
            get { return (MouseEventHandler)GetValue(CustomMouseMoveEventProperty); }
            set { SetValue(CustomMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomMouseMoveEventProperty =
            DependencyProperty.Register("CustomMouseMoveEvent", typeof(MouseEventHandler), typeof(CommonZoomView), new PropertyMetadata(null));



        public CanMouseEventHandler CanCustomMouseMoveEvent
        {
            get { return (CanMouseEventHandler)GetValue(CanCustomMouseMoveEventProperty); }
            set { SetValue(CanCustomMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanCustomMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanCustomMouseMoveEventProperty =
            DependencyProperty.Register("CanCustomMouseMoveEvent", typeof(CanMouseEventHandler), typeof(CommonZoomView), new PropertyMetadata(null));



        public CanMouseEventHandler CanDefaultMouseMoveEvent
        {
            get { return (CanMouseEventHandler)GetValue(CanDefaultMouseMoveEventProperty); }
            set { SetValue(CanDefaultMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDefaultMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanDefaultMouseMoveEventProperty =
            DependencyProperty.Register("CanDefaultMouseMoveEvent", typeof(CanMouseEventHandler), typeof(CommonZoomView), new PropertyMetadata(null));

        
        public CommonZoomView()
        {
            InitializeComponent();
            DataContext = this;
            ZoomScrollViewer.ShouldScrollEventHandler = (e) =>
                !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        public double Scale = 1;

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            if (ContentWidth == 0 || ContentHeight == 0)
                return;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                return;
            
            //if (scale < 0.15 && e.Delta < 0)
            //    return;
            //if (scale > 16 && e.Delta > 0)
            //    return;

            double lastScale = Scale;

            Scale *= (e.Delta > 0 ? 1.2 : 1 / 1.2);
            SetScale();

            Point mousePosition = e.GetPosition(ContentControlInstance);
            double deltaX = mousePosition.X * (Scale - lastScale);
            double deltaY = mousePosition.Y * (Scale - lastScale);
            if (deltaX != 0)
                ZoomScrollViewer.ScrollToHorizontalOffset(ZoomScrollViewer.HorizontalOffset + deltaX);
            if (deltaY != 0)
                ZoomScrollViewer.ScrollToVerticalOffset(ZoomScrollViewer.VerticalOffset + deltaY);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (ContentWidth == 0 || ContentHeight == 0)
                return;

            double lastScale = Scale;
            Scale *= e.DeltaManipulation.Scale.X;
            SetScale();

            var scaleOrigin = e.ManipulationOrigin;
            var deltaX = (scaleOrigin.X + ZoomScrollViewer.HorizontalOffset) 
                / lastScale * (Scale - lastScale) - e.DeltaManipulation.Translation.X;
            var deltaY = (scaleOrigin.Y + ZoomScrollViewer.VerticalOffset) 
                / lastScale * (Scale - lastScale) - e.DeltaManipulation.Translation.Y;

            if (deltaX != 0)
                ZoomScrollViewer.ScrollToHorizontalOffset(ZoomScrollViewer.HorizontalOffset + deltaX);
            if (deltaY != 0)
                ZoomScrollViewer.ScrollToVerticalOffset(ZoomScrollViewer.VerticalOffset + deltaY);
        }

        public void SetScale()
        {
            ContentControlInstance.LayoutTransform = new ScaleTransform(Scale, Scale);
        }

        public void SetScale(double Scale)
        {
            this.Scale = Scale;
            SetScale();
        }

        private Point LastPoint = new Point(0, 0);

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (ContentWidth == 0 || ContentHeight == 0)
                return;

            if (CanDefaultMouseMoveEvent == null || CanDefaultMouseMoveEvent(ContainerContent, e))
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    Point newPoint = e.GetPosition((ScrollViewer)sender);
                    if (LastPoint.X == 0 && LastPoint.Y == 0)
                    {
                        LastPoint = newPoint;
                        return;
                    }
                    double deltaX = LastPoint.X - newPoint.X;
                    double deltaY = LastPoint.Y - newPoint.Y;
                    if (deltaX != 0)
                        ZoomScrollViewer.ScrollToHorizontalOffset(ZoomScrollViewer.HorizontalOffset + deltaX);
                    if (deltaY != 0)
                        ZoomScrollViewer.ScrollToVerticalOffset(ZoomScrollViewer.VerticalOffset + deltaY);
                    LastPoint = newPoint;
                }
                else
                {
                    LastPoint.X = 0;
                    LastPoint.Y = 0;
                }
            }

            if (CanCustomMouseMoveEvent == null || CanCustomMouseMoveEvent(ContainerContent, e))
            {
                CustomMouseMoveEvent?.Invoke(ContainerContent, e);
            }
        }
    }
}