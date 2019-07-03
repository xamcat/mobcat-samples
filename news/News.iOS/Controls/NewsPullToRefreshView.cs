using System;
using System.Linq;
using Airbnb.Lottie;
using UIKit;

namespace News.iOS.Controls
{
    public class NewsPullToRefreshView : UIView
    {
        private LOTAnimationView _loadingAnimationView;
        private readonly UIRefreshControl _container;

        public NewsPullToRefreshView(UIRefreshControl container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container;
            InitContainerView();
            InitAnimationView();
        }

        private void InitContainerView()
        {
            foreach (var item in _container.Subviews.ToList())
                item.RemoveFromSuperview();

            this.ClipsToBounds = true;
            this.TranslatesAutoresizingMaskIntoConstraints = false;
            _container.AddSubview(this);
            _container.ClipsToBounds = true;
            var width = NSLayoutConstraint.Create(this, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _container, NSLayoutAttribute.Width, 1f, 0f);
            var height = NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _container, NSLayoutAttribute.Height, 1f, 0f);
            _container.AddConstraint(width);
            _container.AddConstraint(height);
        }

        private void InitAnimationView()
        {
            if (_loadingAnimationView != null)
                _loadingAnimationView.RemoveFromSuperview();

            _loadingAnimationView = LOTAnimationView.AnimationNamed("lottie_pull_to_refresh");
            _loadingAnimationView.ClipsToBounds = true;
            _loadingAnimationView.AnimationSpeed = 3.0f;
            _loadingAnimationView.ContentMode = UIViewContentMode.ScaleAspectFill;
            _loadingAnimationView.LoopAnimation = true;
            _loadingAnimationView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(_loadingAnimationView);
            var animationCenterX = NSLayoutConstraint.Create(_loadingAnimationView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1f, 0f);
            //var animationCenterY = NSLayoutConstraint.Create(_loadingAnimationView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, refreshControl, NSLayoutAttribute.CenterY, 1f, 0f);
            var animationWidth = NSLayoutConstraint.Create(_loadingAnimationView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 0.3f, 0f);
            var animationHeight = NSLayoutConstraint.Create(_loadingAnimationView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this, NSLayoutAttribute.Height, 1f, 0f);
            this.AddConstraint(animationCenterX);
            //this.AddConstraint(animationCenterY);
            this.AddConstraint(animationWidth);
            this.AddConstraint(animationHeight);
        }

        public void Play()
        {
            _loadingAnimationView?.Play();
        }
    }
}