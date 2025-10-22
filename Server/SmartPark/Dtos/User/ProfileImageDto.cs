using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.User
{
    public record ProfileImageDto
    {
        [Required(ErrorMessage ="UserId is required")]
        public Guid UserId { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
