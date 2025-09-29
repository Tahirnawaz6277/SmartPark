namespace SmartPark.Dtos.Booking
{
    public record BookingDto
    {
        public Guid Id { get; set; }
        public int? Duration { get; set; }
        public string? Status { get; set; }
        public DateTime? BookingDateTime { get; set; }
        public DateTime? ParkingStartTime { get; set; }
        public DateTime? ParkingEndTime { get; set; }
        public DateTime? TimeStamp { get; set; }

        // For lookup/display purposes
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public Guid? SlotId { get; set; }
        public string? SlotNumber { get; set; } // e.g. "A1", "B5"

        // Optional: last history snapshot
        public string? LastStatusSnapshot { get; set; }
    }
}
