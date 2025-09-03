namespace SmartPark.Dtos
{
    public record UserResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Guid RoleId { get; set; }

    }
}
