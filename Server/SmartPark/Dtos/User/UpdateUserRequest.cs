using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.UserDtos
{
    public record UpdateUserRequest
    {
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]

        public string Email { get; set; }

        public string? Address { get; set; }

        //[Required(ErrorMessage = "PhoneNumber is Required")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }


    }
}
