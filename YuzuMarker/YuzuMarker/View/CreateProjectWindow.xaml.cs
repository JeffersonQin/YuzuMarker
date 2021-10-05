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
using System.Windows.Shapes;
using YuzuMarker.ViewModel;

namespace YuzuMarker.View
{
    /// <summary>
    /// CreateProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public delegate void CreateProjectEventHandler(object sender, string fileName, string projectName, string path);

        public event CreateProjectEventHandler CreateProjectEvent;

        private bool ButtonClicked = false;

        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            YuzuCreateProjectViewModel vm = DataContext as YuzuCreateProjectViewModel;
            CreateProjectEvent(this, vm.FileName, vm.ProjectName, vm.Path);
            ButtonClicked = true;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!ButtonClicked)
                CreateProjectEvent(this, "", "", "");
        }
    }
}
