namespace SmartPark.Dtos.Booking
{
    public record BookingHistoryRequest
    {
        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartedAt { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndedAt { get; set; }

        [Required(ErrorMessage = "Status snapshot is required")]
        public string StatusSnapshot { get; set; } = null!;

        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "SlotId is required")]
        public Guid SlotId { get; set; }

        [Required(ErrorMessage = "BookingId is required")]
        public Guid BookingId { get; set; }
    }
}
