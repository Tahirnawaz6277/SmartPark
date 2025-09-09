namespace SmartPark.Dtos.Slot
{
    public record SlotResponseDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string? SlotType { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
