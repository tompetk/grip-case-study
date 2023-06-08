namespace Grip.CaseStudy.Images.Services.Interfaces
{
    public interface IImageScalingService
    {
        Task<IEnumerable<byte>> ScaleAsync(IEnumerable<byte> imageBytes, uint height, CancellationToken cancellationToken = default);
    }
}