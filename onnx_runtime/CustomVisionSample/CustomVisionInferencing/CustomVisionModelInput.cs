using System;

namespace CustomVisionInferencing
{
    public sealed class CustomVisionModelInput
    {
        CustomVisionModelInput() { }

        public byte[] Bytes { get; private set; }
        public int BytesPerPixel { get; private set; }
        public int RowLength { get; private set; }
        public int ColumnLength { get; private set; }

        public static CustomVisionModelInput Create(byte[] bytes, int bytesPerPixel)
        {
            var imageBytes = bytes ?? throw new ArgumentException(nameof(bytes));

            var expectedPixelsPerChannel = CustomVisionClassificationModelConfig.ImageSizeX * CustomVisionClassificationModelConfig.ImageSizeY;
            var expectedPixels = expectedPixelsPerChannel * CustomVisionClassificationModelConfig.NumberOfChannels;
            var actualPixelsPerChannel = bytes.Length / bytesPerPixel;
            var actualPixels = actualPixelsPerChannel * CustomVisionClassificationModelConfig.NumberOfChannels;

            if (expectedPixels != actualPixels)
                throw new ArgumentException($"The length of the {nameof(bytes)} is expected to be {expectedPixels}");

            return new CustomVisionModelInput
            {
                Bytes = imageBytes,
                BytesPerPixel = bytesPerPixel,
                RowLength = bytesPerPixel * CustomVisionClassificationModelConfig.ImageSizeX,
                ColumnLength = bytesPerPixel * CustomVisionClassificationModelConfig.ImageSizeY,
            };
        }
    }
}