using SmartPark.Dtos.Slot;

namespace SmartPark.Dtos.Location
{
    public record LocationResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? TotalSlots { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public Guid? UserId { get; set; }


        public List<SlotResponseDto> Slots { get; set; } = new();

    }
}
