namespace Grip.CaseStudy.Images.API.DTOs
{
    public class UploadImageResponseDTO
    {
        public UploadImageResponseDTO(string imageId)
        {
            ImageId = imageId;
        }

        public string ImageId { get; set; }
    }
}