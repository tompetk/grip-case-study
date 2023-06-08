using Grip.CaseStudy.Images.API.DTOs;
using Grip.CaseStudy.Images.Services;
using Grip.CaseStudy.Images.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Grip.CaseStudy.Images.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImageIngestionService _imageIngestionService;
        private readonly IImageEgressService _imageEgressService;

        public ImagesController(
            ILogger<ImagesController> logger,
            IImageIngestionService imageIngestionService,
            IImageEgressService imageEgressService)
        {
            _logger = logger;
            _imageIngestionService = imageIngestionService;
            _imageEgressService = imageEgressService;
        }

        /// <summary>
        /// Uploads image. JPEG image type allowed only (for simplicity).
        /// </summary>
        /// <returns>Image ID of the uploaded image.</returns>
        [HttpPost()]
        [RequestSizeLimit(1_000_000)]
        public async Task<ActionResult<UploadImageResponseDTO>> UploadImage(
            [FromForm] UploadImageRequestDTO uploadFileRequestDTO,
            CancellationToken cancellationToken)
        {
            // Get image bytes.
            var imageBytes = new byte[uploadFileRequestDTO.File.Length];
            var stream = uploadFileRequestDTO.File.OpenReadStream();
            await stream.ReadAsync(imageBytes, cancellationToken);

            // Validate.
            if (!await _imageIngestionService.ValidateAsync(imageBytes, uploadFileRequestDTO.File.ContentType, cancellationToken))
                return BadRequest("Invalid image. Please refer to the docs.");

            // Ingest.
            var newImageId = await _imageIngestionService.IngestAsync(imageBytes, cancellationToken);

            return new OkObjectResult(new UploadImageResponseDTO(newImageId));
        }

        /// <summary>
        /// Returns uploaded image scaled to the given height.
        /// </summary>
        /// <param name="imageId">Image ID returned by the upload endpoint.</param>
        /// <param name="height">Height image should be scaled to (0 < height <= 4096).</param>
        /// <returns>JPEG image response with the according content type.</returns>
        [HttpGet("{imageId}")]
        public async Task<ActionResult> GetImage(
            [FromRoute] string imageId,
            [FromQuery] uint height,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(imageId))
                return BadRequest($"Invalid {nameof(imageId)}");

            if (height > 4096)
                return BadRequest($"Invalid {nameof(height)}. Please refer the docs.");

            var imageBytes = await _imageEgressService.GetAsync(imageId, height, cancellationToken);
            if (imageBytes == null)
                return NotFound();

            return File(imageBytes.ToArray(), "image/jpeg");
        }
    }
}