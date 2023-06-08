using Grip.CaseStudy.Images.Services.Interfaces;
using SkiaSharp;

namespace Grip.CaseStudy.Images.Services
{
    public class ImageScalingService : IImageScalingService
    {
        public Task<IEnumerable<byte>> ScaleAsync(IEnumerable<byte> imageBytes, uint height, CancellationToken cancellationToken = default)
        {
            // TODO: optimize implementation.
            using MemoryStream stream = new MemoryStream(imageBytes.ToArray());
            using SKBitmap sourceBitmap = SKBitmap.Decode(stream);

            var scaleFactor = (double)sourceBitmap.Height / height;
            var width = (int)((double)sourceBitmap.Width / scaleFactor);

            using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, (int)height), SKFilterQuality.High); // TODO: make configurable
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100); // TODO: make configurable

            return Task.FromResult<IEnumerable<byte>>(data.ToArray());
        }
    }
}
