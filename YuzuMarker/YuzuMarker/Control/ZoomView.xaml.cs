﻿using System;
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

        public ZoomView()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
