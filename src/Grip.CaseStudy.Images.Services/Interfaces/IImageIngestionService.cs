namespace Grip.CaseStudy.Images.Services.Interfaces
{
    public interface IImageIngestionService
    {
        Task<string> IngestAsync(IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default);
        Task<bool> ValidateAsync(IEnumerable<byte> imageBytes, string contentType, CancellationToken cancellationToken = default);
    }
}