﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using YuzuMarker.Files;

namespace YuzuMarker.ViewModel
{
    public class YuzuProjectViewModel : NotifyObject
    {
        public YuzuProject<ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>,
            ObservableCollection<YuzuNotationGroup>> Project
        {
            get
            {
                return Manager.YuzuMarkerManager.Project;
            }
        }

        public ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>> Images
        {
            get
            {
                if (Manager.YuzuMarkerManager.Project == null) return null;
                return Manager.YuzuMarkerManager.Project.Images;
            }
        }

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

        private YuzuImage<ObservableCollection<YuzuNotationGroup>> selectedItem = null;

        public YuzuImage<ObservableCollection<YuzuNotationGroup>> SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                if (Manager.YuzuMarkerManager.Project == null)
                    Manager.YuzuMarkerManager.Image = null;
                else
                    Manager.YuzuMarkerManager.Image = selectedItem;
                RaisePropertyChanged("ImageSource");
                RaisePropertyChanged("SelectedItem");
            }
        }

        #region Add Image Command
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

        #region Load Project Command
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
                                    SelectedItem = null;
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

        #region Save Project Command
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

        #region Create Project Command
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
                SelectedItem = null;
            }
            catch (Exception e)
            {
                Utils.ExceptionHandler.ShowExceptionMessage(e);
            }
        }
        #endregion
    }
}
