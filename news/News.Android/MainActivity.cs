using Android.App;
using Android.Content.PM;
using Android.OS;
using CarouselView.FormsPlugin.Android;

namespace News.Droid
{
    [Activity(Label = "News", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CarouselViewRenderer.Init();
            LoadApplication(new App());
            Lottie.Forms.Droid.AnimationViewRenderer.Init();
            News.Bootstrap.Begin();
        }
    }
}