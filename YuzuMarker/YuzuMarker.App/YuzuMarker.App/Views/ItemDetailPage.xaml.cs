using System.ComponentModel;
using Xamarin.Forms;
using YuzuMarker.App.ViewModels;

namespace YuzuMarker.App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}