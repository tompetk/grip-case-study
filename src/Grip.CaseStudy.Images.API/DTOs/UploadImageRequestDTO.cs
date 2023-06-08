using System.ComponentModel.DataAnnotations;

namespace Grip.CaseStudy.Images.API.DTOs
{
    public class UploadImageRequestDTO
    {
        //[StringLength(64)]
        //public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}