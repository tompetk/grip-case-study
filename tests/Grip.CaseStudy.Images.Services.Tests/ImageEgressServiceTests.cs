using FluentAssertions;
using Grip.CaseStudy.Images.Persistence.Interfaces;
using Grip.CaseStudy.Images.Services.Constants;
using Grip.CaseStudy.Images.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grip.CaseStudy.Images.Services.Tests
{
    [TestClass]
    public class ImageEgressServiceTests
    {
        private readonly Mock<IImageRepository> _imageRepositoryMock;
        private readonly Mock<IImageScalingService> _imageScalingServiceMock;
        private readonly ImageEgressService _imageEgressService;
        private readonly CancellationToken _cancellationToken = default;

        private readonly byte[] _fakeImageBytes = new byte[] { 1, 2, 3, 4 };
        private readonly byte[] _fakeThumbnailBytes = new byte[] { 5, 6 };

        private const string ExistingImageId = "image1";
        private const string NonExistingImageId = "image2";

        private string GetThumbnailId(string imageId) => 
            $"{imageId}_{PredefinedImagePostfixes.Thumbnail}";

        public ImageEgressServiceTests() 
        {
            _imageRepositoryMock = new Mock<IImageRepository>();
            _imageScalingServiceMock = new Mock<IImageScalingService>();

            _imageRepositoryMock
                .Setup(x => x.DownloadAsync(It.IsAny<string>(), _cancellationToken))
                .Returns(Task.FromResult<IEnumerable<byte>?>(null));

            _imageRepositoryMock
                .Setup(x => x.DownloadAsync(ExistingImageId, _cancellationToken))
                .ReturnsAsync(_fakeImageBytes);

            _imageRepositoryMock
                .Setup(x => x.DownloadAsync(GetThumbnailId(ExistingImageId), _cancellationToken))
                .ReturnsAsync(_fakeThumbnailBytes);

            _imageEgressService = new ImageEgressService(_imageRepositoryMock.Object, _imageScalingServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAsync_ExistingImageThumbnail_ShouldSucceed()
        {
            var imageBytes = await _imageEgressService.GetAsync(
                ExistingImageId, PredefinedImageHeights.Thumbnail, _cancellationToken);
            
            imageBytes.Should().BeEquivalentTo(_fakeThumbnailBytes);

            _imageRepositoryMock.Verify(x => 
                x.DownloadAsync(
                    GetThumbnailId(ExistingImageId), 
                    _cancellationToken),
                Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_NonExistingImageThumbnail_ShouldReturnNull()
        {
            var imageBytes = await _imageEgressService.GetAsync(
                NonExistingImageId, PredefinedImageHeights.Thumbnail, _cancellationToken);

            imageBytes.Should().BeNull();

            _imageRepositoryMock.Verify(x =>
                x.DownloadAsync(
                    GetThumbnailId(NonExistingImageId),
                    _cancellationToken),
                Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_NonExistingImage_ShouldReturnNull()
        {
            var imageBytes = await _imageEgressService.GetAsync(
                NonExistingImageId, 200, _cancellationToken);

            imageBytes.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAsync_ExistingImage_ShouldSucceed()
        {
            var imageBytes = await _imageEgressService.GetAsync(
                ExistingImageId, 200, _cancellationToken);

            imageBytes.Should().NotBeNull();

            _imageScalingServiceMock.Verify(x =>
                x.ScaleAsync(_fakeImageBytes, 200, _cancellationToken), Times.Once());

            // TODO: assert image resolution and more.
        }
    }
}