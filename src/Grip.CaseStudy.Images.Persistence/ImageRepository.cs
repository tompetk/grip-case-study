using Azure.Storage.Blobs;
using Grip.CaseStudy.Images.Persistence.Configuration;
using Grip.CaseStudy.Images.Persistence.Interfaces;

namespace Grip.CaseStudy.Images.Persistence
{
    public class ImageRepository : IImageRepository
    {
        protected readonly BlobContainerClient ContainerClient;

        public ImageRepository(StorageConfiguration configuration)
        {
            ContainerClient = new BlobContainerClient(configuration.ConnectionString, "images");
            ContainerClient.CreateIfNotExists();
        }

        public async Task<IEnumerable<byte>?> DownloadAsync(
            string imageId,
            CancellationToken cancellationToken = default)
        {
            var blobClient = GetBlobClient(imageId);
            if (!await blobClient.ExistsAsync(cancellationToken))
                return null;

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return buffer;
        }

        public async Task UploadAsync(
            string imageId,
            IEnumerable<byte> imageBytes,
            CancellationToken cancellationToken = default)
        {
            var blobClient = GetBlobClient(imageId);

            using var stream = new MemoryStream();
            stream.Write(imageBytes.ToArray());
            stream.Seek(0, SeekOrigin.Begin);

            await blobClient.UploadAsync(stream, true, cancellationToken);
        }

        private BlobClient GetBlobClient(string imageId)
        {
            return ContainerClient.GetBlobClient($"{imageId}");
        }
    }
}