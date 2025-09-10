using SmartPark.Dtos.Slot;

namespace SmartPark.Dtos.Location
{
    public record LocationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? TotalSlots { get; set; }
        public int? SmallSlots { get; set; }
        public int? LargeSlots { get; set; }
        public int? MediumSlots { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public List<SlotSummaryDto> Slots { get; set; } = new List<SlotSummaryDto>(); // Summary of slot counts

    }

    public class SlotSummaryDto
    {
        public string SlotType { get; set; }
        public int SlotCount { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
