using System;
using System.Reflection;
using Microsoft.MobCAT;
using Microsoft.MobCAT.Forms.Services;
using Microsoft.MobCAT.MVVM.Abstractions;
using News.Pages;

namespace News
{
    public static class Bootstrap
    {
        public static void Begin(Action platformSpecificBegin = null)
        {
            var navigationService = new NavigationService();
            navigationService.RegisterViewModels(typeof(HomePage).GetTypeInfo().Assembly);

            ServiceContainer.Register<INavigationService>(navigationService);
        
            platformSpecificBegin?.Invoke();
        }
    }
}
