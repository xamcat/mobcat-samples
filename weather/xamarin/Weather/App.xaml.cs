using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.MobCAT;
using Weather.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Weather
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            try
            {
                // Handle when your app starts
                AppCenter.Start($"{ServiceConfig.AndroidAppCenterSecret};" +
                      $"{ServiceConfig.iOSAppCenterSecret}",
                      typeof(Analytics), typeof(Crashes));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}