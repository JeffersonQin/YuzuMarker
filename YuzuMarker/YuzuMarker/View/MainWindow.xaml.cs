using System;
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
using YuzuMarker.Files;
using YuzuMarker.ViewModel;

namespace YuzuMarker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private YuzuProjectViewModel YuzuProjectVM;

        public MainWindow()
        {
            InitializeComponent();
            YuzuProjectVM = this.DataContext as YuzuProjectViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //YuzuProject project = YuzuIO.CreateProject("C:/Users/JeffersonQin/Desktop", "testyuzu", "测试");
            //project.AddImage("C:/Users/JeffersonQin/Desktop/QQ图片20210724200932.jpg");
            //project.AddImage("C:/Users/JeffersonQin/Desktop/QQ图片20210724200950.png");
            //project.InsertImageAt(0, "C:/Users/JeffersonQin/Desktop/netease-music.gif");
            //project.Images[0].AddSimpleNotation(3, 2, "jfdlksj5379&(%#\n\rfdslkag");
            //project.Images[0].AddSimpleNotation(46, 65, "54326&(%#\n\rfdslkag");
            //YuzuIO.SaveProject(project);
            //YuzuProject project = YuzuIO.LoadProject("C:/Users/JeffersonQin/Desktop/testyuzu/testyuzu.yuzu");
            //project.Images[1].AddSimpleNotation(5432654, 432543, "fdsagd");
            //YuzuIO.SaveProject(project);
        }

        private void MenuBarLoadProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void MenuBarCreateProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
