namespace SmartPark.Dtos.Location
{
    public record CreateLocationRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int SmallSlotCount { get; set; } // Client provides count for small slots
        public int LargeSlotCount { get; set; } // Client provides count for large slots
        public int MediumSlotCount { get; set; } // Client provides count for large slots
        public string City { get; set; }
        public string Image { get; set; }
    }
}
