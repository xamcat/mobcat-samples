using Lottie.Forms;
using Xamarin.Forms;

namespace News.Controls
{
    public class FavoriteButton : ImageButton
    {
        #region dependency properties

        public static readonly BindableProperty IsFavoriteProperty = BindableProperty.Create(nameof(IsFavorite), typeof(bool?), typeof(FavoriteButton), null, propertyChanged: OnIsFavoriteChanged);

        public bool IsFavorite
        {
            get { return ((bool?)GetValue(IsFavoriteProperty) ?? false); }
            set { SetValue(IsFavoriteProperty, value); }
        }

        private static void OnIsFavoriteChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var button = (FavoriteButton)bindable;
            var stateChanged = (bool?)oldValue != (bool?)newValue;
            if (stateChanged)
            {
                button.UpdateControlState();

                if (oldValue != null)
                {
                    button.UpdateControlStateWithAnimation();
                }
            }
        }

        #endregion

        private AnimationView _animationView;

        public FavoriteButton()
        {
            AutomationId = "FavoriteButton";
            Init();
            UpdateControlState();
        }

        private void Init()
        {
            BackgroundColor = Color.Transparent;
            Aspect = Aspect.AspectFill;

            if (_animationView == null)
            {
                _animationView = new AnimationView()
                {
                    Animation = "lottie_favorite_add.json",
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    BackgroundColor = Color.Transparent,
                    Loop = false,
                    AutoPlay = false,
                    IsVisible = false,
                    PlaybackFinishedCommand = new Command(UpdateControlState),
                    Margin = new Thickness(0, 1, 1, 0),
                };
            }
        }

        private void UpdateControlState()
        {
            if (_animationView != null)
            {
                _animationView.IsPlaying = false;
                _animationView.IsVisible = false;
            }

            Source = IsFavorite ? "ic_favorite_fill.png" : "ic_favorite_empty.png";
        }

        private void UpdateControlStateWithAnimation()
        {
            if (IsFavorite)
            {
                if (Parent is Layout<View> animationContainer)
                {
                    //await Task.Delay(1000);
                    if (!animationContainer.Children.Contains(_animationView))
                    {
                        animationContainer.Children.Add(_animationView);
                    }

                    Source = null;
                    _animationView.AbortAnimation(GetHashCode().ToString());
                    _animationView.IsVisible = true;
                    _animationView.Play();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Favorite button doesn't have parent container to render animation.");
                }
            }
        }
    }
}