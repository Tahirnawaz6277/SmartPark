using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.Booking
{
    public record BookingRequest
    {
        [Required(ErrorMessage = "StartTime is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "At lease one SlotId is required for booking")]
        public List<Guid> SlotIds { get; set; }
    }
}
