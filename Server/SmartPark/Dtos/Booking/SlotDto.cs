namespace SmartPark.Dtos.Booking
{
    public record SlotDto
    {
        public Guid SlotId { get; set; }
        public string SlotNumber { get; set; }
        public string? LocationName { get; set; }
    }
}
