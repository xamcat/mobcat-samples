using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OfflineSample.Services;
using OfflineSample.Views;
using OfflineSample.Data.InMemory;

namespace OfflineSample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<InMemorySampleRepository>();
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
