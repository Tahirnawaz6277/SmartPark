namespace SmartPark.Dtos.UserDtos
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; } 
        public string? Email { get; init; } 
        public string? Address { get; init; } 
        public string? PhoneNumber { get; init; } 
        public string? City { get; init; }
        public Guid? RoleId { get; init; } 
        public string? RoleName { get; init; } 

    }

    public record ProfileDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? City { get; init; }
        public string? picture { get; set; }

    }
}
