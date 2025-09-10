namespace SmartPark.Dtos.Location
{
    public record LocationReponse
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public int? TotalSlots { get; set; }

    }
}
