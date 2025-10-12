namespace SmartPark.Dtos.Booking
{
    public class BookingHistoryDto
    {
        public Guid Id { get; set; }
        public string? StatusSnapshot { get; set; }
        public Guid? SlotId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime StatTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? TimeStamp { get; set; }

    }
}
