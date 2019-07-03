using System;
using System.ComponentModel;
using Microsoft.MobCAT.Forms.Behaviors;
using Xamarin.Forms;

namespace Weather.Behaviors
{
    public class FadeInOnPropertyChangedBehavior : BehaviorBase<VisualElement>
    {
        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration),
            typeof(uint),
            typeof(FadeInOnPropertyChangedBehavior),
            defaultValue: (uint)1000);

        public uint Duration
        {
            get { return (uint)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly BindableProperty PropertyNameProperty =
        BindableProperty.Create(nameof(PropertyName),
            typeof(string),
            typeof(FadeInOnPropertyChangedBehavior),
            default(string));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.PropertyChanged -= OnPropertyChanged;
        }

        async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is VisualElement element)
            {
                if (e.PropertyName == PropertyName)
                {
                    await element.FadeTo(opacity: 1.0d, length: Duration, easing: Easing.CubicIn);
                }
            }
        }
    }
}
