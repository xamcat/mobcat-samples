using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.MobCAT;
using Weather.Droid.Services;
using Weather.Services.Abstractions;

namespace Weather.Droid
{
    [Activity(Label = "Weather", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Bootstrap.Begin(() =>
            {
                ServiceContainer.Register<ILocalizationService>(new LocalizationService());
            });

            LoadApplication(new App());
        }
    }
}