using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.Booking
{
    public record BookingRequest
    {
        //[Required(ErrorMessage = "Booking DateTime is required")]
        //public DateTime BookingDateTime { get; set; }

        [Required(ErrorMessage = "SlotId is required")]
        public Guid SlotId { get; set; }
    }
}
