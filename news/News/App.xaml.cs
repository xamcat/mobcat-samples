using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.MobCAT.MVVM;
using Microsoft.MobCAT.Forms.Services;
using Microsoft.MobCAT.Forms.Pages;
using News.Pages;
using News.ViewModels;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace News
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SetMainPage();
        }

        private void SetMainPage()
        {
            var startPage = new LoadingPage();
            MainPage = new BaseNavigationPage(startPage);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
