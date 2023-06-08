using Grip.CaseStudy.Images.Persistence.Interfaces;
using Grip.CaseStudy.Images.Services.Constants;
using Grip.CaseStudy.Images.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grip.CaseStudy.Images.Services
{
    public class ImageEgressService : IImageEgressService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IImageScalingService _imageScalingService;

        public ImageEgressService(
            IImageRepository imageRepository,
            IImageScalingService imageScalingService)
        {
            _imageRepository = imageRepository;
            _imageScalingService = imageScalingService;
        }

        public async Task<IEnumerable<byte>?> GetAsync(string imageId, uint height, CancellationToken cancellationToken = default)
        {
            if (height == PredefinedImageHeights.Thumbnail)
            {
                var thumbnailImageId = $"{imageId}_{PredefinedImagePostfixes.Thumbnail}";
                var thumbnailImageBytes = await _imageRepository.DownloadAsync(thumbnailImageId, cancellationToken);
                return thumbnailImageBytes;
            }

            var originalImageBytes = await _imageRepository.DownloadAsync(imageId, cancellationToken);
            if (originalImageBytes == null)
                return null;

            var scaledImageBytes = await _imageScalingService.ScaleAsync(originalImageBytes, height, cancellationToken);
            return scaledImageBytes;
        }
    }
}
