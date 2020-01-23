using System;
using Xamarin.Forms;
using OfflineSample.Data.InMemory;
using OfflineSample.Services;

namespace OfflineSample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<InMemorySampleRepository>();
            DependencyService.Register<InMemorySampleUserRepository>();
            DependencyService.Register<InMemorySampleOrderRepository>();
            DependencyService.Register<ItemGeneratorService>();

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
