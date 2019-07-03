using Microsoft.MobCAT.Forms.Pages;
using Weather.ViewModels;
using Xamarin.Forms;

namespace Weather
{
    public partial class MainPage : BaseContentPage<WeatherViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            if (DesignMode.IsDesignModeEnabled)
            {
                BackgroundImage.Opacity = 1d;
                WeatherImage.Opacity = 1d;
                CityName.Opacity = 1d;
                WeatherDescriptionLabel.Opacity = 1d;
                TemperatureRangeLabel.Opacity = 1d;
                CurrentTemperatureLabel.Opacity = 1d;
            }
        }
    }
}