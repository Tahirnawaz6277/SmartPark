namespace SmartPark.Dtos.Location
{
    public record LocationRequest
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int TotalSlots { get; set; }
        public string City { get; set; } = null!;
        public string? Image { get; set; }
    }
}
