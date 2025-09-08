namespace SmartPark.Dtos.Location
{
    public record LocationRequestDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? TotalSlots { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public Guid? UserId { get; set; }


    }
}
