namespace SmartPark.Dtos.Booking
{
    public record BookingResponse
    {
        public Guid Id { get; set; }
        public string Duration { get; set; }
        public string? Status { get; set; }
        public DateTime? BookingDateTime { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SlotId { get; set; }
    }
}
