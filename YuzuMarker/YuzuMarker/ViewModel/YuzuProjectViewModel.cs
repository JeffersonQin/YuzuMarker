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
            set => SetProperty(ref _project, value);
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
                if (_selectedImageItem == null)
                {
                    SetProperty(ref _selectedImageItem, value);
                    goto EndSection;
                }
                if (!_refreshingImageList)
                    _selectedImageItem.UnloadImageNotations();
                SetProperty(ref _selectedImageItem, Project == null ? null : value);
            EndSection:
                if (!_refreshingImageList)
                    value?.LoadImageNotations();
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
        
        public YuzuNotationGroup SelectedNotationGroupItem
        {
            get => _selectedNotationGroupItem;
            set
            {
                SetProperty(ref _selectedNotationGroupItem, value);
                RaisePropertyChanged("SelectedNotationGroupText");
            }
        }
        #endregion

        #region Property: LabelMode
        private bool _labelMode = false;

        public bool LabelMode
        {
            get => _labelMode;
            set => SetProperty(ref _labelMode, value);
        }
        #endregion

        #region Property: SelectionModeEnabled
        private bool _selectionModeEnabled = false;

        public bool SelectionModeEnabled
        {
            get
            {
                return _selectionModeEnabled;
            }
            set
            {
                _selectionModeEnabled = value;
                RaisePropertyChanged("SelectionModeEnabled");
            }
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
                    
                    switch (SelectionMode)
                    {
                        case SelectionMode.New:
                            SelectionMaskUMat.Dispose();
                            SelectionMaskUMat = newUMat;
                            break;
                        case SelectionMode.Add:
                            Cv2.BitwiseOr(SelectionMaskUMat, newUMat, SelectionMaskUMat);
                            newUMat.Dispose();
                            break;
                        case SelectionMode.Subtract:
                            Cv2.BitwiseNot(newUMat, newUMat);
                            Cv2.BitwiseAnd(SelectionMaskUMat, newUMat, SelectionMaskUMat);
                            newUMat.Dispose();
                            break;
                        case SelectionMode.Intersect:
                            Cv2.BitwiseAnd(SelectionMaskUMat, newUMat, SelectionMaskUMat);
                            newUMat.Dispose();
                            break;
                    }
                    
                    RaisePropertyChanged("SelectionMaskUMat");
                    
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

        public SelectionType SelectionType
        {
            get => _selectionType;
            set => SetProperty(ref _selectionType, value);
        }
        #endregion

        #region Property: SelectionMode
        private SelectionMode _selectionMode = SelectionMode.New;

        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }
        #endregion
        
        #region Property: LassoPoints (Used for Lasso Polygon)
        private PointCollection _LassoPoints = new PointCollection();

        public PointCollection LassoPoints
        {
            get
            {
                return _LassoPoints;
            }
            set
            {
                _LassoPoints = value;
                RaisePropertyChanged("LassoPoints");
            }
        }
        #endregion

        #region Property: RectangleShapeData (Used for Lasso Rectangle)
        private ShapeData _rectangleShapeData = new ShapeData();
        
        public ShapeData RectangleShapeData
        {
            get => _rectangleShapeData;
            set => SetProperty(ref _rectangleShapeData, value);
        }
        #endregion

        #region Property: OvalShapeData (Used for Lasso Oval)
        private ShapeData _ovalShapeData = new ShapeData();
        
        public ShapeData OvalShapeData
        {
            get => _ovalShapeData;
            set => SetProperty(ref _ovalShapeData, value);
        }
        #endregion

        #region Property: SelectionMaskUMat
        private UMat _selectionMaskUMat;

        public UMat SelectionMaskUMat
        {
            get => _selectionMaskUMat;
            set
            {
                _selectionMaskUMat = value;
                RaisePropertyChanged("SelectionMaskUMat");
            }
        }
        #endregion

        #region Property: SelectedNotationGroupText
        public string SelectedNotationGroupText
        {
            get
            {
                if (SelectedNotationGroupItem == null) return null;
                return SelectedNotationGroupItem.Text;
            }
            set
            {
                SelectedNotationGroupItem.Text = value;
                RaisePropertyChanged("SelectedNotationGroupText");
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
                                SelectedImageItem.RemoveAndUnloadNotationGroup(SelectedNotationGroupItem);
                                RefreshImageList();
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
                            int index = NotationGroups.IndexOf(SelectedNotationGroupItem);
                            if (index > 0)
                            {
                                NotationGroups.Move(index, index - 1);
                                RefreshImageList();
                            }
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
                            int index = NotationGroups.IndexOf(SelectedNotationGroupItem);
                            if (index < NotationGroups.Count - 1)
                            {
                                NotationGroups.Move(index, index + 1);
                                RefreshImageList();
                            }
                        }
                    };
                return _MoveNotationGroupDown;
            }
        }
        #endregion

        #region Command: Set NotationGroup Finish Status
        private DelegateCommand _SetNotationGroupFinishStatus;

        public DelegateCommand SetNotationGroupFinishStatus
        {
            get
            {
                if (_SetNotationGroupFinishStatus == null)
                    _SetNotationGroupFinishStatus = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            SelectedNotationGroupItem.IsFinished = !SelectedNotationGroupItem.IsFinished;
                            RefreshImageList();
                        }
                    };
                return _SetNotationGroupFinishStatus;
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
                                SelectedImageItem.WriteImageNotations();
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

        #region Refresh children attributes (which are not notify objects), also used for refreshing notationGroup, because it is converted as a whole
        private bool _refreshingImageList = false;
        
        public void RefreshImageList()
        {
            _refreshingImageList = true;
            // Backup properties
            var project = Project;
            var selectedImageItem = SelectedImageItem;
            var selectedNotationGroupItem = SelectedNotationGroupItem;
            // set certain fields to null to reset value in UI
            Project = null;
            SelectedImageItem = null;
            // clear properties manually
            RaisePropertyChanged("Images");
            RaisePropertyChanged("NotationGroups");
            // set back properties
            Project = project;
            SelectedImageItem = selectedImageItem;
            SelectedNotationGroupItem = selectedNotationGroupItem;
            // refresh properties again
            RaisePropertyChanged("Images");
            RaisePropertyChanged("NotationGroups");
            _refreshingImageList = false;
        }
        #endregion
    }
}
