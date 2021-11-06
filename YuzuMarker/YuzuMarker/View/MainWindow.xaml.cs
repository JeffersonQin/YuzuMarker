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
using OpenCvSharp;
using YuzuMarker.Common;
using YuzuMarker.DataFormat;
using YuzuMarker.Model;
using YuzuMarker.Utils;
using YuzuMarker.ViewModel;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

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
                UndoRedoManager.IgnoreOtherRecording = true;
                ViewModel.SelectedImageItem.CreateAndLoadNewNotationGroup((int)ClickPoint.X, (int)ClickPoint.Y, "", false);
                UndoRedoManager.IgnoreOtherRecording = false;
                var newGroup = ViewModel.SelectedImageItem.NotationGroups[^1];
                UndoRedoManager.PushRecord(null, value => { }, () => null, (o) =>
                {
                    ViewModel.SelectedImageItem.NotationGroups.RemoveAt(ViewModel.SelectedImageItem.NotationGroups.Count - 1);
                }, o =>
                {
                    ViewModel.SelectedImageItem.NotationGroups.Add(newGroup);
                });
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
                if (ViewModel.SelectedImageItem.NotationGroups.IndexOf(ViewModel.SelectedNotationGroupItem) ==
                    ViewModel.SelectedImageItem.NotationGroups.Count - 1) return;
                NotationGroupListView.SelectedIndex += 1;
            }
        }
        #endregion
        
        #region Canvas Item Clicking Event
        private void NotationRenderItemClicked(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SelectedNotationGroupItem = (YuzuNotationGroup)((ContentPresenter) sender).DataContext;
        }
        #endregion

        #region Selection Drawing Event Handling
        private bool CanDefaultMouseMoveEventHappen(object sender, MouseEventArgs e)
        {
            return !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        private bool CanCustomMouseMoveEventHappen(object sender, MouseEventArgs e)
        {
            if (!ViewModel.SelectionModeEnabled) return false;
            if (e.LeftButton == MouseButtonState.Released) return false;
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) return false;
            return true;
        }
        
        // Handle Selection Drawing
        private void CustomMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (!ViewModel.SelectionDrawing) return;
            var mousePoint = e.GetPosition((IInputElement) sender);
            switch (ViewModel.SelectionType)
            {
                case SelectionType.Lasso:
                    var pc = ViewModel.LassoPoints.Clone();
                    pc.Add(mousePoint);
                    ViewModel.LassoPoints = pc;
                    break;
                case SelectionType.Rectangle:
                    ViewModel.RectangleShapeData.Width = (float)(mousePoint.X - ViewModel.RectangleShapeData.X);
                    ViewModel.RectangleShapeData.Height = (float)(mousePoint.Y - ViewModel.RectangleShapeData.Y);
                    ViewModel.RaisePropertyChanged("RectangleShapeData");
                    break;
                case SelectionType.Oval:
                    ViewModel.OvalShapeData.Width = (float)(mousePoint.X - ViewModel.OvalShapeData.X);
                    ViewModel.OvalShapeData.Height = (float)(mousePoint.Y - ViewModel.OvalShapeData.Y);
                    ViewModel.RaisePropertyChanged("OvalShapeData");
                    break;
            }
        }

        // Start Selection Drawing
        private void SelectionMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ViewModel.SelectionDrawing = true;
                var mousePoint = e.GetPosition((IInputElement) sender);
                switch (ViewModel.SelectionType)
                {
                    case SelectionType.Lasso:
                        var pc = new PointCollection { mousePoint };
                        ViewModel.LassoPoints = pc;
                        break;
                    case SelectionType.Rectangle:
                        ViewModel.RectangleShapeData.X = (float)mousePoint.X;
                        ViewModel.RectangleShapeData.Y = (float)mousePoint.Y;
                        ViewModel.RectangleShapeData.Width = ViewModel.RectangleShapeData.Height = 0;
                        ViewModel.RaisePropertyChanged("RectangleShapeData");
                        break;
                    case SelectionType.Oval:
                        ViewModel.OvalShapeData.X = (float)mousePoint.X;
                        ViewModel.OvalShapeData.Y = (float)mousePoint.Y;
                        ViewModel.RectangleShapeData.Width = ViewModel.RectangleShapeData.Height = 0;
                        ViewModel.RaisePropertyChanged("OvalShapeData");
                        break;
                }
            }
        }
        
        // Stop Selection Drawing
        private void Window_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectionDrawing) ViewModel.SelectionDrawing = false;
        }
        
        // Stop Selection Drawing
        private void Window_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) ViewModel.SelectionDrawing = false;
        }
        #endregion

        #region Selection UI Finish / Cancel Event Handling
        // Cancel Selection
        private void ShadeMouseDown(object sender, MouseButtonEventArgs e)
        {
            UndoRedoManager.StartContinuousRecording();
            DisableSelectionMode();
            UndoRedoManager.StopContinuousRecording();
        }

        // Finish Selection
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ViewModel.SelectionModeEnabled) return;
            if (e.Key != Key.Enter) return;
            UndoRedoManager.StartContinuousRecording();
            
            DidSelectionModeFinished?.Invoke();
            DisableSelectionMode();
            
            UndoRedoManager.StopContinuousRecording();
        }
        #endregion

        #region Selection Delegate Defining
        private delegate void SelectionModeEventHandler();

        private event SelectionModeEventHandler DidSelectionModeFinished;
        #endregion

        #region Selection Enable / Finish Wrapping
        private void EnableSelectionMode(SelectionModeEventHandler didSelectionModeFinished)
        {
            UndoRedoManager.StartContinuousRecording();
            
            Manager.YuzuMarkerManager.PushMessage(ViewModel, "按住 Ctrl + 鼠标左键选择工作区域，点击画布外侧区域取消，Enter键确认区域");
            var message = "";
            UndoRedoManager.PushRecord(null, value => { }, () => null, value =>
            {
                message = Manager.YuzuMarkerManager.PopMessage(ViewModel);
            }, value =>
            {
                Manager.YuzuMarkerManager.PushMessage(ViewModel, message);
            });

            UndoRedoManager.PushRecord(ViewModel.SelectionMaskUMat, value =>
            {
                ViewModel.SelectionMaskUMat = (UMat)value;
                ViewModel.RefreshImageList();
            }, () => ViewModel.SelectionMaskUMat, disposeAction: value => ((UMat)value).Dispose());
            ViewModel.SelectionMaskUMat.SafeDispose();
            ViewModel.SelectionMaskUMat = ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.Clone();

            ViewModel.LassoPoints = new PointCollection();
            ViewModel.RectangleShapeData = new ShapeData();
            ViewModel.OvalShapeData = new ShapeData();
            ViewModel.SelectionModeEnabled = true;
            
            UndoRedoManager.StopContinuousRecording();
            
            DidSelectionModeFinished = didSelectionModeFinished;
        }

        private void DisableSelectionMode()
        {
            ViewModel.SelectionDrawing = false;
            ViewModel.SelectionModeEnabled = false;
            
            UndoRedoManager.PushRecord(ViewModel.SelectionMaskUMat.Clone(), value =>
            {
                ViewModel.SelectionMaskUMat = (UMat)value;
                ViewModel.RefreshImageList();
            }, () => ViewModel.SelectionMaskUMat, disposeAction: value => ((UMat)value).Dispose());
            ViewModel.SelectionMaskUMat.Dispose();
            
            var message = Manager.YuzuMarkerManager.PopMessage(ViewModel);
            UndoRedoManager.PushRecord(null, value => { }, () => null, value =>
            {
                Manager.YuzuMarkerManager.PushMessage(ViewModel, message);
            }, value =>
            {
                Manager.YuzuMarkerManager.PopMessage(ViewModel);
            });
        }
        #endregion

        #region Selection CheckBox Group Handling
        private void didSelectionModeFinishedForCustomCleaning()
        {
            UndoRedoManager.PushRecord(ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.Clone(), value =>
            {
                ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask = (UMat)value;
                ViewModel.RefreshImageList();
            }, () => ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask, disposeAction: value => ((UMat)value).Dispose());
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.Dispose();
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask = ViewModel.SelectionMaskUMat.Clone();
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType = YuzuCleaningNotationType.Custom;
            ViewModel.RefreshImageList();
        }

        private void CleaningCustomChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom) return;
            EnableSelectionMode(didSelectionModeFinishedForCustomCleaning);
        }

        private void didSelectionModeFinishedForNormalCleaning()
        {
            UndoRedoManager.PushRecord(ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.Clone(), value =>
            {
                ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask = (UMat)value;
                ViewModel.RefreshImageList();
            }, () => ViewModel.SelectedNotationGroupItem?.CleaningNotation.CleaningMask, disposeAction: value => ((UMat)value).Dispose());
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.Dispose();
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask = ViewModel.SelectionMaskUMat.Clone();
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType = YuzuCleaningNotationType.Normal;
            ViewModel.RefreshImageList();
        }
        
        private void CleaningNormalChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Normal) return;
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType = YuzuCleaningNotationType.Normal;
            ViewModel.RefreshImageList();
        }

        private void CleaningNormalSelectionModeButtonClicked(object sender, RoutedEventArgs e)
        {
            EnableSelectionMode(didSelectionModeFinishedForNormalCleaning);
        }
        #endregion
    }
}
