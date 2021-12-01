using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp;
using YuzuMarker.BasicDataFormat;
using YuzuMarker.Common;
using YuzuMarker.DataFormat;
using YuzuMarker.Model;
using YuzuMarker.Utils;
using Color = System.Drawing.Color;

namespace YuzuMarker.ViewModel
{
    public class YuzuProjectViewModel : NotifyObject
    {
        #region Property: TopMessage
        public string TopMessage => Manager.YuzuMarkerManager.MessageStack[^1];

        #endregion

        #region Property: Project
        private YuzuProject _project;
        
        public YuzuProject Project
        {
            get => _project;
            set => SetProperty(value);
        }
        #endregion

        #region Property: Images
        public ObservableCollection<BasicYuzuImage> Images => Project?.Images;
        #endregion

        #region Property: ImageSource
        public ImageSource ImageSource
        {
            get
            {
                if (SelectedImageItem == null) return null;
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(SelectedImageItem.GetImageFilePath());
                    bitmap.EndInit();
                    return bitmap;
                }
                catch (Exception e)
                {
                    Utils.ExceptionHandler.ShowExceptionMessage(e);
                }
                return null;
            }
        }
        #endregion

        #region Property: SelectedImageItem
        private YuzuImage _selectedImageItem;
        
        public YuzuImage SelectedImageItem
        {
            get => _selectedImageItem;
            set
            {
                UndoRedoManager.StopRecording();
                if (_selectedImageItem == null)
                {
                    SetProperty(value);
                    goto EndSection;
                }
                _selectedImageItem.UnloadImageNotations();
                SetProperty(Project == null ? null : value);
            EndSection:
                if (value != null)
                {
                    value.LoadImageNotations();
                    UndoRedoManager.Clear();
                    UndoRedoManager.StartRecording();
                }
                RaisePropertyChanged("ImageSource");
                RaisePropertyChanged("NotationGroups");
                RaisePropertyChanged("SelectedNotationGroupItem");
            }
        }
        #endregion

        #region Property: NotationGroups
        public ObservableCollection<BasicYuzuNotationGroup> NotationGroups 
            => SelectedImageItem?.NotationGroups;
        #endregion

        #region Property: SelectedNotationGroupItem
        private YuzuNotationGroup _selectedNotationGroupItem;
        
        [Undoable]
        public YuzuNotationGroup SelectedNotationGroupItem
        {
            get => _selectedNotationGroupItem;
            set => SetProperty(value);
        }
        #endregion

        #region Property: LabelMode
        private bool _labelMode = false;

        [Undoable]
        public bool LabelMode
        {
            get => _labelMode;
            set => SetProperty(value);
        }
        #endregion

        #region Property: SelectionModeEnabled
        private bool _selectionModeEnabled = false;

        [Undoable]
        public bool SelectionModeEnabled
        {
            get => _selectionModeEnabled;
            set => SetProperty(value);
        }
        #endregion
        
        #region Property: SelectionDrawing
        private bool _selectionDrawing = false;
        
        public bool SelectionDrawing
        {
            get => _selectionDrawing;
            set
            {
                if (_selectionDrawing == value) return;
                _selectionDrawing = value;
                if (!value)
                {
                    try
                    {
                        switch (SelectionType)
                        {
                            case SelectionType.Lasso:
                                if (LassoPoints.Count < 3)
                                    throw new Exception("套索未选择工作区域，操作失败");
                                break;
                            case SelectionType.Rectangle:
                                if (RectangleShapeData.Width == 0 || RectangleShapeData.Height == 0)
                                    throw new Exception("未框选工作区域，操作失败");
                                break;
                            case SelectionType.Oval:
                                if (OvalShapeData.Width == 0 || OvalShapeData.Height == 0)
                                    throw new Exception("未圈选工作区域，操作失败");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.ExceptionHandler.ShowExceptionMessage(ex);
                    }

                    var newUMat = UMat.Zeros(SelectionMaskUMat.Rows, SelectionMaskUMat.Cols, MatType.CV_8UC1);

                    switch (SelectionType)
                    {
                        case SelectionType.Lasso:
                            // Photoshop style: FillConvexPoly
                            Cv2.FillPoly(newUMat, InputArray.Create(LassoPoints.ToOpenCvPoint()), new Scalar(255));
                            break;
                        case SelectionType.Rectangle:
                            if (RectangleShapeData.Width < 0)
                            {
                                RectangleShapeData.Width *= -1;
                                RectangleShapeData.X -= RectangleShapeData.Width;
                            }
                            if (RectangleShapeData.Height < 0)
                            {
                                RectangleShapeData.Height *= -1;
                                RectangleShapeData.Y -= RectangleShapeData.Height;
                            }
                            Cv2.Rectangle(newUMat, new OpenCvSharp.Point(RectangleShapeData.X, RectangleShapeData.Y), 
                            new OpenCvSharp.Point(RectangleShapeData.X + RectangleShapeData.Width, RectangleShapeData.Y + RectangleShapeData.Height), 
                            new Scalar(255), -1);
                            break;
                        case SelectionType.Oval:
                            if (OvalShapeData.Width < 0)
                            {
                                OvalShapeData.Width *= -1;
                                OvalShapeData.X -= OvalShapeData.Width;
                            }
                            if (OvalShapeData.Height < 0)
                            {
                                OvalShapeData.Height *= -1;
                                OvalShapeData.Y -= OvalShapeData.Height;
                            }
                            Cv2.Ellipse(newUMat, new RotatedRect(
                                new Point2f((float)(OvalShapeData.X + OvalShapeData.Width / 2.0), (float)(OvalShapeData.Y + OvalShapeData.Height / 2.0)), 
                                    new Size2f(OvalShapeData.Width, OvalShapeData.Height), 0), new Scalar(255), -1);
                            break;
                    }

                    var result = UMat.Zeros(SelectionMaskUMat.Rows, SelectionMaskUMat.Cols, MatType.CV_8UC1);
                    switch (SelectionMode)
                    {
                        case SelectionMode.New:
                            result = newUMat;
                            break;
                        case SelectionMode.Add:
                            Cv2.BitwiseOr(SelectionMaskUMat, newUMat, result);
                            newUMat.SafeDispose();
                            break;
                        case SelectionMode.Subtract:
                            Cv2.BitwiseNot(newUMat, newUMat);
                            Cv2.BitwiseAnd(SelectionMaskUMat, newUMat, result);
                            newUMat.SafeDispose();
                            break;
                        case SelectionMode.Intersect:
                            Cv2.BitwiseAnd(SelectionMaskUMat, newUMat, result);
                            newUMat.SafeDispose();
                            break;
                    }
                    SelectionMaskUMat = result;

                    LassoPoints = new PointCollection();
                    RectangleShapeData = new ShapeData();
                    OvalShapeData = new ShapeData();
                }
                RaisePropertyChanged("SelectionDrawing");
            }
        }
        #endregion

        #region Property: SelectionType
        private SelectionType _selectionType = SelectionType.Lasso;

        [Undoable]
        public SelectionType SelectionType
        {
            get => _selectionType;
            set => SetProperty(value);
        }
        #endregion

        #region Property: SelectionMode
        private SelectionMode _selectionMode = SelectionMode.New;

        [Undoable]
        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(value);
        }
        #endregion
        
        #region Property: LassoPoints (Used for Lasso Polygon)
        private PointCollection _lassoPoints = new PointCollection();

        public PointCollection LassoPoints
        {
            get => _lassoPoints;
            set => SetProperty(value);
        }
        #endregion

        #region Property: RectangleShapeData (Used for Lasso Rectangle)
        private ShapeData _rectangleShapeData = new ShapeData();
        
        public ShapeData RectangleShapeData
        {
            get => _rectangleShapeData;
            set => SetProperty(value);
        }
        #endregion

        #region Property: OvalShapeData (Used for Lasso Oval)
        private ShapeData _ovalShapeData = new ShapeData();
        
        public ShapeData OvalShapeData
        {
            get => _ovalShapeData;
            set => SetProperty(value);
        }
        #endregion

        #region Property: SelectionMaskUMat
        private UMat _selectionMaskUMat;

        [Undoable]
        public UMat SelectionMaskUMat
        {
            get => _selectionMaskUMat;
            set => SetProperty(value, disposeAction: o => ((UMat)o).SafeDispose());
        }
        #endregion

        #region Property: PeakColor
        private System.Drawing.Color _peakColor = Color.White;
        
        [Undoable]
        public System.Drawing.Color PeakColor
        {
            get => _peakColor;
            set => SetProperty(value);
        }
        #endregion
        
        #region Command: DetectColor
        private DelegateCommand _detectMaxColor;

        public DelegateCommand DetectMaxColor
        {
            get
            {
                _detectMaxColor ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        try
                        {
                            var tempFileName = Path.GetTempFileName() + ".png";
                            var writeMat =
                                SelectedNotationGroupItem.CleaningNotation.CleaningMask.GetMat(AccessFlag.READ);
                            Cv2.ImWrite(tempFileName, writeMat);
                            writeMat.Dispose();
                            ((YuzuColorCleaningNotation)SelectedNotationGroupItem.CleaningNotation).CleaningNotationColor =
                                IPC.Invoker.DetectMaxColor(SelectedImageItem.GetImageFilePath(), tempFileName);
                        }
                        catch (Exception e)
                        {
                            Utils.ExceptionHandler.ShowExceptionMessage(e);
                        }
                    }
                };
                return _detectMaxColor;
            }
        }
        
        private DelegateCommand _detectPeakColor;

        public DelegateCommand DetectPeakColor
        {
            get
            {
                _detectPeakColor ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        try
                        {
                            var tempFileName = Path.GetTempFileName() + ".png";
                            var writeMat =
                                SelectedNotationGroupItem.CleaningNotation.CleaningMask.GetMat(AccessFlag.READ);
                            Cv2.ImWrite(tempFileName, writeMat);
                            writeMat.Dispose();
                            ((YuzuColorCleaningNotation)SelectedNotationGroupItem.CleaningNotation).CleaningNotationColor =
                                IPC.Invoker.DetectPeakColor(SelectedImageItem.GetImageFilePath(), tempFileName, 
                                    preferredR: PeakColor.R, preferredG: PeakColor.G, preferredB: PeakColor.B);
                        }
                        catch (Exception e)
                        {
                            Utils.ExceptionHandler.ShowExceptionMessage(e);
                        }
                    }
                };
                return _detectPeakColor;
            }
        }
        #endregion

        #region Commands: Modify Selection Area
        private DelegateCommand _zoomInSelectionArea;

        public DelegateCommand ZoomInSelectionArea
        {
            get
            {
                _zoomInSelectionArea ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        if (SelectedNotationGroupItem?.CleaningNotation?.CleaningMask == null) return;
                        var newUMat = SelectedNotationGroupItem.CleaningNotation.CleaningMask.SafeClone();
                        Cv2.Dilate(newUMat, newUMat, new UMat());
                        SelectedNotationGroupItem.CleaningNotation.CleaningMask = newUMat;
                    }
                };
                return _zoomInSelectionArea;
            }
        }
        
        private DelegateCommand _zoomOutSelectionArea;

        public DelegateCommand ZoomOutSelectionArea
        {
            get
            {
                _zoomOutSelectionArea ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        if (SelectedNotationGroupItem?.CleaningNotation?.CleaningMask == null) return;
                        var newUMat = SelectedNotationGroupItem.CleaningNotation.CleaningMask.SafeClone();
                        Cv2.Erode(newUMat, newUMat, new UMat());
                        SelectedNotationGroupItem.CleaningNotation.CleaningMask = newUMat;
                    }
                };
                return _zoomOutSelectionArea;
            }
        }
        
        private DelegateCommand _clearSelectionArea;

        public DelegateCommand ClearSelectionArea
        {
            get
            {
                _clearSelectionArea ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        if (SelectedNotationGroupItem?.CleaningNotation?.CleaningMask == null) return;
                        if (SelectedNotationGroupItem.CleaningNotation.CleaningMask.IsEmpty()) return;
                        SelectedNotationGroupItem.CleaningNotation.CleaningMask = 
                            UMat.Zeros(SelectedNotationGroupItem.CleaningNotation.CleaningMask.Rows, 
                                SelectedNotationGroupItem.CleaningNotation.CleaningMask.Cols, MatType.CV_8UC1);;
                    }
                };
                return _clearSelectionArea;
            }
        }
        
        private DelegateCommand _checkAllSelectionArea;

        public DelegateCommand CheckAllSelectionArea
        {
            get
            {
                _checkAllSelectionArea ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        if (SelectedNotationGroupItem?.CleaningNotation?.CleaningMask == null) return;
                        SelectedNotationGroupItem.CleaningNotation.CleaningMask = 
                            new UMat(SelectedNotationGroupItem.CleaningNotation.CleaningMask.Rows, 
                                SelectedNotationGroupItem.CleaningNotation.CleaningMask.Cols, MatType.CV_8UC1, new Scalar(255));
                    }
                };
                return _checkAllSelectionArea;
            }
        }
        
        private DelegateCommand _inverseSelectionArea;

        public DelegateCommand InverseSelectionArea
        {
            get
            {
                _inverseSelectionArea ??= new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        if (SelectedNotationGroupItem?.CleaningNotation?.CleaningMask == null) return;
                        var newUMat = SelectedNotationGroupItem.CleaningNotation.CleaningMask.SafeClone();
                        Cv2.BitwiseNot(newUMat, newUMat);
                        SelectedNotationGroupItem.CleaningNotation.CleaningMask = newUMat;
                    }
                };
                return _inverseSelectionArea;
            }
        }
        #endregion

        #region Command: Export Custom Cleaning Mask to Photoshop

        private DelegateCommand _ExportCustomCleaningMaskToPhotoshop;

        public DelegateCommand ExportCustomCleaningMaskToPhotoshop
        {
            get
            {
                if (_ExportCustomCleaningMaskToPhotoshop == null)
                    _ExportCustomCleaningMaskToPhotoshop = new DelegateCommand()
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                PSBridge.CommonWrapper.OpenAndInitPSDFileStructureIfNotExist(
                                    SelectedImageItem.GetImageFilePath(), SelectedImageItem.GetImagePsdPath());
                                
                                // TODO: Copy Layer to specific path
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _ExportCustomCleaningMaskToPhotoshop;
            }
        }

        #endregion

        #region Command: Select NotationGroup
        private DelegateCommand<int> _SelectNotationGroup;

        public DelegateCommand<int> SelectNotationGroup
        {
            get
            {
                if (_SelectNotationGroup == null)
                    _SelectNotationGroup = new DelegateCommand<int>()
                    {
                        CommandAction = (index) =>
                        {
                            try
                            {
                                SelectedNotationGroupItem = NotationGroups[index - 1] as YuzuNotationGroup;
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _SelectNotationGroup;
            }
        }
        #endregion

        #region Command: Delete NotationGroup
        private DelegateCommand _DeleteNotationGroup;

        public DelegateCommand DeleteNotationGroup
        {
            get
            {
                if (_DeleteNotationGroup == null)
                    _DeleteNotationGroup = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                UndoRedoManager.PushAndPerformRecord(o =>
                                {
                                    var removedItem = ((List<object>)o)[0] as YuzuNotationGroup;
                                    var removedIndex = ((List<object>)o)[1] as int? ?? 0;
                                    NotationGroups.Insert(removedIndex, removedItem);
                                    RaisePropertyChanged("NotationGroups");
                                    return o;
                                }, o =>
                                {
                                    o ??= new List<object>
                                    {
                                        SelectedNotationGroupItem,
                                        NotationGroups.IndexOf(SelectedNotationGroupItem)
                                    };
                                    var removedItem = ((List<object>)o)[0] as YuzuNotationGroup;
                                    NotationGroups.Remove(removedItem);
                                    RaisePropertyChanged("NotationGroups");
                                    return o;
                                }, o =>
                                {
                                    var removedItem = ((List<object>)o)[0] as YuzuNotationGroup;
                                    removedItem?.Dispose();
                                });
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _DeleteNotationGroup;
            }
        }
        #endregion

        #region Command: Move NotationGroup up
        private DelegateCommand _MoveNotationGroupUp;

        public DelegateCommand MoveNotationGroupUp
        {
            get
            {
                if (_MoveNotationGroupUp == null)
                    _MoveNotationGroupUp = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            var index = NotationGroups.IndexOf(SelectedNotationGroupItem);
                            if (index > 0)
                                UndoRedoManager.PushAndPerformRecord(o =>
                                {
                                    NotationGroups.Move(index - 1, index);
                                    RaisePropertyChanged("NotationGroups");
                                    return null;
                                }, o =>
                                {
                                    NotationGroups.Move(index, index - 1);
                                    RaisePropertyChanged("NotationGroups");
                                    return null;
                                });
                        }
                    };
                return _MoveNotationGroupUp;
            }
        }
        #endregion

        #region Command: Move NotationGroup Down
        private DelegateCommand _MoveNotationGroupDown;

        public DelegateCommand MoveNotationGroupDown
        {
            get
            {
                if (_MoveNotationGroupDown == null)
                    _MoveNotationGroupDown = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            var index = NotationGroups.IndexOf(SelectedNotationGroupItem);
                            if (index < NotationGroups.Count - 1)
                                UndoRedoManager.PushAndPerformRecord(o =>
                                {
                                    NotationGroups.Move(index + 1, index);
                                    RaisePropertyChanged("NotationGroups");
                                    return null;
                                }, o =>
                                {
                                    NotationGroups.Move(index, index + 1);
                                    RaisePropertyChanged("NotationGroups");
                                    return null;
                                });
                        }
                    };
                return _MoveNotationGroupDown;
            }
        }
        #endregion

        #region Command: Add Image Command
        private DelegateCommand _AddImages;

        public DelegateCommand AddImages
        {
            get
            {
                if (_AddImages == null)
                    _AddImages = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                if (MessageBox.Show("工程文件将被自动保存。确定继续？", "导入图片", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    SaveProject.CommandAction.Invoke();
                                    OpenFileDialog openFileDialog = new OpenFileDialog
                                    {
                                        Filter = "(*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg",
                                        Multiselect = true,
                                        Title = "选择打开的项目",
                                        CheckFileExists = true
                                    };
                                    if (openFileDialog.ShowDialog() == true)
                                    {
                                        foreach (string filePath in openFileDialog.FileNames)
                                        {
                                            Project.CreateNewImage(filePath);
                                        }
                                    }
                                    SaveProject.CommandAction.Invoke();
                                }
                            } catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _AddImages;
            }
        }
        #endregion

        #region Command: Delete Image
        private DelegateCommand _DeleteImage;

        public DelegateCommand DeleteImage
        {
            get
            {
                if (_DeleteImage == null)
                    _DeleteImage = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                if (MessageBox.Show("此操作不可撤回，并且工程文件将被自动保存。确定继续？", "删除图片", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    SaveProject.CommandAction.Invoke();
                                    Project.RemoveImage(SelectedImageItem);
                                    RaisePropertyChanged("Project");
                                    RaisePropertyChanged("Images");
                                    SaveProject.CommandAction.Invoke();
                                }
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _DeleteImage;
            }
        }
        #endregion

        #region Command: Set Image Finish Status
        private DelegateCommand _SetImageFinishStatus;

        public DelegateCommand SetImageFinishStatus
        {
            get
            {
                if (_SetImageFinishStatus == null)
                    _SetImageFinishStatus = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            SelectedImageItem.IsFinished = !SelectedImageItem.IsFinished;
                        }
                    };
                return _SetImageFinishStatus;
            }
        }
        #endregion

        #region Command: Move image up
        private DelegateCommand _MoveSelectedImageUp;

        public DelegateCommand MoveSelectedImageUp
        {
            get
            {
                if (_MoveSelectedImageUp == null)
                    _MoveSelectedImageUp = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            int index = Project.Images.IndexOf(SelectedImageItem);
                            if (index > 0)
                            {
                                Project.Images.Move(index, index - 1);
                                RaisePropertyChanged("Images");
                            }
                        }
                    };
                return _MoveSelectedImageUp;
            }
        }
        #endregion

        #region Command: Move image down
        private DelegateCommand _MoveSelectedImageDown;

        public DelegateCommand MoveSelectedImageDown
        {
            get
            {
                if (_MoveSelectedImageDown == null)
                    _MoveSelectedImageDown = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            int index = Project.Images.IndexOf(SelectedImageItem);
                            if (index < Project.Images.Count - 1)
                            {
                                Project.Images.Move(index, index + 1);
                                RaisePropertyChanged("Images");
                            }
                        }
                    };
                return _MoveSelectedImageDown;
            }
        }
        #endregion

        #region Command: Load Project
        private DelegateCommand _LoadProject;

        public DelegateCommand LoadProject
        {
            get
            {
                if (_LoadProject == null)
                    _LoadProject = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                OpenFileDialog openFileDialog = new OpenFileDialog
                                {
                                    Filter = "(*.yuzu)|*.yuzu",
                                    Multiselect = false,
                                    Title = "选择打开的项目",
                                    CheckFileExists = true
                                };
                                if (openFileDialog.ShowDialog() == true)
                                {
                                    Project = YuzuIO.LoadProject(openFileDialog.FileNames[0]);
                                    RaisePropertyChanged("Project");
                                    RaisePropertyChanged("Images");
                                    RaisePropertyChanged("ImageSource");
                                    SelectedImageItem = null;
                                }
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _LoadProject;
            }
        }
        #endregion

        #region Command: Save Project
        private DelegateCommand _SaveProject;

        public DelegateCommand SaveProject
        {
            get
            {
                if (_SaveProject == null)
                    _SaveProject = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                SelectedImageItem?.WriteImageNotations();
                                YuzuIO.SaveProject(Project);
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _SaveProject;
            }
        }
        #endregion

        #region Command: Create Project
        private DelegateCommand _CreateProject;

        public DelegateCommand CreateProject
        {
            get
            {
                if (_CreateProject == null)
                    _CreateProject = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                var window = new View.CreateProjectWindow();
                                window.CreateProjectEvent += CreateProjectHandler;
                                window.Show();
                            }
                            catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return _CreateProject;
            }
        }

        private void CreateProjectHandler(object sender, string fileName, string projectName, string path)
        {
            try
            {
                Project = YuzuIO.CreateProject(path, fileName, projectName);
                RaisePropertyChanged("Images");
                RaisePropertyChanged("ImageSource");
                SelectedImageItem = null;
            }
            catch (Exception e)
            {
                Utils.ExceptionHandler.ShowExceptionMessage(e);
            }
        }
        #endregion
        
        #region Command: Undo Step
        private DelegateCommand _undoStep;

        public DelegateCommand UndoStep
        {
            get
            {
                return _undoStep ??= new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        try
                        {
                            UndoRedoManager.Undo();
                        }
                        catch (Exception e)
                        {
                            Utils.ExceptionHandler.ShowExceptionMessage(e);
                        }
                    }
                };
            }
        }
        #endregion
        
        #region Command: Redo Step
        private DelegateCommand _redoStep;

        public DelegateCommand RedoStep
        {
            get
            {
                return _redoStep ??= new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        try
                        {
                            UndoRedoManager.Redo();
                        }
                        catch (Exception e)
                        {
                            Utils.ExceptionHandler.ShowExceptionMessage(e);
                        }
                    }
                };
            }
        }
        #endregion
    }
}
