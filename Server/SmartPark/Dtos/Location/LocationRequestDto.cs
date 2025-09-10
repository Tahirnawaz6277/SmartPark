namespace SmartPark.Dtos.Location
{
    public record LocationRequestDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int SmallSlotCount { get; set; } // Client provides count for small slots
        public int LargeSlotCount { get; set; } // Client provides count for large slots
        public string City { get; set; }
        public string Image { get; set; }
    }
}
