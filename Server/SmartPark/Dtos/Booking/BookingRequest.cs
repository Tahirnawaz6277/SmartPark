using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.Booking
{
    public record BookingRequest
    {
        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = null!;

        //[Required(ErrorMessage = "Booking DateTime is required")]
        //public DateTime BookingDateTime { get; set; }

        [Required(ErrorMessage = "Parking start time is required")]
        public DateTime ParkingStartTime { get; set; }

        [Required(ErrorMessage = "Parking end time is required")]
        public DateTime ParkingEndTime { get; set; }

        [Required(ErrorMessage = "SlotId is required")]
        public Guid SlotId { get; set; }
    }
}
