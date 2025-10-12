namespace SmartPark.Dtos.Booking
{
    public record BookingResponse
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SlotId { get; set; }
    }
}
