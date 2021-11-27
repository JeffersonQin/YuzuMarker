using System.Windows;
using ColorPicker.Models;
using ColorPicker;

namespace YuzuMarker.Control
{
    /// <summary>
    /// Modified from: https://github.com/PixiEditor/ColorPicker/blob/master/src/ColorPicker/PortableColorPicker.xaml.cs
    /// </summary>
    public partial class CustomPortableColorPicker : DualPickerControlBase
    {
        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(CustomPortableColorPicker),
                new PropertyMetadata(1.0));

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(CustomPortableColorPicker),
                new PropertyMetadata(true));
        
        public PickerType PickerType
        {
            get => (PickerType)GetValue(PickerTypeProperty);
            set => SetValue(PickerTypeProperty, value);
        }

        public static readonly DependencyProperty PickerTypeProperty
            = DependencyProperty.Register(nameof(PickerType), typeof(PickerType), typeof(CustomPortableColorPicker),
                new PropertyMetadata(PickerType.HSV));

        public RoutedEventHandler ColorPickingStartedEventHandler
        {
            get => (RoutedEventHandler)GetValue(ColorPickingStartedEventHandlerProperty);
            set => SetValue(ColorPickingStartedEventHandlerProperty, value);
        }

        public static readonly DependencyProperty ColorPickingStartedEventHandlerProperty =
            DependencyProperty.Register(nameof(ColorPickingStartedEventHandler), typeof(RoutedEventHandler), typeof(CustomPortableColorPicker), 
                new PropertyMetadata(null));
        
        public RoutedEventHandler ColorPickingFinishedEventHandler
        {
            get => (RoutedEventHandler)GetValue(ColorPickingFinishedEventHandlerProperty);
            set => SetValue(ColorPickingFinishedEventHandlerProperty, value);
        }

        public static readonly DependencyProperty ColorPickingFinishedEventHandlerProperty =
            DependencyProperty.Register(nameof(ColorPickingFinishedEventHandler), typeof(RoutedEventHandler), typeof(CustomPortableColorPicker), 
                new PropertyMetadata(null));

        public CustomPortableColorPicker()
        {
            InitializeComponent();
        }

        private void toggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ColorPickingStartedEventHandler(sender, e);
        }

        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ColorPickingFinishedEventHandler(sender, e);
        }
    }
}