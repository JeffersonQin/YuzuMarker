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
                Manager.YuzuMarkerManager.Image.CreateNewNotation((int)ClickPoint.X, (int)ClickPoint.Y, "", false);
            }
            // Clear status
            ClickPoint.X = 0;
            ClickPoint.Y = 0;
            ClickTimestamp = 0;
        }
        #endregion

        #region TextArea Handler (Bottom Right)
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
        #endregion

        #region Lasso Drawing Event Handling
        private bool CanDefaultMouseMoveEventHappen(object sender, MouseEventArgs e)
        {
            return !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }
        
        private void CustomMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (!ViewModel.LassoMode) return;
            if (e.LeftButton == MouseButtonState.Released) return;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) return;

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
            if (!ViewModel.LassoMode) return;
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

        #region Lasso Finish / Cancel Delegate Defining
        private delegate void LassoModeEventHandler();

        private event LassoModeEventHandler DidLassoModeCancelled;

        private event LassoModeEventHandler DidLassoModeFinished;
        #endregion

        #region Lasso Enable / Finish / Cancel Wrapping
        private void EnableLassoMode(LassoModeEventHandler DidLassoModeEnabled, LassoModeEventHandler DidLassoModeCancelled, LassoModeEventHandler DidLassoModeFinished)
        {
            DidLassoModeEnabled?.Invoke();
            Manager.YuzuMarkerManager.PushMessage(ViewModel, "按住 Ctrl + 鼠标左键选择工作区域，点击画布外侧区域取消，Enter键确认区域");
            ViewModel.LassoPoints = new PointCollection();
            ViewModel.LassoMode = true;
            this.DidLassoModeCancelled = DidLassoModeCancelled;
            this.DidLassoModeFinished = DidLassoModeFinished;
        }

        private void DisableLassoMode()
        {
            ViewModel.LassoMode = false;
            Manager.YuzuMarkerManager.PopMessage(ViewModel);
        }

        private void CancelLassoSelection()
        {
            DidLassoModeCancelled?.Invoke();
            DisableLassoMode();
        }

        private void FinishLassoSelection()
        {
            DidLassoModeFinished?.Invoke();
            DisableLassoMode();
        }
        #endregion

        private void DidLassoModeFinishedForCustomCleaning()
        {
            ViewModel.SelectedNotationGroupItem.CleaningNotation = new DataFormat.YuzuCleaningNotation(DataFormat.YuzuCleaningNotationType.Custom, ViewModel.LassoPoints.ToGenericPoints());
            ViewModel.RefreshImageList();
        }

        private void CleaningCustomChecked(object sender, RoutedEventArgs e)
        {
            // TODO: 暂存上一个CleaningType (enum)
            // TODO: 恢复暂存的CleaningType, 刷新 Data Binding
            EnableLassoMode(null, null, DidLassoModeFinishedForCustomCleaning);
        }
    }
}
