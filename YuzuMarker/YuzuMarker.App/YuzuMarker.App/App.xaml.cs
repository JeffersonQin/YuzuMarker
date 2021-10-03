using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YuzuMarker.App.Services;
using YuzuMarker.App.Views;

namespace YuzuMarker.App
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
