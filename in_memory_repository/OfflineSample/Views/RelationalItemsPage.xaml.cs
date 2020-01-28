using OfflineSample.Data;
using OfflineSample.Services;
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
        private IItemGeneratorService ItemGeneratorService => DependencyService.Get<IItemGeneratorService>();

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

        async void AddUsers_Clicked(object sender, EventArgs e)
        {
            var random = new Random();
            var userCount = random.Next(1, 10);
            var orderCount = 0;
            var minimumOrderCount = 0;
            var maximumOrderCount = 5;
            for (int i = 0; i < userCount; i++)
            {
                orderCount = random.Next(minimumOrderCount, maximumOrderCount + 1);
                await ItemGeneratorService.GenerateUserAsync(orderCount);
            }
            viewModel.LoadUsersCommand.Execute(null);
            await DisplayAlert(title: "Success",
                message: $"Generated {userCount} user(s) with {minimumOrderCount}-{maximumOrderCount} orders each.",
                cancel: "Thanks");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Users == null || viewModel.Users.Count == 0)
            {
                viewModel.LoadUsersCommand.Execute(null);
            }
        }
    }
}