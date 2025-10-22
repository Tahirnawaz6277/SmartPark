using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.Location
{
    public record LocationRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "TotalSlots is required")]
        public int TotalSlots { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = null!;

        public IFormFile? ImageFile { get; set; }

    }
}
