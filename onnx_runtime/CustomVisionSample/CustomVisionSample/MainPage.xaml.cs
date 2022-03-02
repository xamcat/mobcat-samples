using System;
using System.IO;
using System.Threading.Tasks;
using CustomVisionInferencing;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CustomVisionSample
{
    public partial class MainPage : ContentPage
    {
        ICustomVisionImageProcessor _imageProcessor;
        ICustomVisionImageClassifier _classifier;
        byte[] _sampleImage;
        Task _initTask;

        public MainPage()
        {
            InitializeComponent();
            _imageProcessor = new CustomVisionInferencing.SkiaSharp.CustomVisionImageProcessor();
            _ = InitAsync();
        }

        public Task InitAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask();

            return _initTask;
        }

        async Task RunInferenceAsync()
        {
            RunButton.IsEnabled = false;

            try
            {
                await InitAsync().ConfigureAwait(false);
                var result = await _classifier.RunAsync(_sampleImage).ConfigureAwait(false);

                MainThread.BeginInvokeOnMainThread(() => _ = DisplayAlert(
                    "Result",
                    $"{result.TopResultLabel} ({result.TopResultScore})",
                    "OK"));
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() => _ = DisplayAlert(
                    "Error",
                    ex.Message,
                    "OK"));
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => RunButton.IsEnabled = true);
            }
        }

        Task InitTask() => Task.Run(() =>
        {
            var assembly = GetType().Assembly;

            // Get model
            using var modelStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.PlanktonModel.onnx");
            using var modelMemoryStream = new MemoryStream();

            modelStream.CopyTo(modelMemoryStream);
            var model = modelMemoryStream.ToArray();

            _classifier = new CustomVisionInferencing.OnnxRuntime.CustomVisionImageClassifier(model, _imageProcessor);

            // Get sample image
            using var sampleImageStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.sample.bmp");
            using var sampleImageMemoryStream = new MemoryStream();

            sampleImageStream.CopyTo(sampleImageMemoryStream);
            _sampleImage = sampleImageMemoryStream.ToArray();
        });

        void RunButton_Clicked(object sender, EventArgs e)
            => _ = RunInferenceAsync();
    }
}