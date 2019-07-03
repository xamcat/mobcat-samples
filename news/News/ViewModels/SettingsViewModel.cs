using System;
using Microsoft.MobCAT.MVVM;
using News.Services.Abstractions;
using Xamarin.Forms;

namespace News.ViewModels
{
    public class SettingsViewModel : BaseNavigationViewModel
    {
        protected IVisualThemeService VisualThemeService => DependencyService.Resolve<IVisualThemeService>();

        private bool _isLightMode;
        public bool IsLightMode
        {
            get => _isLightMode;
            set
            {
                VisualThemeService.SetVisualTheme(value ? VisualTheme.Light : VisualTheme.Dark);
                RaiseAndUpdate(ref _isLightMode, value);
            }
        }

        public SettingsViewModel()
        {
            IsLightMode = VisualThemeService.CurrentVisualTheme == VisualTheme.Light;
        }
    }
}