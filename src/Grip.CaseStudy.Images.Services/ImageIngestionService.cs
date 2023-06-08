using Grip.CaseStudy.Images.Persistence;
using Grip.CaseStudy.Images.Persistence.Interfaces;
using Grip.CaseStudy.Images.Services.Constants;
using Grip.CaseStudy.Images.Services.Interfaces;

namespace Grip.CaseStudy.Images.Services
{
    public class ImageIngestionService : IImageIngestionService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IImageScalingService _imageScalingService;

        public ImageIngestionService(
            IImageRepository imageRepository,
            IImageScalingService imageScalingService)
        {
            _imageRepository = imageRepository;
            _imageScalingService = imageScalingService;
        }

        public Task<bool> ValidateAsync(
            IEnumerable<byte> imageBytes, 
            string contentType, 
            CancellationToken cancellationToken = default)
        {
            if (!contentType.ToLowerInvariant().Equals("image/jpeg"))
                return Task.FromResult(false);

            // TODO: validate resolution, image validity, etc.
            return Task.FromResult(true);
        }

        public async Task<string> IngestAsync(
            IEnumerable<byte> imageBytes, 
            CancellationToken cancellationToken = default)
        {
            var newImageId = Guid.NewGuid().ToString();

            // Save original.
            await _imageRepository.UploadAsync(newImageId, imageBytes, cancellationToken);

            // Scale and save thumbnail.
            var thumbnailImageId = $"{newImageId}_{PredefinedImagePostfixes.Thumbnail}";
            var thumbnailImageBytes = await _imageScalingService.ScaleAsync(imageBytes, PredefinedImageHeights.Thumbnail);

            await _imageRepository.UploadAsync(thumbnailImageId, thumbnailImageBytes, cancellationToken);

            // TODO: consider mocing out thumbnail (and other predefined resolutions) scaling to be processed by another job.
            //  not to put load on the API itself.

            return newImageId;
        }
    }
}