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
using YuzuMarker.DataFormat;
using YuzuMarker.Utils;
using YuzuMarker.ViewModel;

namespace YuzuMarker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Base
        private YuzuProjectViewModel ViewModel;
        
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = DataContext as YuzuProjectViewModel;
        }
        #endregion
        
        #region Image Button Operation
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
                Manager.YuzuMarkerManager.Image.CreateAndLoadNewNotationGroup((int)ClickPoint.X, (int)ClickPoint.Y, "", false);
            }
            // Clear status
            ClickPoint.X = 0;
            ClickPoint.Y = 0;
            ClickTimestamp = 0;
        }
        #endregion

        #region TextArea Handler (Bottom Right)
        private void TextAreaOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.Right))) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (NotationGroupListView.SelectedIndex == 0) return;
                NotationGroupListView.SelectedIndex -= 1;
            }
            else
            {
                if (Manager.YuzuMarkerManager.Image.NotationGroups.IndexOf(Manager.YuzuMarkerManager.Group) ==
                    Manager.YuzuMarkerManager.Image.NotationGroups.Count - 1) return;
                NotationGroupListView.SelectedIndex += 1;
            }
        }
        #endregion

        // TODO: refactor start: 重写 lasso 交互
        // lasso 用 PointsCollection 没问题, 每次结束以后 / 开始之前 都需要和 cv::Mat 做运算，重新渲染，每次圈完以后 lasso 消失
        #region Canvas Item Clicking Event
        private void NotationRenderItemClicked(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SelectedNotationGroupItem = (YuzuNotationGroup)((ContentPresenter) sender).DataContext;
        }
        #endregion

        #region Lasso Drawing Event Handling
        private bool CanDefaultMouseMoveEventHappen(object sender, MouseEventArgs e)
        {
            return !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        private bool CanCustomMouseMoveEventHappen(object sender, MouseEventArgs e)
        {
            if (!ViewModel.LassoModeEnabled) return false;
            if (e.LeftButton == MouseButtonState.Released) return false;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) return false;
            return true;
        }
        
        private void CustomMouseMoveEvent(object sender, MouseEventArgs e)
        {
            var pc = ViewModel.LassoPoints.Clone();
            pc.Add(e.GetPosition((IInputElement) sender));
            ViewModel.LassoPoints = pc;
        }

        private void LassoMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pc = new PointCollection { e.GetPosition((IInputElement)sender) };
                ViewModel.LassoPoints = pc;
            }
        }
        #endregion

        #region Lasso UI Finish / Cancel Event Handling
        private void ShadeMouseDown(object sender, MouseButtonEventArgs e)
        {
            CancelLassoSelection();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ViewModel.LassoModeEnabled) return;
            if (e.Key != Key.Enter) return;

            if (ViewModel.LassoPoints.Count < 3)
            {
                try
                {
                    throw new Exception("未选择工作区域，操作失败");
                }
                catch (Exception ex)
                {
                    Utils.ExceptionHandler.ShowExceptionMessage(ex);
                    CancelLassoSelection();
                }
            }
            FinishLassoSelection();
        }
        #endregion

        #region Lasso Delegate Defining
        private delegate void LassoModeEventHandler();

        private event LassoModeEventHandler DidLassoModeFinished;
        #endregion

        #region Lasso Enable / Finish / Cancel Wrapping
        private YuzuCleaningNotation LastCleaningNotation = null;

        private void EnableLassoMode(LassoModeEventHandler DidLassoModeFinished)
        {
            LastCleaningNotation = ViewModel.SelectedNotationGroupItem.CleaningNotation;
            Manager.YuzuMarkerManager.PushMessage(ViewModel, "按住 Ctrl + 鼠标左键选择工作区域，点击画布外侧区域取消，Enter键确认区域");
            ViewModel.LassoPoints = new PointCollection();
            ViewModel.LassoModeEnabled = true;
            this.DidLassoModeFinished = DidLassoModeFinished;
        }

        private void DisableLassoMode()
        {
            ViewModel.LassoModeEnabled = false;
            Manager.YuzuMarkerManager.PopMessage(ViewModel);
        }

        private void CancelLassoSelection()
        {
            ViewModel.SelectedNotationGroupItem.CleaningNotation = LastCleaningNotation;
            ViewModel.RefreshImageList();
            DisableLassoMode();
        }

        private void FinishLassoSelection()
        {
            DidLassoModeFinished?.Invoke();
            DisableLassoMode();
        }
        #endregion

        #region Lasso CheckBox Group Handling
        private void DidLassoModeFinishedForCustomCleaning()
        {
            ViewModel.SelectedNotationGroupItem.CleaningNotation = 
                new YuzuCleaningNotation(YuzuCleaningNotationType.Custom, ViewModel.LassoPoints.ToGenericPoints());
            ViewModel.RefreshImageList();
        }

        private void CleaningCustomChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom) return;
            EnableLassoMode(DidLassoModeFinishedForCustomCleaning);
        }

        private void DidLassoModeFinishedForNormalCleaning()
        {
            ViewModel.SelectedNotationGroupItem.CleaningNotation =
                new YuzuCleaningNotation(YuzuCleaningNotationType.Normal, ViewModel.LassoPoints.ToGenericPoints());
            ViewModel.RefreshImageList();
        }
        
        private void CleaningNormalChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Normal) return;
            ViewModel.SelectedNotationGroupItem.CleaningNotation =
                new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
            ViewModel.RefreshImageList();
        }

        private void CleaningNormalLassoModeButtonClicked(object sender, RoutedEventArgs e)
        {
            EnableLassoMode(DidLassoModeFinishedForNormalCleaning);
        }
        #endregion
        // TODO: refactor end
    }
}
