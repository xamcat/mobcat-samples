using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CustomVisionInferencing.OnnxRuntime
{
    public class CustomVisionImageClassifier : ICustomVisionImageClassifier
    {
        byte[] _model;
        ICustomVisionImageProcessor _imageProcessor;
        string _namedConfiguration;
        InferenceSession _session;
        int _channelLength;
        int _inputLength;
        Task _initTask;

        Task InitAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask();

            return _initTask;
        }

        public CustomVisionImageClassifier(byte[] model, ICustomVisionImageProcessor imageProcessor, string namedConfiguration = null)
        {
            _model = model ?? throw new ArgumentException(nameof(model));
            _imageProcessor = imageProcessor ?? throw new ArgumentException(nameof(imageProcessor));
            _namedConfiguration = namedConfiguration;
            _channelLength = CustomVisionClassificationModelConfig.ImageSizeX * CustomVisionClassificationModelConfig.ImageSizeY;
            _inputLength = _channelLength * CustomVisionClassificationModelConfig.BatchSize * CustomVisionClassificationModelConfig.NumberOfChannels;
            _ = InitAsync();
        }

        public async Task<CustomVisionClassificationModelOutput> RunAsync(byte[] image)
        {
            await InitAsync().ConfigureAwait(false);

            if (_initTask.IsFaulted)
                throw new Exception($"{nameof(CustomVisionImageClassifier)} failed to initialize", _initTask.Exception);

            if (image == null)
                throw new ArgumentException(nameof(image));

            var imageData = await _imageProcessor.CreateResizeCenterCroppedRGBAImageAsync(
                image,
                CustomVisionClassificationModelConfig.ImageSizeX,
                CustomVisionClassificationModelConfig.ImageSizeY);

            var imageBytes = imageData.Bytes;
            var bytesPerPixel = imageData.BytesPerPixel;
            var rowLength = imageData.RowLength;

            var channelDataIndex = 0;
            var channelData = new float[_inputLength];

            for (int y = 0; y < CustomVisionClassificationModelConfig.ImageSizeY; y++)
            {
                var rowOffset = y * rowLength;

                for (int x = 0, columnOffset = 0; x < CustomVisionClassificationModelConfig.ImageSizeX; x++, columnOffset += bytesPerPixel)
                {
                    var pixelOffset = rowOffset + columnOffset;

                    // Image bytes are in RGBA sequence
                    var pixelR = imageBytes[pixelOffset];
                    var pixelG = imageBytes[pixelOffset + 1];
                    var pixelB = imageBytes[pixelOffset + 2];

                    // Expects channels in flat sequence in BGR order
                    var rChannelIndex = channelDataIndex + (_channelLength * 2);
                    var gChannelIndex = channelDataIndex + _channelLength;
                    var bChannelIndex = channelDataIndex;

                    channelData[rChannelIndex] = pixelR;
                    channelData[gChannelIndex] = pixelG;
                    channelData[bChannelIndex] = pixelB;
                    
                    channelDataIndex++;
                }
            }

            var tensor = new DenseTensor<float>(
                channelData,
                new[] {
                    CustomVisionClassificationModelConfig.BatchSize,
                    CustomVisionClassificationModelConfig.NumberOfChannels,
                    CustomVisionClassificationModelConfig.ImageSizeY,
                    CustomVisionClassificationModelConfig.ImageSizeX });

            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(
                    CustomVisionClassificationModelConfig.ModelInputName,
                    tensor)
            });

            var topLabel = result
                ?.FirstOrDefault(i => i.Name == CustomVisionClassificationModelConfig.ModelClassLabelOutputName)
                ?.AsTensor<string>()
                ?.First();

            var labelScores = result
                ?.FirstOrDefault(i => i.Name == CustomVisionClassificationModelConfig.ModelLossOutputName)
                ?.AsEnumerable<NamedOnnxValue>()
                ?.First()
                ?.AsDictionary<string, float>();

            return CustomVisionClassificationModelOutput.Create(topLabel, labelScores);
        }

        Task InitTask()
        {
            var sessionOptions = new SessionOptions();

            if (!string.IsNullOrWhiteSpace(_namedConfiguration))
                sessionOptions.ApplyConfiguration(_namedConfiguration);

            _session = new InferenceSession(_model, sessionOptions);

            // Warm up the registers
            return Task.Run(() =>
            {
                var input = new float[_inputLength];

                var tensor = new DenseTensor<float>(
                    input,
                    new[] {
                        CustomVisionClassificationModelConfig.BatchSize,
                        CustomVisionClassificationModelConfig.NumberOfChannels,
                        CustomVisionClassificationModelConfig.ImageSizeY,
                        CustomVisionClassificationModelConfig.ImageSizeX });

                _ = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(CustomVisionClassificationModelConfig.ModelInputName, tensor) });
            });
        }
    }
}