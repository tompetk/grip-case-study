namespace Grip.CaseStudy.Images.Services.Interfaces
{
    public interface IImageEgressService
    {
        Task<IEnumerable<byte>?> GetAsync(string imageId, uint height, CancellationToken cancellationToken = default);
    }
}