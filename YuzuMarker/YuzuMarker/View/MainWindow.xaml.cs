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
using System.Windows.Interop;
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

        #region Horizontal Scroll of Trackpad
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this);
            ((HwndSource) source)?.AddHook(Hook);
        }
        
        const int WM_MOUSEHWHEEL = 0x020E;

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    var tilt = BitOperation.HIWORD(wParam);
                    OnMouseTilt(tilt);
                    return (IntPtr) 1;
            }
            return IntPtr.Zero;
        }

        private void OnMouseTilt(double tilt)
        {
            if (!(Mouse.DirectlyOver is UIElement element)) return;
            var scrollViewer = element is ScrollViewer viewer ? viewer : FindParent<ScrollViewer>(element);
            scrollViewer?.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            return parentObject switch
            {
                null => null,
                T parent => parent,
                _ => FindParent<T>(parentObject)
            };
        }
        #endregion

        #region Image Button Operation

        private void ImageAreaMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.LabelMode) return;
            if (ViewModel.SelectionModeEnabled) return;
            Point clickPoint = e.GetPosition((IInputElement) sender);

            var lastIgnoreStatus = UndoRedoManager.IgnoreOtherRecording;
            UndoRedoManager.IgnoreOtherRecording = true;
            ViewModel.SelectedImageItem.CreateAndLoadNewNotationGroup((int)clickPoint.X, (int)clickPoint.Y, "", false);
            UndoRedoManager.IgnoreOtherRecording = lastIgnoreStatus;

            UndoRedoManager.PushAndPerformRecord(newGroup =>
            {
                ViewModel.SelectedImageItem.NotationGroups.RemoveAt(ViewModel.SelectedImageItem.NotationGroups.Count - 1);
                return newGroup;
            }, newGroup =>
            {
                if (newGroup == null)
                    newGroup = ViewModel.SelectedImageItem.NotationGroups[^1];
                else
                    ViewModel.SelectedImageItem.NotationGroups.Add(newGroup as YuzuNotationGroup);
                return newGroup;
            });
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
            
            ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask = ViewModel.SelectionMaskUMat.SafeClone();
            DisableSelectionMode();
            
            UndoRedoManager.StopContinuousRecording();
        }
        #endregion

        #region Selection Enable / Finish Wrapping
        private void EnableSelectionMode(object sender, RoutedEventArgs e)
        {
            UndoRedoManager.StartContinuousRecording();

            UndoRedoManager.PushAndPerformRecord(message =>
            {
                message = Manager.YuzuMarkerManager.PopMessage(ViewModel);
                return message;
            }, message =>
            {
                message ??= "按住 Ctrl + 鼠标左键选择工作区域，点击画布外侧区域取消，Enter键确认区域";
                Manager.YuzuMarkerManager.PushMessage(ViewModel, message as string);
                return message;
            });

            ViewModel.SelectionMaskUMat = ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningMask.SafeClone();
            ViewModel.LassoPoints = new PointCollection();
            ViewModel.RectangleShapeData = new ShapeData();
            ViewModel.OvalShapeData = new ShapeData();
            ViewModel.SelectionModeEnabled = true;
            
            UndoRedoManager.StopContinuousRecording();
        }

        private void DisableSelectionMode()
        {
            ViewModel.SelectionDrawing = false;
            ViewModel.SelectionModeEnabled = false;

            UndoRedoManager.PushAndPerformRecord(message =>
            {
                Manager.YuzuMarkerManager.PushMessage(ViewModel, message as string);
                return message;
            }, message =>
            {
                return Manager.YuzuMarkerManager.PopMessage(ViewModel);
            });
        }
        #endregion

        // TODO: refactor: 
        // 1. 保留一个按钮, 做 selection
        // 2. checkbox 处理 type 的事情
        #region Selection CheckBox Group Handling
        private void CleaningInpaintingChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Inpainting) return;
            ViewModel.SelectedNotationGroupItem.CleaningNotation = 
                ViewModel.SelectedNotationGroupItem.CleaningNotation.ConvertTo(YuzuCleaningNotationType.Inpainting);
        }

        private void CleaningColorChecked(object sender, RoutedEventArgs e)
        {
            // Judge whether this is triggered by data binding
            if (ViewModel.SelectedNotationGroupItem.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Color) return;
            ViewModel.SelectedNotationGroupItem.CleaningNotation = 
                ViewModel.SelectedNotationGroupItem.CleaningNotation.ConvertTo(YuzuCleaningNotationType.Color);
        }
        #endregion

        #region Color Picking Section

        private bool _lastIgnoringStatus = false;

        private void ColorPickingStarted(object sender, RoutedEventArgs e)
        {
            UndoRedoManager.StartContinuousRecording();
            UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor;
                ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor = (System.Drawing.Color)o;
                return nowValue;
            }, o =>
            {
                var nowValue = ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor;
                o ??= nowValue;
                ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor = (System.Drawing.Color)o;
                return nowValue;
            });
            _lastIgnoringStatus = UndoRedoManager.IgnoreOtherRecording;
            UndoRedoManager.IgnoreOtherRecording = true;
        }
        
        private void ColorPickingFinished(object sender, RoutedEventArgs e)
        {

            UndoRedoManager.IgnoreOtherRecording = _lastIgnoringStatus;
            UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor;
                ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor = (System.Drawing.Color)o;
                return nowValue;
            }, o =>
            {
                var nowValue = ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor;
                o ??= nowValue;
                ((YuzuColorCleaningNotation)(ViewModel.SelectedNotationGroupItem.CleaningNotation)).CleaningNotationColor = (System.Drawing.Color)o;
                return nowValue;
            });
            UndoRedoManager.StopContinuousRecording();
        }
        #endregion
    }
}
