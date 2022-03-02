using System;
using System.Threading.Tasks;
using SkiaSharp;

namespace CustomVisionInferencing.SkiaSharp
{
    public class CustomVisionImageProcessor : ICustomVisionImageProcessor
    {
        public async Task<CustomVisionModelInput> CreateResizeCenterCroppedRGBAImageAsync(byte[] image, int width, int height)
        {
            using SKBitmap sourceBitmap = await Task.Run(() => SKBitmap.Decode(image)).ConfigureAwait(false);

            float ratio = (float)
                Math.Min(
                    CustomVisionClassificationModelConfig.ImageSizeX,
                    CustomVisionClassificationModelConfig.ImageSizeY) /
                Math.Min(
                    sourceBitmap.Width,
                    sourceBitmap.Height);

            using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(
                (int)Math.Round(ratio * sourceBitmap.Width),
                (int)Math.Round(ratio * sourceBitmap.Height)),
                SKFilterQuality.Medium);

            var horizontalCrop = scaledBitmap.Width - CustomVisionClassificationModelConfig.ImageSizeX;
            var verticalCrop = scaledBitmap.Height - CustomVisionClassificationModelConfig.ImageSizeY;
            var leftOffset = horizontalCrop == 0 ? 0 : horizontalCrop / 2;
            var topOffset = verticalCrop == 0 ? 0 : verticalCrop / 2;

            var cropRect = SKRectI.Create(
                new SKPointI(
                    leftOffset,
                    topOffset),
                new SKSizeI(
                    CustomVisionClassificationModelConfig.ImageSizeX,
                    CustomVisionClassificationModelConfig.ImageSizeY));

            using SKImage currentImage = SKImage.FromBitmap(scaledBitmap);
            using SKImage croppedImage = currentImage.Subset(cropRect);
            using SKBitmap croppedBitmap = SKBitmap.FromImage(croppedImage);

            var bytes = new byte[croppedBitmap.Bytes.Length];
            croppedBitmap.Bytes.CopyTo(bytes, 0);

            return CustomVisionModelInput.Create(bytes, croppedBitmap.BytesPerPixel);
        }
    }
}