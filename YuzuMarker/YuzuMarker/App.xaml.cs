using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YuzuMarker.Common;
using YuzuMarker.DataFormat;
using YuzuMarker.Properties;

namespace YuzuMarker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CoreSettings.PhotoshopBridgeType = (PSBridgeType)Settings.Default.PhotoshopBridgeType;
            CoreSettings.PhotoshopExtensionHTTPServerPort = Settings.Default.PhotoshopExtensionHTTPServerPort;
            UndoRedoManager.StopRecording();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Settings.Default.Save();
        }
    }
}
