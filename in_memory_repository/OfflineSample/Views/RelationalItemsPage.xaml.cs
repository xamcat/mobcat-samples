using OfflineSample.Data;
using OfflineSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OfflineSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RelationalItemsPage : ContentPage
    {
        RelationalItemsViewModel viewModel;

        public RelationalItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new RelationalItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var user = args.SelectedItem as SampleUserModel;
            if (user == null)
                return;

            await Navigation.PushAsync(new RelationalItemDetailPage(new RelationalItemDetailViewModel(user)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Users.Count == 0)
                viewModel.LoadUsersCommand.Execute(null);
        }
    }
}