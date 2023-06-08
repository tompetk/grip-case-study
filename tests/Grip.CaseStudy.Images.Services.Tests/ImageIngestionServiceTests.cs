using FluentAssertions;
using Grip.CaseStudy.Images.Persistence.Interfaces;
using Grip.CaseStudy.Images.Services.Constants;
using Grip.CaseStudy.Images.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Grip.CaseStudy.Images.Services.Tests
{
    [TestClass]
    public class ImageIngestionServiceTests
    {
        private readonly Mock<IImageRepository> _imageRepositoryMock;
        private readonly Mock<IImageScalingService> _imageScalingServiceMock;
        private readonly ImageIngestionService _imageIngestionService;
        private readonly CancellationToken _cancellationToken = default;

        private readonly byte[] _fakeImageBytes = new byte[] { 1, 2, 3, 4 };

    public ImageIngestionServiceTests() 
        {
            _imageRepositoryMock = new Mock<IImageRepository>();
            _imageScalingServiceMock = new Mock<IImageScalingService>();

            _imageIngestionService = new ImageIngestionService(_imageRepositoryMock.Object, _imageScalingServiceMock.Object);
        }

        [TestMethod]
        public async Task ValidateAsync_WithValidImage_ShouldSucceed()
        {
            var isValid = await _imageIngestionService.ValidateAsync(_fakeImageBytes, "image/jpeg", _cancellationToken);
            
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateAsync_WithPngImage_ShouldFail()
        {
            var isValid = await _imageIngestionService.ValidateAsync(_fakeImageBytes, "image/png", _cancellationToken);
            
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task IngestAsync_WithValidImage_ShouldSucceed()
        {
            var newImageId = await _imageIngestionService.IngestAsync(_fakeImageBytes, _cancellationToken);
            
            newImageId.Should().NotBeNull();
            _imageRepositoryMock.Verify(x => 
                x.UploadAsync(
                    newImageId, 
                    _fakeImageBytes, 
                    It.IsAny<CancellationToken>()),
                Times.Once());
            
            _imageRepositoryMock.Verify(x => 
                x.UploadAsync(
                    $"{newImageId}_{PredefinedImagePostfixes.Thumbnail}", 
                    It.IsAny<byte[]>(), // TODO: assert scaled image resolution. 
                    _cancellationToken), 
                Times.Once());

            _imageScalingServiceMock.Verify(x => 
                x.ScaleAsync(_fakeImageBytes, PredefinedImageHeights.Thumbnail, _cancellationToken), Times.Once());
        }
    }
}