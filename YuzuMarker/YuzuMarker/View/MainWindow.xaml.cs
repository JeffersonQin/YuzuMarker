﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuzuMarker.ViewModel;

namespace YuzuMarker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private YuzuProjectViewModel ViewModel;
        
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = DataContext as YuzuProjectViewModel;
        }

        private Point ClickPoint = new Point(0, 0);
        private long ClickTimestamp = 0;

        private void ImageAreaMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.LabelMode) return;
            ClickPoint = e.GetPosition((IInputElement) sender);
            ClickTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }

        private void ImageAreaMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.LabelMode) return;
            Point NewClickPoint = e.GetPosition((IInputElement) sender);
            long TimestampNow = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            if (ClickPoint.X == NewClickPoint.X && ClickPoint.Y == NewClickPoint.Y && TimestampNow - ClickTimestamp <= 500)
            {
                Manager.YuzuMarkerManager.Image.CreateNewNotation((int)ClickPoint.X, (int)ClickPoint.Y, "", false);
            }
            // Clear status
            ClickPoint.X = 0;
            ClickPoint.Y = 0;
            ClickTimestamp = 0;
        }

        private void TextAreaOnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshImageList();
        }

        private void TextAreaOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.Right))) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (NotationGroupListView.SelectedIndex == 0) goto Finish;
                NotationGroupListView.SelectedIndex -= 1;
            }
            else
            {
                if (Manager.YuzuMarkerManager.Image.NotationGroups.IndexOf(Manager.YuzuMarkerManager.Group) ==
                    Manager.YuzuMarkerManager.Image.NotationGroups.Count - 1)
                    goto Finish;
                NotationGroupListView.SelectedIndex += 1;
            }
            Finish:
            ViewModel.RefreshImageList();
        }
    }
}
