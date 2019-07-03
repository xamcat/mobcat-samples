using CarouselView.FormsPlugin.iOS;
using Foundation;
using UIKit;

namespace News.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Calabash.Start();
            global::Xamarin.Forms.Forms.Init();
            CarouselViewRenderer.Init();
            LoadApplication(new App());
            Lottie.Forms.iOS.Renderers.AnimationViewRenderer.Init();
            News.Bootstrap.Begin();

            return base.FinishedLaunching(app, options);
        }
    }
}