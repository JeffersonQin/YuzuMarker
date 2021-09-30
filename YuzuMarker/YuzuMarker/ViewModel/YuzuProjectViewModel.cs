using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using YuzuMarker.Files;

namespace YuzuMarker.ViewModel
{
    public class YuzuProjectViewModel : NotifyObject
    {
        public List<YuzuImage> Images
        {
            get
            {
                if (Manager.YuzuMarkerManager.Project == null) return null;
                return Manager.YuzuMarkerManager.Project.Images;
            }
        }

        private DelegateCommand<string> _AddImage;

        public DelegateCommand<string> AddImage
        {
            get
            {
                if (_AddImage == null)
                    _AddImage = new DelegateCommand<string>
                    {
                        CommandAction = (path) =>
                        {
                            try
                            {
                                Manager.YuzuMarkerManager.Project.AddImage(path);
                                RaisePropertyChanged("Images");
                            } catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        },
                        CanExecuteFunc = (path) => File.Exists(path)
                    };
                return _AddImage;
            }
        }

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
                                    Manager.YuzuMarkerManager.Project = YuzuIO.LoadProject(openFileDialog.FileNames[0]);
                                    RaisePropertyChanged("Images");
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
                Manager.YuzuMarkerManager.Project = YuzuIO.CreateProject(path, fileName, projectName);
                RaisePropertyChanged("Images");
            }
            catch (Exception e)
            {
                Utils.ExceptionHandler.ShowExceptionMessage(e);
            }
        }
    }
}
