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
        private YuzuImage<ObservableCollection<YuzuNotationGroup>> _SelectedImageItem = null;

        public YuzuImage<ObservableCollection<YuzuNotationGroup>> SelectedImageItem
        {
            get
            {
                return _SelectedImageItem;
            }
            set
            {
                _SelectedImageItem = value;
                if (Manager.YuzuMarkerManager.Project == null)
                    Manager.YuzuMarkerManager.Image = null;
                else
                    Manager.YuzuMarkerManager.Image = _SelectedImageItem;
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
        private YuzuNotationGroup _SelectedNotationGroupItem = null;

        public YuzuNotationGroup SelectedNotationGroupItem
        {
            get
            {
                return _SelectedNotationGroupItem;
            }
            set
            {
                _SelectedNotationGroupItem = value;
                RaisePropertyChanged("SelectedNotationGroupItem");
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
            // set project to null
            Manager.YuzuMarkerManager.Project = null;
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
