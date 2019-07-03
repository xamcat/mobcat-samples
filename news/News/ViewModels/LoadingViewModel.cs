using System;
using System.Threading.Tasks;
using Microsoft.MobCAT.MVVM;
using News;
using News.Services.Abstractions;
using Xamarin.Forms;

namespace News.ViewModels
{
    /// <summary>
    /// Loading view model.
    /// </summary>
    public class LoadingViewModel : BaseNavigationViewModel
    {
        protected IVisualThemeService VisualThemeService => DependencyService.Resolve<IVisualThemeService>();

        public AsyncCommand ContinueCommand { get; }

        public LoadingViewModel()
        {
            VisualThemeService.LoadSelectedVisualTheme();
            ContinueCommand = new AsyncCommand(OnContinue);
        }

        private async Task OnContinue()
        {
            await Navigation.PushAsync(new HomeViewModel(), true);
        }
    }
}