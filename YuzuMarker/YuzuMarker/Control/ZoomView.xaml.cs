using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YuzuMarker.Control
{
    /// <summary>
    /// ZoomView.xaml 的交互逻辑
    /// </summary>
    public partial class ZoomView : UserControl
    {
        public int ContentWidth
        {
            get { return (int)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(int), typeof(ZoomView), new PropertyMetadata(0));



        public int ContentHeight
        {
            get { return (int)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register("ContentHeight", typeof(int), typeof(ZoomView), new PropertyMetadata(0));



        public UIElement ContainerContent
        {
            get { return (UIElement)GetValue(ContainerContentProperty); }
            set { SetValue(ContainerContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainerContentProperty =
            DependencyProperty.Register("ContainerContent", typeof(UIElement), typeof(ZoomView), new PropertyMetadata(null));



        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(ZoomView), new PropertyMetadata(null));



        public delegate bool CanMouseEventHandler(object sender, MouseEventArgs e);


        public MouseEventHandler CustomMouseMoveEvent
        {
            get { return (MouseEventHandler)GetValue(CustomMouseMoveEventProperty); }
            set { SetValue(CustomMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomMouseMoveEventProperty =
            DependencyProperty.Register("CustomMouseMoveEvent", typeof(MouseEventHandler), typeof(ZoomView), new PropertyMetadata(null));



        public CanMouseEventHandler CanCustomMouseMoveEvent
        {
            get { return (CanMouseEventHandler)GetValue(CanCustomMouseMoveEventProperty); }
            set { SetValue(CanCustomMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanCustomMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanCustomMouseMoveEventProperty =
            DependencyProperty.Register("CanCustomMouseMoveEvent", typeof(CanMouseEventHandler), typeof(ZoomView), new PropertyMetadata(null));



        public CanMouseEventHandler CanDefaultMouseMoveEvent
        {
            get { return (CanMouseEventHandler)GetValue(CanDefaultMouseMoveEventProperty); }
            set { SetValue(CanDefaultMouseMoveEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDefaultMouseMoveEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanDefaultMouseMoveEventProperty =
            DependencyProperty.Register("CanDefaultMouseMoveEvent", typeof(CanMouseEventHandler), typeof(ZoomView), new PropertyMetadata(null));



        public ZoomView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public double Scale = 1;

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            if (ContentWidth == 0 || ContentHeight == 0)
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

            if (CanDefaultMouseMoveEvent == null || CanDefaultMouseMoveEvent(sender, e))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
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

            if (CanCustomMouseMoveEvent == null || CanCustomMouseMoveEvent(sender, e))
            {
                CustomMouseMoveEvent?.Invoke(sender, e);
            }
        }
    }
}
