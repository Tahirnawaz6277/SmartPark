using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.UserDtos
{
    public record LoginRequest
    {
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
