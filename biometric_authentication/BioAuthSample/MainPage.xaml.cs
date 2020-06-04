using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiometricAuthentication.NetCore;
using Microsoft.MobCAT;
using Xamarin.Forms;

namespace BioAuthSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        IBiometricAuthenticationService _authHelper = ServiceContainer.Resolve<IBiometricAuthenticationService>();

        public MainPage()
        {
            InitializeComponent();

            var biometricType = _authHelper.AvailableBiometricType;
            StatusLabel.Text += biometricType;
            if (biometricType == BiometricType.None)
            {
                Authenticate.IsEnabled = false;
            }

            Authenticate.Clicked += Authenticate_Clicked;
        }

        async void Authenticate_Clicked(object sender, System.EventArgs e)
        {
            var authenticationResult = await _authHelper.AuthenticateAsync();

            if (authenticationResult.Success == true)
            {
                StatusValue.Text = "Authentication Status : Success!";
            }
            else
            {
                StatusValue.Text = "Authentication Status : Failure! :: " + authenticationResult.Message;
            }
        }
    }
}
