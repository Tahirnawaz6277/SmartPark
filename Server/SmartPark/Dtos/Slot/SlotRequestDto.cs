namespace SmartPark.Dtos.Slot
{
    public record SlotRequestDto
    {
        public Guid LocationId { get; set; }
        public string? SlotType { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
