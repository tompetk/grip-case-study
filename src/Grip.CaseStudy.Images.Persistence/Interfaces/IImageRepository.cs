namespace Grip.CaseStudy.Images.Persistence.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<byte>?> DownloadAsync(string imageId, CancellationToken cancellationToken = default);
        Task UploadAsync(string imageId, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default);
    }
}