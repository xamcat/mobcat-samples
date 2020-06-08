using System;
using System.ComponentModel;
using PushDemo.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PushDemo
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

        public MainPage()
        {
            InitializeComponent();

            _notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>();
        }

        void RegisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.RegisterDeviceAsync().ContinueWith((task)
                => { ShowAlert(task.IsFaulted ?
                        task.Exception.Message :
                        $"Device registered"); });

        void DeregisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.DeregisterDeviceAsync().ContinueWith((task)
                => { ShowAlert(task.IsFaulted ?
                       task.Exception.Message :
                       $"Device deregistered"); });

        void ShowAlert(string message)
            => MainThread.BeginInvokeOnMainThread(()
                => DisplayAlert("PushDemo", message, "OK").ContinueWith((task)
                    => { if (task.IsFaulted) throw task.Exception; }));
    }
}