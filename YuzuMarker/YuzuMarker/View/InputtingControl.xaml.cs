using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using YuzuMarker.DataFormat;
using YuzuMarker.BasicDataFormat;
using System.Windows;

namespace YuzuMarker.View
{
    /// <summary>
    /// InputtingControl.xaml 的交互逻辑
    /// </summary>
    public partial class InputtingControl : UserControl
    {
        public InputtingControl()
        {
            InitializeComponent();
        }

        public YuzuNotationGroup SelectedNotationGroupItem
        {
            get { return (YuzuNotationGroup)GetValue(SelectedNotationGroupItemProperty); }
            set { SetValue(SelectedNotationGroupItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedNotationGroupItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedNotationGroupItemProperty =
            DependencyProperty.Register("SelectedNotationGroupItem", typeof(YuzuNotationGroup), typeof(InputtingControl), new PropertyMetadata(null));


        private ObservableCollection<BasicYuzuNotationGroup> NotationGroups
            => SelectedNotationGroupItem?.ParentImage.NotationGroups;

        private void TextAreaOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.Right))) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (NotationGroups.IndexOf(SelectedNotationGroupItem) == 0) return;
                SelectedNotationGroupItem = (YuzuNotationGroup)NotationGroups[NotationGroups.IndexOf(SelectedNotationGroupItem) - 1];
            }
            else
            {
                if (NotationGroups.IndexOf(SelectedNotationGroupItem) == NotationGroups.Count - 1) return;
                SelectedNotationGroupItem = (YuzuNotationGroup)NotationGroups[NotationGroups.IndexOf(SelectedNotationGroupItem) + 1];
            }
        }
    }
}
