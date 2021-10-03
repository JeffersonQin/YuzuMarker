using System;
using System.Collections.Generic;
using Xamarin.Forms;
using YuzuMarker.App.ViewModels;
using YuzuMarker.App.Views;

namespace YuzuMarker.App
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
