namespace SmartPark.Dtos.Location
{
    public record LocationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? TotalSlots { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public DateTime? CreatedAt { get; set; }   // align with domain
        //public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public Guid? UpdatedBy { get; set; }
        public List<SlotResponseDto> Slots { get; set; } = new();

    }

    public record SlotResponseDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string SlotNumber { get; set; } = null!;
        public bool? IsAvailable { get; set; }
    }

}
