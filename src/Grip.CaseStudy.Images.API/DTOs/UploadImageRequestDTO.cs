using System.ComponentModel.DataAnnotations;

namespace Grip.CaseStudy.Images.API.DTOs
{
    public class UploadImageRequestDTO
    {
        public IFormFile File { get; set; }
    }
}