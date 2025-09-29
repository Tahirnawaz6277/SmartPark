namespace SmartPark.Dtos.Booking
{
    public record BookingHistoryResponse
    {
        public Guid Id { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? StatusSnapshot { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? SlotId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? UserId { get; set; }
    }
}
