using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using YuzuMarker.Common;
using YuzuMarker.DataFormat;

namespace YuzuMarker.ViewModel
{
    public class YuzuProjectViewModel : NotifyObject
    {
        #region Property: TopMessage
        public string TopMessage
        {
            get
            {
                return Manager.YuzuMarkerManager.MessageStack[^1];
            }
        }
        #endregion

        #region Property: Project
        public YuzuProject<ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>,
            ObservableCollection<YuzuNotationGroup>> Project
        {
            get
            {
                return Manager.YuzuMarkerManager.Project;
            }
        }
        #endregion

        #region Property: Images
        public ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>> Images
        {
            get
            {
                if (Manager.YuzuMarkerManager.Project == null) return null;
                return Manager.YuzuMarkerManager.Project.Images;
            }
        }
        #endregion

        #region Property: ImageSource
        public ImageSource ImageSource
        {
            get
            {
                if (Manager.YuzuMarkerManager.Image == null) return null;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(Manager.YuzuMarkerManager.Image.GetImageFilePath());
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
        public YuzuImage<ObservableCollection<YuzuNotationGroup>> SelectedImageItem
        {
            get
            {
                return Manager.YuzuMarkerManager.Image;
            }
            set
            {
                if (Manager.YuzuMarkerManager.Project == null)
                    Manager.YuzuMarkerManager.Image = null;
                else
                    Manager.YuzuMarkerManager.Image = value;
                RaisePropertyChanged("ImageSource");
                RaisePropertyChanged("SelectedImageItem");
                RaisePropertyChanged("NotationGroups");
                RaisePropertyChanged("SelectedNotationGroupItem");
            }
        }
        #endregion

        #region Property: NotationGroups
        public ObservableCollection<YuzuNotationGroup> NotationGroups
        {
            get
            {
                if (SelectedImageItem == null) return null;
                return SelectedImageItem.NotationGroups;
            }
        }
        #endregion

        #region Property: SelectedNotationGroupItem
        public YuzuNotationGroup SelectedNotationGroupItem
        {
            get
            {
                return Manager.YuzuMarkerManager.Group;
            }
            set
            {
                Manager.YuzuMarkerManager.Group = value;
                RaisePropertyChanged("SelectedNotationGroupItem");
                RaisePropertyChanged("SelectedNotationGroupText");
            }
        }
        #endregion

        #region Property: LabelMode
        private bool _LabelMode = false;

        public bool LabelMode
        {
            get
            {
                return _LabelMode;
            }
            set
            {
                _LabelMode = value;
                RaisePropertyChanged("LabelMode");
            }
        }
        #endregion

        #region Property: LassoMode
        private bool _LassoMode = false;

        public bool LassoMode
        {
            get
            {
                return _LassoMode;
            }
            set
            {
                _LassoMode = value;
                RaisePropertyChanged("LassoMode");
            }
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

        #region Property: SelectedNotationGroupText
        public string SelectedNotationGroupText
        {
            get
            {
                if (SelectedNotationGroupItem == null) return null;
                return SelectedNotationGroupItem.text;
            }
            set
            {
                SelectedNotationGroupItem.text = value;
                RaisePropertyChanged("SelectedNotationGroupText");
            }
        }
        #endregion

        #region Command: Export Custom Cleaning Mask to Phot

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
                                PSBridge.CommonWrapper.OpenAndGeneratePSDIfNotExist(
                                    SelectedImageItem.GetImageFilePath(), SelectedImageItem.GetImagePSDPath());
                                // TODO: Finish Exportation
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
                                SelectedNotationGroupItem = NotationGroups[index - 1];
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
                                NotationGroups.Remove(SelectedNotationGroupItem);
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
                                    YuzuIO.SaveProject(Project);
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
                                            Manager.YuzuMarkerManager.Project.CreateNewImage(filePath);
                                        }
                                    }
                                    YuzuIO.SaveProject(Project);
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
                                    YuzuIO.SaveProject(Project);
                                    Project.RemoveImage(SelectedImageItem);
                                    RaisePropertyChanged("Project");
                                    RaisePropertyChanged("Images");
                                    YuzuIO.SaveProject(Project);
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
                            RefreshImageList();
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
                                    Manager.YuzuMarkerManager.Project = YuzuIO.LoadProject
                                        <ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>,
                                        ObservableCollection<YuzuNotationGroup>>
                                        (openFileDialog.FileNames[0]);
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
                                YuzuIO.SaveProject(Manager.YuzuMarkerManager.Project);
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
                                View.CreateProjectWindow window = new View.CreateProjectWindow();
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
                Manager.YuzuMarkerManager.Project = YuzuIO.CreateProject
                    <ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>,
                    ObservableCollection<YuzuNotationGroup>>
                    (path, fileName, projectName);
                RaisePropertyChanged("Project");
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

        #region Refresh children attributes (which are not notify objects)
        public void RefreshImageList()
        {
            // Backup properties
            var project = Project;
            var selectedImageItem = SelectedImageItem;
            var selectedNotationGroupItem = SelectedNotationGroupItem;
            // set certain fields to null to reset value in UI
            Manager.YuzuMarkerManager.Project = null;
            SelectedImageItem = null;
            // clear properties manually
            RaisePropertyChanged("Images");
            RaisePropertyChanged("NotationGroups");
            // set back properties
            Manager.YuzuMarkerManager.Project = project;
            SelectedImageItem = selectedImageItem;
            SelectedNotationGroupItem = selectedNotationGroupItem;
            // refresh properties again
            RaisePropertyChanged("Images");
            RaisePropertyChanged("NotationGroups");
        }
        #endregion
    }
}
