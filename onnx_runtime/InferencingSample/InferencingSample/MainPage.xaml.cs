using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InferencingSample
{
    public partial class MainPage : ContentPage
    {
        MobileNetImageClassifier _classifier;

        public MainPage()
        {
            InitializeComponent();
            _classifier = new MobileNetImageClassifier();
        }

        async Task RunInferenceAsync()
        {
            RunButton.IsEnabled = false;

            try
            {
                var sampleImage = await _classifier.GetSampleImageAsync();
                var result = await _classifier.GetClassificationAsync(sampleImage);
                await DisplayAlert("Result", result, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                RunButton.IsEnabled = true;
            }
        }

        void RunButton_Clicked(object sender, EventArgs e)
            => _ = RunInferenceAsync();
    }
}