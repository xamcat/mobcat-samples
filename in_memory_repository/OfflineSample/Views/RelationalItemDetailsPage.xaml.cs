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
    public partial class RelationalItemDetailPage : ContentPage
    {
        RelationalItemDetailViewModel viewModel;

        public RelationalItemDetailPage(RelationalItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public RelationalItemDetailPage()
        {
            InitializeComponent();

            var user = new SampleUserModel
            {
                Name = "Sample User"
            };

            viewModel = new RelationalItemDetailViewModel(user);
            BindingContext = viewModel;
        }
    }
}