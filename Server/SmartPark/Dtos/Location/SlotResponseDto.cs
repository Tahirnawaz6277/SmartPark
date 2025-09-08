namespace SmartPark.Dtos.Location
{
    public record SlotResponseDto
    {
        public Guid Id { get; set; }
        public string? SlotType { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
