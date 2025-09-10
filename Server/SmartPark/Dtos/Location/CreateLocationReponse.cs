namespace SmartPark.Dtos.Location
{
    public class CreateLocationReponse
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public int? TotalSlots { get; set; }
    }
}
