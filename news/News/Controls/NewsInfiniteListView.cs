using Xamarin.Forms;
using Lottie.Forms;
using MobCAT.Forms.Controls;

namespace News.Controls
{
    public class NewsInfiniteListView : InfiniteListView
    {
        private AnimationView _animationView;

        public NewsInfiniteListView()
        {
            AutomationId = nameof(NewsInfiniteListView);
        }

        protected override void OnIsLoadingMoreChanged()
        {
            InitializeLoadingMoreAnimation();

            if (IsLoadingMore)
            {
                if (Footer == null)
                {
                    Footer = _animationView;
                }

                _animationView.AbortAnimation(GetHashCode().ToString());
                _animationView.IsVisible = true;
                _animationView.Play();
            }
            else
            {
                _animationView.AbortAnimation(GetHashCode().ToString());
                _animationView.IsPlaying = false;
                _animationView.IsVisible = false;
                Footer = null;
            }
        }

        private void InitializeLoadingMoreAnimation()
        {
            if (_animationView == null)
            {
                _animationView = new AnimationView()
                {
                    Animation = "lottie_load_more.json",
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    Loop = true,
                    AutoPlay = false,
                    IsVisible = false,
                    HeightRequest = 220,
                    Margin = new Thickness(0),
                };
            }
        }
    }
}