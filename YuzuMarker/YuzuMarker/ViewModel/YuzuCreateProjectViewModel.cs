using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.ViewModel
{
    public class YuzuCreateProjectViewModel : NotifyObject
    {
        private string fileName;

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        private string projectName;

        public string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
                RaisePropertyChanged("ProjectName");
            }
        }

        private string path;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                RaisePropertyChanged("Path");
            }
        }

        private DelegateCommand chooseDirectoryCommand;

        public DelegateCommand ChooseDirectoryCommand
        {
            get
            {
                if (chooseDirectoryCommand == null)
                    chooseDirectoryCommand = new DelegateCommand
                    {
                        CommandAction = () =>
                        {
                            try
                            {
                                OpenFileDialog openFileDialog = new OpenFileDialog
                                {
                                    Multiselect = false,
                                    Title = "选择新建项目的位置",
                                    CheckFileExists = false,
                                    CheckPathExists = true,
                                    FileName = "Folder Selection"
                                };
                                if (openFileDialog.ShowDialog() == true)
                                {
                                    Path = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);
                                    RaisePropertyChanged("Path");
                                }
                            } catch (Exception e)
                            {
                                Utils.ExceptionHandler.ShowExceptionMessage(e);
                            }
                        }
                    };
                return chooseDirectoryCommand;
            }
        }
    }
}
