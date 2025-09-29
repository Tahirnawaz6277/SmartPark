namespace SmartPark.Dtos.Booking
{
    public record BookingResponse
    {
        public Guid Id { get; set; }
        public int? Duration { get; set; }
        public string? Status { get; set; }
        public DateTime? BookingDateTime { get; set; }
        public DateTime? ParkingStartTime { get; set; }
        public DateTime? ParkingEndTime { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SlotId { get; set; }
    }
}
