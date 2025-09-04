namespace SmartPark.Dtos
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public Guid RoleId { get; init; } 
        public string RoleName { get; init; } = string.Empty;

    }
}
